namespace KateMorrisonMCP.Data.Models;

/// <summary>
/// Links characters to timeline events
/// </summary>
public class EventParticipant
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public int CharacterId { get; set; }
    public string? Role { get; set; } // primary, present, mentioned

    // For Dapper mapping
    public int event_id { get => EventId; set => EventId = value; }
    public int character_id { get => CharacterId; set => CharacterId = value; }
}
