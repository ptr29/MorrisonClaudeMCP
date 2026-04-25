using System.Text.Json;

namespace KateMorrisonMCP.Ingestion.Helpers;

/// <summary>
/// Converts comma-delimited strings to JSON arrays
/// </summary>
public static class JsonArrayHelper
{
    /// <summary>
    /// Converts comma-delimited string to JSON array
    /// Example: "red eyes, tall, athletic" → ["red eyes","tall","athletic"]
    /// Handles both comma and semicolon separators
    /// </summary>
    public static string ToJsonArray(string? input, char[]? separators = null)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return "[]";
        }

        separators ??= [',', ';'];

        var items = input
            .Split(separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim())
            .Distinct()
            .ToList();

        return JsonSerializer.Serialize(items);
    }

    /// <summary>
    /// Converts JSON array back to comma-delimited string
    /// </summary>
    public static string FromJsonArray(string? jsonArray)
    {
        if (string.IsNullOrWhiteSpace(jsonArray) || jsonArray == "[]")
        {
            return string.Empty;
        }

        try
        {
            var items = JsonSerializer.Deserialize<List<string>>(jsonArray);
            return items != null ? string.Join(", ", items) : string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Validates that a string is valid JSON array format
    /// </summary>
    public static bool IsValidJsonArray(string? jsonArray)
    {
        if (string.IsNullOrWhiteSpace(jsonArray))
        {
            return false;
        }

        try
        {
            var items = JsonSerializer.Deserialize<List<string>>(jsonArray);
            return items != null;
        }
        catch
        {
            return false;
        }
    }
}
