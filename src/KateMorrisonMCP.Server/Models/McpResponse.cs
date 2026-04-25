using System.Text.Json.Serialization;

namespace KateMorrisonMCP.Server.Models;

public class McpResponse
{
    public string Jsonrpc { get; set; } = "2.0";

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Result { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Error { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public object? Id { get; set; }
}
