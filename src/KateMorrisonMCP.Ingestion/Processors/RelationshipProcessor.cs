using KateMorrisonMCP.Data;
using KateMorrisonMCP.Ingestion.Helpers;
using KateMorrisonMCP.Ingestion.Parsing;

namespace KateMorrisonMCP.Ingestion.Processors;

/// <summary>
/// Processes relationship tags
/// </summary>
public class RelationshipProcessor : ITagProcessor
{
    private readonly DatabaseContext _db;
    private readonly CharacterLookup _characterLookup;

    public string TagType => "relationship";
    public int Priority => 5; // Relationships depend on characters

    public RelationshipProcessor(DatabaseContext db, CharacterLookup characterLookup)
    {
        _db = db;
        _characterLookup = characterLookup;
    }

    public async Task<int> ProcessAsync(CanonicalTag tag)
    {
        var characterAName = tag.GetRequired("character_a");
        var characterBName = tag.GetRequired("character_b");
        var relationshipType = tag.GetRequired("relationship_type");

        // Look up character IDs
        var characterAId = await _characterLookup.GetRequiredIdAsync(characterAName, tag.SourceFile);
        var characterBId = await _characterLookup.GetRequiredIdAsync(characterBName, tag.SourceFile);

        // Check if relationship exists
        var existingId = await _db.QuerySingleOrDefaultAsync<int?>(
            @"SELECT id FROM relationships
              WHERE character_a_id = @CharacterAId
              AND character_b_id = @CharacterBId
              AND LOWER(relationship_type) = LOWER(@RelationshipType)",
            new { CharacterAId = characterAId, CharacterBId = characterBId, RelationshipType = relationshipType });

        if (existingId.HasValue)
        {
            // Update existing relationship
            await _db.ExecuteAsync(@"
                UPDATE relationships SET
                    relationship_status = COALESCE(@RelationshipStatus, relationship_status),
                    started_date = COALESCE(@StartedDate, started_date),
                    ended_date = COALESCE(@EndedDate, ended_date),
                    notes = COALESCE(@Notes, notes),
                    source_file = @SourceFile
                WHERE id = @Id",
                new
                {
                    Id = existingId.Value,
                    RelationshipStatus = tag.GetOptional("relationship_status"),
                    StartedDate = tag.GetOptional("started_date"),
                    EndedDate = tag.GetOptional("ended_date"),
                    Notes = tag.GetOptional("notes"),
                    SourceFile = tag.SourceFile
                });

            return existingId.Value;
        }
        else
        {
            // Insert new relationship
            await _db.ExecuteAsync(@"
                INSERT INTO relationships (
                    character_a_id, character_b_id, relationship_type,
                    relationship_status, started_date, ended_date, notes, source_file
                ) VALUES (
                    @CharacterAId, @CharacterBId, @RelationshipType,
                    @RelationshipStatus, @StartedDate, @EndedDate, @Notes, @SourceFile
                )",
                new
                {
                    CharacterAId = characterAId,
                    CharacterBId = characterBId,
                    RelationshipType = relationshipType,
                    RelationshipStatus = tag.GetOptional("relationship_status") ?? "current",
                    StartedDate = tag.GetOptional("started_date"),
                    EndedDate = tag.GetOptional("ended_date"),
                    Notes = tag.GetOptional("notes"),
                    SourceFile = tag.SourceFile
                });

            var newId = await _db.QuerySingleOrDefaultAsync<int>(
                "SELECT last_insert_rowid()");

            return newId;
        }
    }
}
