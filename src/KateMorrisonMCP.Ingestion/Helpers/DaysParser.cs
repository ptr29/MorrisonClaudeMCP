namespace KateMorrisonMCP.Ingestion.Helpers;

/// <summary>
/// Parses day specifications into JSON arrays
/// Supports: weekdays, weekends, daily, specific days
/// </summary>
public static class DaysParser
{
    private static readonly string[] Weekdays = ["monday", "tuesday", "wednesday", "thursday", "friday"];
    private static readonly string[] Weekend = ["saturday", "sunday"];
    private static readonly string[] AllDays = ["monday", "tuesday", "wednesday", "thursday", "friday", "saturday", "sunday"];

    /// <summary>
    /// Parses days specification into JSON array string
    /// Examples:
    ///   "weekdays" → ["monday","tuesday","wednesday","thursday","friday"]
    ///   "weekends" → ["saturday","sunday"]
    ///   "daily" → ["monday","tuesday","wednesday","thursday","friday","saturday","sunday"]
    ///   "monday, wednesday, friday" → ["monday","wednesday","friday"]
    /// </summary>
    public static string Parse(string? days)
    {
        if (string.IsNullOrWhiteSpace(days))
        {
            return "[]";
        }

        var normalized = days.Trim().ToLowerInvariant();

        // Handle special keywords
        if (normalized == "weekdays")
        {
            return System.Text.Json.JsonSerializer.Serialize(Weekdays);
        }

        if (normalized is "weekends" or "weekend")
        {
            return System.Text.Json.JsonSerializer.Serialize(Weekend);
        }

        if (normalized is "daily" or "every day" or "all days")
        {
            return System.Text.Json.JsonSerializer.Serialize(AllDays);
        }

        // Parse comma-separated days
        var daysList = normalized
            .Split(',')
            .Select(d => d.Trim())
            .Where(d => !string.IsNullOrEmpty(d))
            .Where(d => AllDays.Contains(d)) // Only valid day names
            .Distinct()
            .ToList();

        return System.Text.Json.JsonSerializer.Serialize(daysList);
    }

    /// <summary>
    /// Validates that a day specification is valid
    /// </summary>
    public static bool IsValid(string? days)
    {
        if (string.IsNullOrWhiteSpace(days))
        {
            return false;
        }

        var normalized = days.Trim().ToLowerInvariant();

        // Check special keywords
        if (normalized is "weekdays" or "weekends" or "weekend" or "daily" or "every day" or "all days")
        {
            return true;
        }

        // Check comma-separated days
        var daysList = normalized
            .Split(',')
            .Select(d => d.Trim())
            .Where(d => !string.IsNullOrEmpty(d))
            .ToList();

        return daysList.Count > 0 && daysList.All(d => AllDays.Contains(d));
    }
}
