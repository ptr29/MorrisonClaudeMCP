using System.Text.RegularExpressions;

namespace KateMorrisonMCP.Ingestion.Helpers;

/// <summary>
/// Parses height strings like "5'6\"" into total inches
/// </summary>
public static partial class HeightParser
{
    // Matches patterns like: 5'6", 5'6, 5 feet 6 inches, 5ft 6in
    [GeneratedRegex(@"(\d+)'?\s*(?:feet|ft)?\s*(\d+)?""?\s*(?:inches|in)?", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex HeightRegex();

    /// <summary>
    /// Parses height string to total inches
    /// Examples: "5'6\"" → 66, "6'2" → 74, "5 feet 10 inches" → 70
    /// </summary>
    public static int? Parse(string? height)
    {
        if (string.IsNullOrWhiteSpace(height))
        {
            return null;
        }

        var match = HeightRegex().Match(height);
        if (!match.Success)
        {
            return null;
        }

        if (!int.TryParse(match.Groups[1].Value, out var feet))
        {
            return null;
        }

        var inches = 0;
        if (match.Groups[2].Success)
        {
            if (!int.TryParse(match.Groups[2].Value, out inches))
            {
                return null;
            }
        }

        return (feet * 12) + inches;
    }

    /// <summary>
    /// Formats height in inches back to feet'inches" format
    /// </summary>
    public static string Format(int totalInches)
    {
        var feet = totalInches / 12;
        var inches = totalInches % 12;
        return $"{feet}'{inches}\"";
    }
}
