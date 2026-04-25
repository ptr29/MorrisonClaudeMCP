using System.Text.Json;

namespace KateMorrisonMCP.Data.Models;

/// <summary>
/// Represents a physical location (house, apartment, restaurant, etc.)
/// </summary>
public class Location
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? AddressStreet { get; set; }
    public string? AddressCity { get; set; }
    public string? AddressState { get; set; }
    public string? AddressZip { get; set; }
    public string LocationType { get; set; } = string.Empty; // house, apartment, restaurant, office, campus
    public string? BuildingType { get; set; }
    public int? FloorCount { get; set; }
    public string? UnitNumber { get; set; }
    public int? SquareFeet { get; set; }

    // Ownership
    public int? OwnerId { get; set; }
    public string? OwnershipType { get; set; } // owned, rented, family
    public decimal? MonthlyCost { get; set; }
    public string? PurchaseDate { get; set; }

    // Geographic context
    public string? Neighborhood { get; set; }
    public string? DistanceToChicagoLoop { get; set; }

    // JSON field for nearby landmarks
    private string? _nearbyLandmarks;
    public List<string>? NearbyLandmarks
    {
        get => string.IsNullOrEmpty(_nearbyLandmarks)
            ? null
            : JsonSerializer.Deserialize<List<string>>(_nearbyLandmarks);
        set => _nearbyLandmarks = value == null ? null : JsonSerializer.Serialize(value);
    }

    // Metadata
    public int IsFictional { get; set; } = 0;
    public string CreatedAt { get; set; } = string.Empty;
    public string UpdatedAt { get; set; } = string.Empty;

    // For Dapper mapping (snake_case)
    public string? address_street { get => AddressStreet; set => AddressStreet = value; }
    public string? address_city { get => AddressCity; set => AddressCity = value; }
    public string? address_state { get => AddressState; set => AddressState = value; }
    public string? address_zip { get => AddressZip; set => AddressZip = value; }
    public string location_type { get => LocationType; set => LocationType = value; }
    public string? building_type { get => BuildingType; set => BuildingType = value; }
    public int? floor_count { get => FloorCount; set => FloorCount = value; }
    public string? unit_number { get => UnitNumber; set => UnitNumber = value; }
    public int? square_feet { get => SquareFeet; set => SquareFeet = value; }
    public int? owner_id { get => OwnerId; set => OwnerId = value; }
    public string? ownership_type { get => OwnershipType; set => OwnershipType = value; }
    public decimal? monthly_cost { get => MonthlyCost; set => MonthlyCost = value; }
    public string? purchase_date { get => PurchaseDate; set => PurchaseDate = value; }
    public string? distance_to_chicago_loop { get => DistanceToChicagoLoop; set => DistanceToChicagoLoop = value; }
    public string? nearby_landmarks { get => _nearbyLandmarks; set => _nearbyLandmarks = value; }
    public int is_fictional { get => IsFictional; set => IsFictional = value; }
    public string created_at { get => CreatedAt; set => CreatedAt = value; }
    public string updated_at { get => UpdatedAt; set => UpdatedAt = value; }

    /// <summary>
    /// Format full address as readable string
    /// </summary>
    public string FormattedAddress =>
        string.Join(", ", new[] { AddressStreet, AddressCity, AddressState, AddressZip }
            .Where(s => !string.IsNullOrEmpty(s)));
}
