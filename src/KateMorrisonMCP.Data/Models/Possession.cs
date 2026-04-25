namespace KateMorrisonMCP.Data.Models;

/// <summary>
/// Represents an item owned by a character
/// </summary>
public class Possession
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? ItemCategory { get; set; } // vehicle, jewelry, electronics, clothing, furniture
    public string? ItemDescription { get; set; }

    // Acquisition
    public string? AcquisitionDate { get; set; }
    public string? AcquisitionMethod { get; set; } // purchased, gift, inherited
    public string? AcquisitionFrom { get; set; }

    // Storage
    public int? LocationId { get; set; }
    public string? StorageLocation { get; set; }

    // Significance
    public decimal? MonetaryValue { get; set; }
    public string? SentimentalValue { get; set; } // none, low, medium, high, critical
    public string? SignificanceNotes { get; set; }

    // Status
    public int IsCurrent { get; set; } = 1;

    // For Dapper mapping
    public int owner_id { get => OwnerId; set => OwnerId = value; }
    public string item_name { get => ItemName; set => ItemName = value; }
    public string? item_category { get => ItemCategory; set => ItemCategory = value; }
    public string? item_description { get => ItemDescription; set => ItemDescription = value; }
    public string? acquisition_date { get => AcquisitionDate; set => AcquisitionDate = value; }
    public string? acquisition_method { get => AcquisitionMethod; set => AcquisitionMethod = value; }
    public string? acquisition_from { get => AcquisitionFrom; set => AcquisitionFrom = value; }
    public int? location_id { get => LocationId; set => LocationId = value; }
    public string? storage_location { get => StorageLocation; set => StorageLocation = value; }
    public decimal? monetary_value { get => MonetaryValue; set => MonetaryValue = value; }
    public string? sentimental_value { get => SentimentalValue; set => SentimentalValue = value; }
    public string? significance_notes { get => SignificanceNotes; set => SignificanceNotes = value; }
    public int is_current { get => IsCurrent; set => IsCurrent = value; }
}
