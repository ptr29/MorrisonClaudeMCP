using System.Text.Json;
using KateMorrisonMCP.Data.Repositories;
using KateMorrisonMCP.Tools.Models;

namespace KateMorrisonMCP.Tools.Tools;

/// <summary>
/// HIGH PRIORITY TOOL: Returns character schedules with timing warnings
/// CRITICAL for Paul's 7 AM work start (common error: assuming 9 AM)
/// </summary>
public class GetScheduleTool : ITool
{
    private readonly IScheduleRepository _scheduleRepo;
    private readonly ICharacterRepository _characterRepo;

    public GetScheduleTool(IScheduleRepository scheduleRepo, ICharacterRepository characterRepo)
    {
        _scheduleRepo = scheduleRepo;
        _characterRepo = characterRepo;
    }

    public string Name => "get_schedule";

    public string Description =>
        "Returns a character's schedule with optional filtering by day or type. " +
        "CRITICAL for preventing timing errors (e.g., Paul starts work at 7 AM, NOT 9 AM). " +
        "Includes warnings for common scheduling conflicts.";

    public ToolInputSchema InputSchema => new()
    {
        Type = "object",
        Properties = new Dictionary<string, PropertySchema>
        {
            ["character_name"] = new PropertySchema
            {
                Type = "string",
                Description = "Name of the character"
            },
            ["schedule_type"] = new PropertySchema
            {
                Type = "string",
                Description = "Optional: Filter by schedule type",
                Enum = new List<string> { "work", "exercise", "daily_routine", "weekly_commitment" }
            }
        },
        Required = new List<string> { "character_name" }
    };

    public async Task<object> ExecuteAsync(JsonElement? arguments)
    {
        if (!arguments.HasValue)
        {
            return new { success = false, error = "Missing arguments" };
        }

        var args = arguments.Value;
        var characterName = args.TryGetProperty("character_name", out var charProp) ? charProp.GetString() : null;
        var scheduleType = args.TryGetProperty("schedule_type", out var st) ? st.GetString() : null;

        if (string.IsNullOrEmpty(characterName))
        {
            return new { success = false, error = "Missing character_name" };
        }

        // Find character
        var character = await _characterRepo.FindByNameAsync(characterName);
        if (character == null)
        {
            var suggestions = await _characterRepo.SearchNamesAsync(characterName);
            return new
            {
                success = false,
                error = "Character not found",
                suggestions = suggestions.ToList()
            };
        }

        // Get schedules
        var schedules = await _scheduleRepo.GetByCharacterAsync(character.Id, scheduleType);
        var scheduleList = schedules.ToList();

        // Build schedule objects
        var scheduleObjects = scheduleList.Select(s => new
        {
            type = s.ScheduleType,
            name = s.ScheduleName,
            time = s.StartTime,
            end_time = s.EndTime,
            duration_minutes = s.DurationMinutes,
            days = s.DaysOfWeek,
            location = s.LocationDescription,
            description = s.Description,
            exceptions = s.Exceptions
        }).ToList();

        // Generate warnings based on schedule
        var warnings = new List<string>();

        // CRITICAL: Paul's 7 AM work start warning
        if (character.FullName == "Paul Rogala" || character.PreferredName == "Paul")
        {
            var earlyWorkSchedule = scheduleList.FirstOrDefault(s =>
                s.ScheduleType == "work" && s.StartTime != null && TimeSpan.Parse(s.StartTime) < TimeSpan.Parse("09:00"));

            if (earlyWorkSchedule != null)
            {
                warnings.Add($"⚠️ CRITICAL: Paul's work day starts at {earlyWorkSchedule.StartTime}, NOT 9:00 AM");
                warnings.Add("Extended breakfast or morning conversations are not compatible with Paul's schedule");
            }
        }

        // Kate's running schedule (non-negotiable)
        if (character.FullName.Contains("Morrison") && character.PreferredName == "Kate")
        {
            var runningSchedule = scheduleList.FirstOrDefault(s =>
                s.ScheduleType == "exercise" && s.ScheduleName.Contains("run", StringComparison.OrdinalIgnoreCase));

            if (runningSchedule != null)
            {
                warnings.Add("Kate's running schedule is NON-NEGOTIABLE - she does not skip runs for social events");
            }
        }

        return new
        {
            success = true,
            character = character.PreferredName ?? character.FullName,
            schedules = scheduleObjects,
            warnings = warnings
        };
    }
}
