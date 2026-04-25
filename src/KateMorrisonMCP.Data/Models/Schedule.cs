using System.Text.Json;

namespace KateMorrisonMCP.Data.Models;

/// <summary>
/// Represents recurring schedules and routines for characters
/// CRITICAL: Used to prevent timing errors (e.g., Paul's 7 AM start)
/// </summary>
public class Schedule
{
    public int Id { get; set; }
    public int CharacterId { get; set; }
    public string ScheduleType { get; set; } = string.Empty; // work, exercise, weekly_commitment, daily_routine
    public string ScheduleName { get; set; } = string.Empty;

    // JSON field for days of week array
    private string? _daysOfWeek;
    public List<string>? DaysOfWeek
    {
        get => string.IsNullOrEmpty(_daysOfWeek)
            ? null
            : JsonSerializer.Deserialize<List<string>>(_daysOfWeek);
        set => _daysOfWeek = value == null ? null : JsonSerializer.Serialize(value);
    }

    public string? StartTime { get; set; } // HH:MM format
    public string? EndTime { get; set; }
    public int? DurationMinutes { get; set; }

    // Location
    public int? LocationId { get; set; }
    public string? LocationDescription { get; set; }

    // Details
    public string? Description { get; set; }

    // JSON field for exceptions array
    private string? _exceptions;
    public List<string>? Exceptions
    {
        get => string.IsNullOrEmpty(_exceptions)
            ? null
            : JsonSerializer.Deserialize<List<string>>(_exceptions);
        set => _exceptions = value == null ? null : JsonSerializer.Serialize(value);
    }

    // Validity
    public string? EffectiveFrom { get; set; }
    public string? EffectiveUntil { get; set; }
    public int IsCurrent { get; set; } = 1;

    // For Dapper mapping (snake_case database columns)
    public int character_id
    {
        get => CharacterId;
        set => CharacterId = value;
    }

    public string schedule_type
    {
        get => ScheduleType;
        set => ScheduleType = value;
    }

    public string schedule_name
    {
        get => ScheduleName;
        set => ScheduleName = value;
    }

    public string? days_of_week
    {
        get => _daysOfWeek;
        set => _daysOfWeek = value;
    }

    public string? start_time
    {
        get => StartTime;
        set => StartTime = value;
    }

    public string? end_time
    {
        get => EndTime;
        set => EndTime = value;
    }

    public int? duration_minutes
    {
        get => DurationMinutes;
        set => DurationMinutes = value;
    }

    public int? location_id
    {
        get => LocationId;
        set => LocationId = value;
    }

    public string? location_description
    {
        get => LocationDescription;
        set => LocationDescription = value;
    }

    public string? exceptions
    {
        get => _exceptions;
        set => _exceptions = value;
    }

    public string? effective_from
    {
        get => EffectiveFrom;
        set => EffectiveFrom = value;
    }

    public string? effective_until
    {
        get => EffectiveUntil;
        set => EffectiveUntil = value;
    }

    public int is_current
    {
        get => IsCurrent;
        set => IsCurrent = value;
    }
}
