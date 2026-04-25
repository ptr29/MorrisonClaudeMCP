namespace KateMorrisonMCP.Data.Models;

/// <summary>
/// Represents a canonical timeline event in the story
/// </summary>
public class TimelineEvent
{
    public int Id { get; set; }
    public string EventDate { get; set; } = string.Empty; // ISO format: YYYY-MM-DD or YYYY-MM-DDTHH:MM:SS
    public string EventDatePrecision { get; set; } = "day"; // year, month, day, time
    public string EventTitle { get; set; } = string.Empty;
    public string? EventDescription { get; set; }

    // Location
    public int? LocationId { get; set; }
    public string? LocationDescription { get; set; }

    // Categorization
    public string? EventCategory { get; set; } // relationship_milestone, career, family, death, etc.
    public string? Significance { get; set; } // major, minor, background

    // Verification
    public int IsCanonical { get; set; } = 1;
    public string? SourceFile { get; set; }
    public string CreatedAt { get; set; } = string.Empty;

    // For Dapper mapping
    public string event_date { get => EventDate; set => EventDate = value; }
    public string event_date_precision { get => EventDatePrecision; set => EventDatePrecision = value; }
    public string event_title { get => EventTitle; set => EventTitle = value; }
    public string? event_description { get => EventDescription; set => EventDescription = value; }
    public int? location_id { get => LocationId; set => LocationId = value; }
    public string? location_description { get => LocationDescription; set => LocationDescription = value; }
    public string? event_category { get => EventCategory; set => EventCategory = value; }
    public int is_canonical { get => IsCanonical; set => IsCanonical = value; }
    public string? source_file { get => SourceFile; set => SourceFile = value; }
    public string created_at { get => CreatedAt; set => CreatedAt = value; }
}
