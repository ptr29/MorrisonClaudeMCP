namespace KateMorrisonMCP.Data.Models;

/// <summary>
/// Represents a relationship between two characters
/// </summary>
public class Relationship
{
    public int Id { get; set; }
    public int CharacterAId { get; set; }
    public int CharacterBId { get; set; }
    public string RelationshipType { get; set; } = string.Empty; // romantic_partner, sibling, parent_child, friend, colleague
    public string? RelationshipSubtype { get; set; } // boyfriend_girlfriend, best_friend, mentor
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public string? CurrentStatus { get; set; } // active, ended, estranged, deceased
    public string? Direction { get; set; } // a_to_b, b_to_a, mutual
    public string? Notes { get; set; }

    // For Dapper mapping
    public int character_a_id { get => CharacterAId; set => CharacterAId = value; }
    public int character_b_id { get => CharacterBId; set => CharacterBId = value; }
    public string relationship_type { get => RelationshipType; set => RelationshipType = value; }
    public string? relationship_subtype { get => RelationshipSubtype; set => RelationshipSubtype = value; }
    public string? start_date { get => StartDate; set => StartDate = value; }
    public string? end_date { get => EndDate; set => EndDate = value; }
    public string? current_status { get => CurrentStatus; set => CurrentStatus = value; }
}
