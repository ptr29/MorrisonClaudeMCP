using System.Text.Json;

namespace KateMorrisonMCP.Server.Models;

public class McpRequest
{
    public string Jsonrpc { get; set; } = "2.0";
    public string Method { get; set; } = string.Empty;
    public JsonElement? Params { get; set; }
    public object? Id { get; set; }
}
