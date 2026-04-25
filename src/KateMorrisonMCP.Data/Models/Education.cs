using System.Text.Json;

namespace KateMorrisonMCP.Data.Models;

/// <summary>
/// Represents educational history for a character
/// </summary>
public class Education
{
    public int Id { get; set; }
    public int CharacterId { get; set; }
    public string Institution { get; set; } = string.Empty;
    public string? DegreeType { get; set; } // BS, BA, MS, PhD, MBA, etc.
    public string? FieldOfStudy { get; set; }
    public int? StartYear { get; set; }
    public int? EndYear { get; set; }
    public int IsCompleted { get; set; } = 1;
    public int IsCurrent { get; set; } = 0;

    // JSON field for honors
    private string? _honors;
    public List<string>? Honors
    {
        get => string.IsNullOrEmpty(_honors)
            ? null
            : JsonSerializer.Deserialize<List<string>>(_honors);
        set => _honors = value == null ? null : JsonSerializer.Serialize(value);
    }

    public string? ThesisTitle { get; set; }
    public string? Advisor { get; set; }
    public string? Notes { get; set; }

    // For Dapper mapping
    public int character_id { get => CharacterId; set => CharacterId = value; }
    public string? degree_type { get => DegreeType; set => DegreeType = value; }
    public string? field_of_study { get => FieldOfStudy; set => FieldOfStudy = value; }
    public int? start_year { get => StartYear; set => StartYear = value; }
    public int? end_year { get => EndYear; set => EndYear = value; }
    public int is_completed { get => IsCompleted; set => IsCompleted = value; }
    public int is_current { get => IsCurrent; set => IsCurrent = value; }
    public string? honors { get => _honors; set => _honors = value; }
    public string? thesis_title { get => ThesisTitle; set => ThesisTitle = value; }
}
