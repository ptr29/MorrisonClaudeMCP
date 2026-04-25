using System.Text.Json;
using KateMorrisonMCP.Tools.Models;

namespace KateMorrisonMCP.Tools;

/// <summary>
/// Base interface for all MCP tools
/// </summary>
public interface ITool
{
    string Name { get; }
    string Description { get; }
    ToolInputSchema InputSchema { get; }
    Task<object> ExecuteAsync(JsonElement? arguments);
}
