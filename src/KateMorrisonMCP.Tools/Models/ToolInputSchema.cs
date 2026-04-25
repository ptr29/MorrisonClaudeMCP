namespace KateMorrisonMCP.Tools.Models;

/// <summary>
/// JSON Schema for tool input parameters
/// </summary>
public class ToolInputSchema
{
    public string Type { get; set; } = "object";
    public Dictionary<string, PropertySchema> Properties { get; set; } = new();
    public List<string> Required { get; set; } = new();
}

public class PropertySchema
{
    public string Type { get; set; } = "string";
    public string? Description { get; set; }
    public List<string>? Enum { get; set; }
}
