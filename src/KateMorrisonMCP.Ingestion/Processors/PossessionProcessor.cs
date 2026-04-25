using KateMorrisonMCP.Data;
using KateMorrisonMCP.Ingestion.Helpers;
using KateMorrisonMCP.Ingestion.Parsing;

namespace KateMorrisonMCP.Ingestion.Processors;

/// <summary>
/// Processes possession tags
/// </summary>
public class PossessionProcessor : ITagProcessor
{
    private readonly DatabaseContext _db;
    private readonly CharacterLookup _characterLookup;

    public string TagType => "possession";
    public int Priority => 5; // Possessions depend on characters

    public PossessionProcessor(DatabaseContext db, CharacterLookup characterLookup)
    {
        _db = db;
        _characterLookup = characterLookup;
    }

    public async Task<int> ProcessAsync(CanonicalTag tag)
    {
        var ownerName = tag.GetRequired("owner");
        var itemName = tag.GetRequired("item_name");

        // Look up owner_id (character_id)
        var ownerId = await _characterLookup.GetRequiredIdAsync(ownerName, tag.SourceFile);

        // Check if possession exists
        var existingId = await _db.QuerySingleOrDefaultAsync<int?>(
            @"SELECT id FROM possessions
              WHERE owner_id = @OwnerId
              AND LOWER(item_name) = LOWER(@ItemName)",
            new { OwnerId = ownerId, ItemName = itemName });

        if (existingId.HasValue)
        {
            // Update existing possession
            await _db.ExecuteAsync(@"
                UPDATE possessions SET
                    item_type = COALESCE(@ItemType, item_type),
                    description = COALESCE(@Description, description),
                    current_location = COALESCE(@CurrentLocation, current_location),
                    acquisition_date = COALESCE(@AcquisitionDate, acquisition_date),
                    notes = COALESCE(@Notes, notes),
                    source_file = @SourceFile
                WHERE id = @Id",
                new
                {
                    Id = existingId.Value,
                    ItemType = tag.GetOptional("item_type"),
                    Description = tag.GetOptional("description"),
                    CurrentLocation = tag.GetOptional("current_location"),
                    AcquisitionDate = tag.GetOptional("acquisition_date"),
                    Notes = tag.GetOptional("notes"),
                    SourceFile = tag.SourceFile
                });

            return existingId.Value;
        }
        else
        {
            // Insert new possession
            await _db.ExecuteAsync(@"
                INSERT INTO possessions (
                    owner_id, item_name, item_type, description,
                    current_location, acquisition_date, notes, source_file
                ) VALUES (
                    @OwnerId, @ItemName, @ItemType, @Description,
                    @CurrentLocation, @AcquisitionDate, @Notes, @SourceFile
                )",
                new
                {
                    OwnerId = ownerId,
                    ItemName = itemName,
                    ItemType = tag.GetOptional("item_type") ?? "personal",
                    Description = tag.GetOptional("description"),
                    CurrentLocation = tag.GetOptional("current_location"),
                    AcquisitionDate = tag.GetOptional("acquisition_date"),
                    Notes = tag.GetOptional("notes"),
                    SourceFile = tag.SourceFile
                });

            var newId = await _db.QuerySingleOrDefaultAsync<int>(
                "SELECT last_insert_rowid()");

            return newId;
        }
    }
}
