using KateMorrisonMCP.Data;
using KateMorrisonMCP.Ingestion.Helpers;
using KateMorrisonMCP.Ingestion.Parsing;

namespace KateMorrisonMCP.Ingestion.Processors;

/// <summary>
/// Processes timeline event tags
/// Most complex processor - handles both timeline_events and event_participants tables
/// </summary>
public class TimelineProcessor : ITagProcessor
{
    private readonly DatabaseContext _db;
    private readonly CharacterLookup _characterLookup;

    public string TagType => "timeline";
    public int Priority => 6; // Timeline depends on both characters and locations

    public TimelineProcessor(DatabaseContext db, CharacterLookup characterLookup)
    {
        _db = db;
        _characterLookup = characterLookup;
    }

    public async Task<int> ProcessAsync(CanonicalTag tag)
    {
        var eventDate = tag.GetRequired("event_date");
        var eventTitle = tag.GetRequired("event_title");

        // Look up optional character_id
        int? characterId = null;
        if (tag.HasField("character"))
        {
            characterId = await _characterLookup.GetIdAsync(tag.GetOptional("character")!);
        }

        // Look up optional location_id
        int? locationId = null;
        if (tag.HasField("location"))
        {
            var locationName = tag.GetOptional("location");
            locationId = await _db.QuerySingleOrDefaultAsync<int?>(
                "SELECT id FROM locations WHERE LOWER(name) = LOWER(@Name)",
                new { Name = locationName });
        }

        // Check if timeline event exists
        var existingId = await _db.QuerySingleOrDefaultAsync<int?>(
            @"SELECT id FROM timeline_events
              WHERE event_date = @EventDate
              AND LOWER(event_title) = LOWER(@EventTitle)",
            new { EventDate = eventDate, EventTitle = eventTitle });

        if (existingId.HasValue)
        {
            // Update existing timeline event
            await _db.ExecuteAsync(@"
                UPDATE timeline_events SET
                    character_id = COALESCE(@CharacterId, character_id),
                    location_id = COALESCE(@LocationId, location_id),
                    event_type = COALESCE(@EventType, event_type),
                    description = COALESCE(@Description, description),
                    notes = COALESCE(@Notes, notes),
                    source_file = @SourceFile
                WHERE id = @Id",
                new
                {
                    Id = existingId.Value,
                    CharacterId = characterId,
                    LocationId = locationId,
                    EventType = tag.GetOptional("event_type"),
                    Description = tag.GetOptional("description"),
                    Notes = tag.GetOptional("notes"),
                    SourceFile = tag.SourceFile
                });

            // Handle participants (delete and re-insert)
            await UpdateParticipantsAsync(existingId.Value, tag);

            return existingId.Value;
        }
        else
        {
            // Insert new timeline event
            await _db.ExecuteAsync(@"
                INSERT INTO timeline_events (
                    event_date, event_title, character_id, location_id,
                    event_type, description, notes, source_file
                ) VALUES (
                    @EventDate, @EventTitle, @CharacterId, @LocationId,
                    @EventType, @Description, @Notes, @SourceFile
                )",
                new
                {
                    EventDate = eventDate,
                    EventTitle = eventTitle,
                    CharacterId = characterId,
                    LocationId = locationId,
                    EventType = tag.GetOptional("event_type") ?? "event",
                    Description = tag.GetOptional("description"),
                    Notes = tag.GetOptional("notes"),
                    SourceFile = tag.SourceFile
                });

            var newId = await _db.QuerySingleOrDefaultAsync<int>(
                "SELECT last_insert_rowid()");

            // Handle participants
            await UpdateParticipantsAsync(newId, tag);

            return newId;
        }
    }

    /// <summary>
    /// Updates event participants (deletes existing, inserts new ones)
    /// </summary>
    private async Task UpdateParticipantsAsync(int timelineEventId, CanonicalTag tag)
    {
        if (!tag.HasField("participants"))
        {
            return;
        }

        // Delete existing participants
        await _db.ExecuteAsync(
            "DELETE FROM event_participants WHERE timeline_event_id = @TimelineEventId",
            new { TimelineEventId = timelineEventId });

        // Parse comma-separated participant names
        var participantsText = tag.GetOptional("participants");
        if (string.IsNullOrWhiteSpace(participantsText))
        {
            return;
        }

        var participantNames = participantsText
            .Split(',')
            .Select(n => n.Trim())
            .Where(n => !string.IsNullOrEmpty(n))
            .ToList();

        // Look up each participant and insert
        foreach (var participantName in participantNames)
        {
            var participantId = await _characterLookup.GetIdAsync(participantName);

            if (participantId.HasValue)
            {
                await _db.ExecuteAsync(@"
                    INSERT INTO event_participants (timeline_event_id, character_id)
                    VALUES (@TimelineEventId, @CharacterId)",
                    new { TimelineEventId = timelineEventId, CharacterId = participantId.Value });
            }
            else
            {
                Console.WriteLine($"Warning: Participant '{participantName}' not found in {tag.SourceFile}:{tag.LineNumber}");
            }
        }
    }
}
