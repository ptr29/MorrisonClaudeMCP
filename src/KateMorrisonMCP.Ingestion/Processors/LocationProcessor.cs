using KateMorrisonMCP.Data;
using KateMorrisonMCP.Ingestion.Helpers;
using KateMorrisonMCP.Ingestion.Parsing;

namespace KateMorrisonMCP.Ingestion.Processors;

/// <summary>
/// Processes location tags
/// </summary>
public class LocationProcessor : ITagProcessor
{
    private readonly DatabaseContext _db;

    public string TagType => "location";
    public int Priority => 2; // Locations must be created before rooms

    public LocationProcessor(DatabaseContext db)
    {
        _db = db;
    }

    public async Task<int> ProcessAsync(CanonicalTag tag)
    {
        var name = tag.GetRequired("name");

        // Check if location exists
        var existingId = await _db.QuerySingleOrDefaultAsync<int?>(
            "SELECT id FROM locations WHERE LOWER(name) = LOWER(@Name)",
            new { Name = name });

        // Parse notable_features (comma-delimited → JSON array)
        string? notableFeatures = null;
        if (tag.HasField("notable_features"))
        {
            notableFeatures = JsonArrayHelper.ToJsonArray(tag.GetOptional("notable_features"));
        }

        if (existingId.HasValue)
        {
            // Update existing location
            await _db.ExecuteAsync(@"
                UPDATE locations SET
                    location_type = COALESCE(@LocationType, location_type),
                    address_street = COALESCE(@AddressStreet, address_street),
                    address_city = COALESCE(@AddressCity, address_city),
                    address_state = COALESCE(@AddressState, address_state),
                    address_zip = COALESCE(@AddressZip, address_zip),
                    neighborhood = COALESCE(@Neighborhood, neighborhood),
                    nearby_landmarks = COALESCE(@NearbyLandmarks, nearby_landmarks),
                    source_file = @SourceFile,
                    updated_at = datetime('now')
                WHERE id = @Id",
                new
                {
                    Id = existingId.Value,
                    LocationType = tag.GetOptional("location_type"),
                    AddressStreet = tag.GetOptional("address"),
                    AddressCity = tag.GetOptional("city"),
                    AddressState = tag.GetOptional("state"),
                    AddressZip = tag.GetOptional("zip_code"),
                    Neighborhood = tag.GetOptional("neighborhood"),
                    NearbyLandmarks = notableFeatures,
                    SourceFile = tag.SourceFile
                });

            return existingId.Value;
        }
        else
        {
            // Insert new location
            await _db.ExecuteAsync(@"
                INSERT INTO locations (
                    name, location_type, address_street, address_city, address_state, address_zip,
                    neighborhood, nearby_landmarks,
                    source_file, created_at, updated_at
                ) VALUES (
                    @Name, @LocationType, @AddressStreet, @AddressCity, @AddressState, @AddressZip,
                    @Neighborhood, @NearbyLandmarks,
                    @SourceFile, datetime('now'), datetime('now')
                )",
                new
                {
                    Name = name,
                    LocationType = tag.GetOptional("location_type") ?? "building",
                    AddressStreet = tag.GetOptional("address"),
                    AddressCity = tag.GetOptional("city"),
                    AddressState = tag.GetOptional("state"),
                    AddressZip = tag.GetOptional("zip_code"),
                    Neighborhood = tag.GetOptional("neighborhood"),
                    NearbyLandmarks = notableFeatures,
                    SourceFile = tag.SourceFile
                });

            var newId = await _db.QuerySingleOrDefaultAsync<int>(
                "SELECT last_insert_rowid()");

            return newId;
        }
    }
}
