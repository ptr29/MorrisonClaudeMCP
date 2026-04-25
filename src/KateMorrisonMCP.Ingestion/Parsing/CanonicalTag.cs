namespace KateMorrisonMCP.Ingestion.Parsing;

/// <summary>
/// Represents a parsed canonical tag from a markdown file
/// </summary>
public class CanonicalTag
{
    /// <summary>
    /// Tag type (character, location, schedule, etc.)
    /// </summary>
    public required string Type { get; init; }

    /// <summary>
    /// Source file path
    /// </summary>
    public required string SourceFile { get; init; }

    /// <summary>
    /// Line number in source file where tag starts
    /// </summary>
    public int LineNumber { get; init; }

    /// <summary>
    /// Field name-value pairs from the tag
    /// </summary>
    public Dictionary<string, string> Fields { get; init; } = new();

    /// <summary>
    /// Gets a required field value, throws if missing
    /// </summary>
    public string GetRequired(string fieldName)
    {
        if (!Fields.TryGetValue(fieldName, out var value) || string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"Required field '{fieldName}' is missing or empty");
        }
        return value;
    }

    /// <summary>
    /// Gets an optional field value, returns null if missing
    /// </summary>
    public string? GetOptional(string fieldName)
    {
        return Fields.TryGetValue(fieldName, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : null;
    }

    /// <summary>
    /// Checks if a field exists and has a non-empty value
    /// </summary>
    public bool HasField(string fieldName)
    {
        return Fields.TryGetValue(fieldName, out var value) && !string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// Gets an optional integer field value, returns null if missing or invalid
    /// </summary>
    public int? GetOptionalInt(string fieldName)
    {
        var value = GetOptional(fieldName);
        return value != null && int.TryParse(value, out var result) ? result : (int?)null;
    }

    public override string ToString()
    {
        var fieldCount = Fields.Count;
        return $"{Type} tag at {SourceFile}:{LineNumber} ({fieldCount} fields)";
    }
}
