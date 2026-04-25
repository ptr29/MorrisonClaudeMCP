using System.Text.RegularExpressions;

namespace KateMorrisonMCP.Ingestion.Parsing;

/// <summary>
/// Parses canonical tags from markdown files
/// Supports both block format and inline format
/// </summary>
public partial class TagParser
{
    // Block format: <!-- canonical: character\nname: Kate\nage: 29\n-->
    [GeneratedRegex(@"<!--\s*canonical:\s*(\w+)\s*\n([\s\S]*?)-->", RegexOptions.Compiled | RegexOptions.Multiline)]
    private static partial Regex BlockTagRegex();

    // Inline format: <!-- canonical: character | name: Kate | age: 29 -->
    [GeneratedRegex(@"<!--\s*canonical:\s*(\w+)\s*\|(.*?)-->", RegexOptions.Compiled)]
    private static partial Regex InlineTagRegex();

    /// <summary>
    /// Parses all canonical tags from a markdown file
    /// </summary>
    public IEnumerable<CanonicalTag> ParseFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        var content = File.ReadAllText(filePath);
        var lines = File.ReadAllLines(filePath);

        // Parse block format tags
        foreach (var match in BlockTagRegex().Matches(content).Cast<Match>())
        {
            var lineNumber = GetLineNumber(content, match.Index, lines);
            var tag = ParseBlockTag(match, filePath, lineNumber);
            if (tag != null)
            {
                yield return tag;
            }
        }

        // Parse inline format tags
        foreach (var match in InlineTagRegex().Matches(content).Cast<Match>())
        {
            var lineNumber = GetLineNumber(content, match.Index, lines);
            var tag = ParseInlineTag(match, filePath, lineNumber);
            if (tag != null)
            {
                yield return tag;
            }
        }
    }

    /// <summary>
    /// Parses a block format tag
    /// </summary>
    private CanonicalTag? ParseBlockTag(Match match, string filePath, int lineNumber)
    {
        try
        {
            var tagType = match.Groups[1].Value.Trim().ToLowerInvariant();
            var fieldsBlock = match.Groups[2].Value;

            var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Parse field lines (field: value)
            foreach (var line in fieldsBlock.Split('\n'))
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed))
                {
                    continue;
                }

                var colonIndex = trimmed.IndexOf(':');
                if (colonIndex > 0)
                {
                    var fieldName = trimmed[..colonIndex].Trim();
                    var fieldValue = trimmed[(colonIndex + 1)..].Trim();

                    if (!string.IsNullOrEmpty(fieldName))
                    {
                        fields[fieldName] = fieldValue;
                    }
                }
            }

            return new CanonicalTag
            {
                Type = tagType,
                SourceFile = filePath,
                LineNumber = lineNumber,
                Fields = fields
            };
        }
        catch
        {
            // Skip malformed tags
            return null;
        }
    }

    /// <summary>
    /// Parses an inline format tag
    /// </summary>
    private CanonicalTag? ParseInlineTag(Match match, string filePath, int lineNumber)
    {
        try
        {
            var tagType = match.Groups[1].Value.Trim().ToLowerInvariant();
            var fieldsText = match.Groups[2].Value;

            var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Parse pipe-separated fields (field: value | field: value)
            foreach (var fieldPart in fieldsText.Split('|'))
            {
                var trimmed = fieldPart.Trim();
                if (string.IsNullOrEmpty(trimmed))
                {
                    continue;
                }

                var colonIndex = trimmed.IndexOf(':');
                if (colonIndex > 0)
                {
                    var fieldName = trimmed[..colonIndex].Trim();
                    var fieldValue = trimmed[(colonIndex + 1)..].Trim();

                    if (!string.IsNullOrEmpty(fieldName))
                    {
                        fields[fieldName] = fieldValue;
                    }
                }
            }

            return new CanonicalTag
            {
                Type = tagType,
                SourceFile = filePath,
                LineNumber = lineNumber,
                Fields = fields
            };
        }
        catch
        {
            // Skip malformed tags
            return null;
        }
    }

    /// <summary>
    /// Calculates line number from character index
    /// </summary>
    private int GetLineNumber(string content, int charIndex, string[] lines)
    {
        var currentIndex = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            currentIndex += lines[i].Length + 1; // +1 for newline
            if (currentIndex > charIndex)
            {
                return i + 1; // Line numbers are 1-based
            }
        }
        return lines.Length;
    }
}
