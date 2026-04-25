using KateMorrisonMCP.Data;
using KateMorrisonMCP.Ingestion.Helpers;
using KateMorrisonMCP.Ingestion.Parsing;

namespace KateMorrisonMCP.Ingestion.Processors;

/// <summary>
/// Processes schedule tags
/// </summary>
public class ScheduleProcessor : ITagProcessor
{
    private readonly DatabaseContext _db;
    private readonly CharacterLookup _characterLookup;

    public string TagType => "schedule";
    public int Priority => 4; // Schedules depend on characters

    public ScheduleProcessor(DatabaseContext db, CharacterLookup characterLookup)
    {
        _db = db;
        _characterLookup = characterLookup;
    }

    public async Task<int> ProcessAsync(CanonicalTag tag)
    {
        var characterName = tag.GetRequired("character");
        var scheduleName = tag.GetRequired("schedule_name");

        // Look up character_id
        var characterId = await _characterLookup.GetRequiredIdAsync(characterName, tag.SourceFile);

        // Parse days (weekdays, weekends, specific days, etc.)
        string? days = null;
        if (tag.HasField("days"))
        {
            days = DaysParser.Parse(tag.GetOptional("days"));
        }

        // Parse exceptions (semicolon or comma-delimited → JSON array)
        string? exceptions = null;
        if (tag.HasField("exceptions"))
        {
            exceptions = JsonArrayHelper.ToJsonArray(tag.GetOptional("exceptions"), [',', ';']);
        }

        // Check if schedule exists
        var existingId = await _db.QuerySingleOrDefaultAsync<int?>(
            "SELECT id FROM schedules WHERE character_id = @CharacterId AND LOWER(schedule_name) = LOWER(@ScheduleName)",
            new { CharacterId = characterId, ScheduleName = scheduleName });

        if (existingId.HasValue)
        {
            // Update existing schedule
            await _db.ExecuteAsync(@"
                UPDATE schedules SET
                    activity_type = COALESCE(@ActivityType, activity_type),
                    start_time = COALESCE(@StartTime, start_time),
                    end_time = COALESCE(@EndTime, end_time),
                    days = COALESCE(@Days, days),
                    location = COALESCE(@Location, location),
                    frequency = COALESCE(@Frequency, frequency),
                    exceptions = COALESCE(@Exceptions, exceptions),
                    notes = COALESCE(@Notes, notes),
                    source_file = @SourceFile
                WHERE id = @Id",
                new
                {
                    Id = existingId.Value,
                    ActivityType = tag.GetOptional("activity_type"),
                    StartTime = tag.GetOptional("start_time"),
                    EndTime = tag.GetOptional("end_time"),
                    Days = days,
                    Location = tag.GetOptional("location"),
                    Frequency = tag.GetOptional("frequency"),
                    Exceptions = exceptions,
                    Notes = tag.GetOptional("notes"),
                    SourceFile = tag.SourceFile
                });

            return existingId.Value;
        }
        else
        {
            // Insert new schedule
            await _db.ExecuteAsync(@"
                INSERT INTO schedules (
                    character_id, schedule_name, activity_type, start_time, end_time,
                    days, location, frequency, exceptions, notes, source_file
                ) VALUES (
                    @CharacterId, @ScheduleName, @ActivityType, @StartTime, @EndTime,
                    @Days, @Location, @Frequency, @Exceptions, @Notes, @SourceFile
                )",
                new
                {
                    CharacterId = characterId,
                    ScheduleName = scheduleName,
                    ActivityType = tag.GetOptional("activity_type") ?? "routine",
                    StartTime = tag.GetOptional("start_time"),
                    EndTime = tag.GetOptional("end_time"),
                    Days = days,
                    Location = tag.GetOptional("location"),
                    Frequency = tag.GetOptional("frequency") ?? "weekly",
                    Exceptions = exceptions,
                    Notes = tag.GetOptional("notes"),
                    SourceFile = tag.SourceFile
                });

            var newId = await _db.QuerySingleOrDefaultAsync<int>(
                "SELECT last_insert_rowid()");

            return newId;
        }
    }
}
