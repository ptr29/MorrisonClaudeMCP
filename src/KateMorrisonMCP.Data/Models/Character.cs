using System.Text.Json;

namespace KateMorrisonMCP.Data.Models;

/// <summary>
/// Represents a character in the story with biographical and physical data
/// </summary>
public class Character
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? PreferredName { get; set; }
    public int? Age { get; set; }
    public string? Birthday { get; set; } // YYYY-MM-DD format
    public int? BirthYear { get; set; }

    // Physical attributes
    public int? HeightInches { get; set; }
    public int? WeightLbs { get; set; }
    public string? Build { get; set; }
    public string? HairColor { get; set; }
    public string? HairLength { get; set; }
    public string? EyeColor { get; set; }

    // JSON field for distinctive features array
    private string? _distinctiveFeatures;
    public List<string>? DistinctiveFeatures
    {
        get => string.IsNullOrEmpty(_distinctiveFeatures)
            ? null
            : JsonSerializer.Deserialize<List<string>>(_distinctiveFeatures);
        set => _distinctiveFeatures = value == null ? null : JsonSerializer.Serialize(value);
    }

    // Occupation
    public string? Occupation { get; set; }
    public string? Employer { get; set; }
    public string? JobTitle { get; set; }
    public string? WorkLocation { get; set; }
    public string? WorkScheduleType { get; set; } // remote, hybrid, onsite

    // Contact
    public string? Phone { get; set; }
    public string? Email { get; set; }

    // Residence
    public int? ResidenceId { get; set; }

    // Metadata
    public string CharacterType { get; set; } = "secondary"; // primary, secondary, supporting
    public int IsAlive { get; set; } = 1;
    public string CreatedAt { get; set; } = string.Empty;
    public string UpdatedAt { get; set; } = string.Empty;

    // For Dapper mapping (snake_case database columns)
    public string? distinctive_features
    {
        get => _distinctiveFeatures;
        set => _distinctiveFeatures = value;
    }

    public string? preferred_name
    {
        get => PreferredName;
        set => PreferredName = value;
    }

    public int? height_inches
    {
        get => HeightInches;
        set => HeightInches = value;
    }

    public int? weight_lbs
    {
        get => WeightLbs;
        set => WeightLbs = value;
    }

    public string? hair_color
    {
        get => HairColor;
        set => HairColor = value;
    }

    public string? hair_length
    {
        get => HairLength;
        set => HairLength = value;
    }

    public string? eye_color
    {
        get => EyeColor;
        set => EyeColor = value;
    }

    public string? job_title
    {
        get => JobTitle;
        set => JobTitle = value;
    }

    public string? work_location
    {
        get => WorkLocation;
        set => WorkLocation = value;
    }

    public string? work_schedule_type
    {
        get => WorkScheduleType;
        set => WorkScheduleType = value;
    }

    public int? residence_id
    {
        get => ResidenceId;
        set => ResidenceId = value;
    }

    public string character_type
    {
        get => CharacterType;
        set => CharacterType = value;
    }

    public int is_alive
    {
        get => IsAlive;
        set => IsAlive = value;
    }

    public string created_at
    {
        get => CreatedAt;
        set => CreatedAt = value;
    }

    public string updated_at
    {
        get => UpdatedAt;
        set => UpdatedAt = value;
    }

    /// <summary>
    /// Format height as readable string (e.g., "5'6\"")
    /// </summary>
    public string? FormattedHeight =>
        HeightInches.HasValue
            ? $"{HeightInches / 12}'{HeightInches % 12}\""
            : null;
}
