namespace KateMorrisonMCP.Tools;

/// <summary>
/// Central registry for all MCP tools
/// </summary>
public class ToolRegistry
{
    private readonly Dictionary<string, ITool> _tools = new();

    public void RegisterTool(ITool tool)
    {
        _tools[tool.Name] = tool;
    }

    public ITool? GetTool(string name)
    {
        return _tools.GetValueOrDefault(name);
    }

    public IEnumerable<ITool> GetAllTools()
    {
        return _tools.Values;
    }
}
