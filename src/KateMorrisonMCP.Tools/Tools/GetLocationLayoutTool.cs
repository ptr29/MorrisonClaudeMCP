using System.Text.Json;
using KateMorrisonMCP.Data.Repositories;
using KateMorrisonMCP.Tools.Models;

namespace KateMorrisonMCP.Tools.Tools;

/// <summary>
/// Returns complete layout information for a location
/// CRITICAL for Paul's house layout (22 rooms across 2 floors)
/// </summary>
public class GetLocationLayoutTool : ITool
{
    private readonly ILocationRepository _locationRepo;

    public GetLocationLayoutTool(ILocationRepository locationRepo)
    {
        _locationRepo = locationRepo;
    }

    public string Name => "get_location_layout";

    public string Description =>
        "Returns complete room-by-room layout for a location. " +
        "Prevents spatial errors like placing Kate's workspace in wrong room. " +
        "Critical for Paul's house (office on main floor, Kate's workspace in lower bedroom).";

    public ToolInputSchema InputSchema => new()
    {
        Type = "object",
        Properties = new Dictionary<string, PropertySchema>
        {
            ["location_name"] = new PropertySchema
            {
                Type = "string",
                Description = "Name or address of location (e.g., 'Paul's House', 'Skokie')"
            },
            ["floor"] = new PropertySchema
            {
                Type = "string",
                Description = "Optional: Specific floor to retrieve (e.g., 'main', 'lower')"
            }
        },
        Required = new List<string> { "location_name" }
    };

    public async Task<object> ExecuteAsync(JsonElement? arguments)
    {
        if (!arguments.HasValue)
        {
            return new { success = false, error = "Missing arguments" };
        }

        var args = arguments.Value;
        var locationName = args.TryGetProperty("location_name", out var locProp) ? locProp.GetString() : null;
        var floor = args.TryGetProperty("floor", out var f) ? f.GetString() : null;

        if (string.IsNullOrEmpty(locationName))
        {
            return new { success = false, error = "Missing location_name" };
        }

        // Find location
        var location = await _locationRepo.GetByNameOrAddressAsync(locationName);
        if (location == null)
        {
            return new
            {
                success = false,
                error = "Location not found",
                message = $"No location found matching '{locationName}'"
            };
        }

        // Get rooms
        var rooms = await _locationRepo.GetRoomsByLocationAsync(location.Id, floor);
        var roomsList = rooms.ToList();

        // Group by floor
        var floorGroups = roomsList
            .GroupBy(r => r.FloorLevel)
            .ToDictionary(
                g => g.Key,
                g => g.Select(r => new
                {
                    name = r.RoomName,
                    type = r.RoomType,
                    dimensions = r.FormattedDimensions,
                    current_use = r.CurrentUse,
                    key_features = r.KeyFeatures,
                    furniture = r.Furniture
                }).ToList()
            );

        return new
        {
            success = true,
            location = new
            {
                id = location.Id,
                name = location.Name,
                address = location.FormattedAddress,
                type = location.LocationType,
                building_type = location.BuildingType,
                floor_count = location.FloorCount
            },
            floors = floorGroups
        };
    }
}
