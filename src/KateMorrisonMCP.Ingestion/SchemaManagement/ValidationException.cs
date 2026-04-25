namespace KateMorrisonMCP.Ingestion.SchemaManagement;

/// <summary>
/// Exception thrown when validation fails during ingestion
/// </summary>
public class ValidationException : Exception
{
    public string? FilePath { get; }
    public int? LineNumber { get; }
    public string? TagType { get; }

    public ValidationException(string message) : base(message)
    {
    }

    public ValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public ValidationException(string message, string? filePath, int? lineNumber, string? tagType)
        : base(message)
    {
        FilePath = filePath;
        LineNumber = lineNumber;
        TagType = tagType;
    }

    public override string ToString()
    {
        var location = FilePath != null
            ? $"{FilePath}:{LineNumber}"
            : "unknown location";
        var tagInfo = TagType != null ? $" [{TagType}]" : "";
        return $"{location}{tagInfo}: {Message}";
    }
}
