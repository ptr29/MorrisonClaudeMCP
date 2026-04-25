using KateMorrisonMCP.Data;
using KateMorrisonMCP.Ingestion.Helpers;
using KateMorrisonMCP.Ingestion.Parsing;

namespace KateMorrisonMCP.Ingestion.Processors;

/// <summary>
/// Processes room tags
/// </summary>
public class RoomProcessor : ITagProcessor
{
    private readonly DatabaseContext _db;

    public string TagType => "room";
    public int Priority => 3; // Rooms depend on locations

    public RoomProcessor(DatabaseContext db)
    {
        _db = db;
    }

    public async Task<int> ProcessAsync(CanonicalTag tag)
    {
        var roomName = tag.GetRequired("room_name");
        var locationName = tag.GetRequired("location");

        // Look up location_id
        var locationId = await _db.QuerySingleOrDefaultAsync<int?>(
            "SELECT id FROM locations WHERE LOWER(name) = LOWER(@Name)",
            new { Name = locationName });

        if (!locationId.HasValue)
        {
            throw new ArgumentException($"Location not found: {locationName}");
        }

        // Parse dimensions
        decimal? widthFeet = null;
        decimal? lengthFeet = null;
        if (tag.HasField("dimensions"))
        {
            var (width, length) = DimensionParser.Parse(tag.GetOptional("dimensions"));
            widthFeet = width;
            lengthFeet = length;
        }

        // Parse key_features and furniture (comma-delimited → JSON arrays)
        string? keyFeatures = null;
        if (tag.HasField("key_features"))
        {
            keyFeatures = JsonArrayHelper.ToJsonArray(tag.GetOptional("key_features"));
        }

        string? furniture = null;
        if (tag.HasField("furniture"))
        {
            furniture = JsonArrayHelper.ToJsonArray(tag.GetOptional("furniture"));
        }

        // Check if room exists
        var existingId = await _db.QuerySingleOrDefaultAsync<int?>(
            "SELECT id FROM location_rooms WHERE location_id = @LocationId AND LOWER(room_name) = LOWER(@RoomName)",
            new { LocationId = locationId.Value, RoomName = roomName });

        if (existingId.HasValue)
        {
            // Update existing room
            await _db.ExecuteAsync(@"
                UPDATE location_rooms SET
                    room_type = COALESCE(@RoomType, room_type),
                    floor_level = COALESCE(@FloorLevel, floor_level),
                    width_feet = COALESCE(@WidthFeet, width_feet),
                    length_feet = COALESCE(@LengthFeet, length_feet),
                    key_features = COALESCE(@KeyFeatures, key_features),
                    furniture = COALESCE(@Furniture, furniture),
                    access_notes = COALESCE(@AccessNotes, access_notes),
                    source_file = @SourceFile
                WHERE id = @Id",
                new
                {
                    Id = existingId.Value,
                    RoomType = tag.GetOptional("room_type"),
                    FloorLevel = tag.GetOptionalInt("floor_level"),
                    WidthFeet = widthFeet,
                    LengthFeet = lengthFeet,
                    KeyFeatures = keyFeatures,
                    Furniture = furniture,
                    AccessNotes = tag.GetOptional("access_notes"),
                    SourceFile = tag.SourceFile
                });

            return existingId.Value;
        }
        else
        {
            // Insert new room
            await _db.ExecuteAsync(@"
                INSERT INTO location_rooms (
                    location_id, room_name, room_type, floor_level,
                    width_feet, length_feet, key_features, furniture, access_notes,
                    source_file
                ) VALUES (
                    @LocationId, @RoomName, @RoomType, @FloorLevel,
                    @WidthFeet, @LengthFeet, @KeyFeatures, @Furniture, @AccessNotes,
                    @SourceFile
                )",
                new
                {
                    LocationId = locationId.Value,
                    RoomName = roomName,
                    RoomType = tag.GetOptional("room_type") ?? "room",
                    FloorLevel = tag.GetOptionalInt("floor_level"),
                    WidthFeet = widthFeet,
                    LengthFeet = lengthFeet,
                    KeyFeatures = keyFeatures,
                    Furniture = furniture,
                    AccessNotes = tag.GetOptional("access_notes"),
                    SourceFile = tag.SourceFile
                });

            var newId = await _db.QuerySingleOrDefaultAsync<int>(
                "SELECT last_insert_rowid()");

            return newId;
        }
    }
}
