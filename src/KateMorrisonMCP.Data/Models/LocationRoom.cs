using System.Text.Json;

namespace KateMorrisonMCP.Data.Models;

/// <summary>
/// Represents a room within a location with detailed layout information
/// </summary>
public class LocationRoom
{
    public int Id { get; set; }
    public int LocationId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public string FloorLevel { get; set; } = string.Empty; // main, upper, lower, basement, 1, 2, 3
    public string RoomType { get; set; } = string.Empty; // bedroom, bathroom, kitchen, living, office, etc.

    // Dimensions
    public decimal? WidthFeet { get; set; }
    public decimal? LengthFeet { get; set; }
    public decimal? WidthInches { get; set; }
    public decimal? LengthInches { get; set; }

    // Description
    public string? CurrentUse { get; set; }

    // JSON fields
    private string? _keyFeatures;
    public List<string>? KeyFeatures
    {
        get => string.IsNullOrEmpty(_keyFeatures)
            ? null
            : JsonSerializer.Deserialize<List<string>>(_keyFeatures);
        set => _keyFeatures = value == null ? null : JsonSerializer.Serialize(value);
    }

    private string? _furniture;
    public List<string>? Furniture
    {
        get => string.IsNullOrEmpty(_furniture)
            ? null
            : JsonSerializer.Deserialize<List<string>>(_furniture);
        set => _furniture = value == null ? null : JsonSerializer.Serialize(value);
    }

    private string? _adjacentTo;
    public List<string>? AdjacentTo
    {
        get => string.IsNullOrEmpty(_adjacentTo)
            ? null
            : JsonSerializer.Deserialize<List<string>>(_adjacentTo);
        set => _adjacentTo = value == null ? null : JsonSerializer.Serialize(value);
    }

    // Position
    public int HasExteriorWindow { get; set; } = 0;
    public string? WindowDirection { get; set; }

    // For Dapper mapping
    public int location_id { get => LocationId; set => LocationId = value; }
    public string room_name { get => RoomName; set => RoomName = value; }
    public string floor_level { get => FloorLevel; set => FloorLevel = value; }
    public string room_type { get => RoomType; set => RoomType = value; }
    public decimal? width_feet { get => WidthFeet; set => WidthFeet = value; }
    public decimal? length_feet { get => LengthFeet; set => LengthFeet = value; }
    public decimal? width_inches { get => WidthInches; set => WidthInches = value; }
    public decimal? length_inches { get => LengthInches; set => LengthInches = value; }
    public string? current_use { get => CurrentUse; set => CurrentUse = value; }
    public string? key_features { get => _keyFeatures; set => _keyFeatures = value; }
    public string? furniture { get => _furniture; set => _furniture = value; }
    public string? adjacent_to { get => _adjacentTo; set => _adjacentTo = value; }
    public int has_exterior_window { get => HasExteriorWindow; set => HasExteriorWindow = value; }
    public string? window_direction { get => WindowDirection; set => WindowDirection = value; }

    /// <summary>
    /// Format dimensions as readable string (e.g., "14'11\" × 17'5\"")
    /// </summary>
    public string? FormattedDimensions
    {
        get
        {
            if (!WidthFeet.HasValue || !LengthFeet.HasValue)
                return null;

            var widthInches = WidthInches ?? 0;
            var lengthInches = LengthInches ?? 0;

            return $"{WidthFeet}'{widthInches}\" × {LengthFeet}'{lengthInches}\"";
        }
    }
}
