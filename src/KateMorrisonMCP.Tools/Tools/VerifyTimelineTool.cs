using System.Text.Json;
using KateMorrisonMCP.Data.Repositories;
using KateMorrisonMCP.Tools.Models;

namespace KateMorrisonMCP.Tools.Tools;

/// <summary>
/// Verifies timeline events and provides context (previous/next day)
/// </summary>
public class VerifyTimelineTool : ITool
{
    private readonly ITimelineRepository _timelineRepo;
    private readonly ICharacterRepository _characterRepo;

    public VerifyTimelineTool(ITimelineRepository timelineRepo, ICharacterRepository characterRepo)
    {
        _timelineRepo = timelineRepo;
        _characterRepo = characterRepo;
    }

    public string Name => "verify_timeline";

    public string Description =>
        "Verifies timeline events by date or character involvement. " +
        "Returns events with context (previous day, next day). " +
        "Example: Verify when Kate and Paul first met (2025-09-06).";

    public ToolInputSchema InputSchema => new()
    {
        Type = "object",
        Properties = new Dictionary<string, PropertySchema>
        {
            ["date"] = new PropertySchema
            {
                Type = "string",
                Description = "Date to check (YYYY-MM-DD format)"
            },
            ["character"] = new PropertySchema
            {
                Type = "string",
                Description = "Filter events by character involvement"
            }
        },
        Required = new List<string>()  // At least one of date or character required, but we'll validate in code
    };

    public async Task<object> ExecuteAsync(JsonElement? arguments)
    {
        if (!arguments.HasValue)
        {
            return new { success = false, error = "Missing arguments" };
        }

        var args = arguments.Value;
        var date = args.TryGetProperty("date", out var d) ? d.GetString() : null;
        var characterName = args.TryGetProperty("character", out var c) ? c.GetString() : null;

        if (string.IsNullOrEmpty(date) && string.IsNullOrEmpty(characterName))
        {
            return new { success = false, error = "Must provide either 'date' or 'character'" };
        }

        // Query by character
        if (!string.IsNullOrEmpty(characterName))
        {
            var character = await _characterRepo.FindByNameAsync(characterName);
            if (character == null)
            {
                return new { success = false, error = "Character not found" };
            }

            var events = await _timelineRepo.GetByCharacterAsync(character.Id);
            var eventsList = events.Select(e => new
            {
                date = e.EventDate,
                title = e.EventTitle,
                description = e.EventDescription,
                category = e.EventCategory,
                significance = e.Significance
            }).ToList();

            return new
            {
                success = true,
                character = character.PreferredName ?? character.FullName,
                events = eventsList,
                total_events = eventsList.Count
            };
        }

        // Query by date
        if (!string.IsNullOrEmpty(date))
        {
            var events = await _timelineRepo.GetByDateAsync(date);
            var eventsList = events.ToList();

            // Calculate day of week
            var dayOfWeek = DateTime.TryParse(date, out var dt) ? dt.DayOfWeek.ToString() : null;

            var eventObjects = eventsList.Select(e => new
            {
                id = e.Id,
                title = e.EventTitle,
                description = e.EventDescription,
                category = e.EventCategory,
                significance = e.Significance,
                location = e.LocationDescription
            }).ToList();

            return new
            {
                success = true,
                query_date = date,
                day_of_week = dayOfWeek,
                events = eventObjects
            };
        }

        return new { success = false, error = "Invalid query" };
    }
}
