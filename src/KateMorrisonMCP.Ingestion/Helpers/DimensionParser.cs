using System.Text.RegularExpressions;

namespace KateMorrisonMCP.Ingestion.Helpers;

/// <summary>
/// Parses dimension strings like "14'2\" × 10'11\"" into width and length in feet (decimal)
/// </summary>
public static partial class DimensionParser
{
    // Matches: 14'2" × 10'11" or 14 feet 2 inches x 10 feet 11 inches
    [GeneratedRegex(@"(\d+)'?\s*(\d+)?""?\s*[×x]\s*(\d+)'?\s*(\d+)?""?", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex DimensionRegex();

    /// <summary>
    /// Parses dimension string into (width, length) in decimal feet
    /// Example: "14'2\" × 10'11\"" → (14.17, 10.92)
    /// </summary>
    public static (decimal? width, decimal? length) Parse(string? dimensions)
    {
        if (string.IsNullOrWhiteSpace(dimensions))
        {
            return (null, null);
        }

        var match = DimensionRegex().Match(dimensions);
        if (!match.Success)
        {
            return (null, null);
        }

        // Parse width
        if (!int.TryParse(match.Groups[1].Value, out var widthFeet))
        {
            return (null, null);
        }

        var widthInches = 0;
        if (match.Groups[2].Success && int.TryParse(match.Groups[2].Value, out var wi))
        {
            widthInches = wi;
        }

        // Parse length
        if (!int.TryParse(match.Groups[3].Value, out var lengthFeet))
        {
            return (null, null);
        }

        var lengthInches = 0;
        if (match.Groups[4].Success && int.TryParse(match.Groups[4].Value, out var li))
        {
            lengthInches = li;
        }

        // Convert to decimal feet
        var width = widthFeet + (widthInches / 12.0m);
        var length = lengthFeet + (lengthInches / 12.0m);

        return (width, length);
    }

    /// <summary>
    /// Formats dimensions back to feet'inches" format
    /// </summary>
    public static string Format(decimal widthFeet, decimal lengthFeet)
    {
        var widthFeetInt = (int)widthFeet;
        var widthInches = (int)Math.Round((widthFeet - widthFeetInt) * 12);

        var lengthFeetInt = (int)lengthFeet;
        var lengthInches = (int)Math.Round((lengthFeet - lengthFeetInt) * 12);

        return $"{widthFeetInt}'{widthInches}\" × {lengthFeetInt}'{lengthInches}\"";
    }
}
