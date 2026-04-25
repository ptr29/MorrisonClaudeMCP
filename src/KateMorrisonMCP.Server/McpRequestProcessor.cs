using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using KateMorrisonMCP.Server.Models;
using KateMorrisonMCP.Tools;
using KateMorrisonMCP.Data;

namespace KateMorrisonMCP.Server;

/// <summary>
/// Core MCP request processing logic shared between stdio and HTTP transports
/// </summary>
public class McpRequestProcessor
{
    private readonly ILogger<McpRequestProcessor> _logger;
    private readonly ToolRegistry _toolRegistry;
    private readonly DatabaseContext _dbContext;

    // JSON serialization options for MCP protocol (camelCase required)
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    public McpRequestProcessor(
        ILogger<McpRequestProcessor> logger,
        ToolRegistry toolRegistry,
        DatabaseContext dbContext)
    {
        _logger = logger;
        _toolRegistry = toolRegistry;
        _dbContext = dbContext;
    }

    public async Task InitializeAsync()
    {
        _logger.LogInformation("Initializing MCP request processor...");

        // Initialize database connection
        await _dbContext.InitializeAsync();

        // Run health check
        var healthCheck = await _dbContext.PerformHealthCheckAsync();
        if (!healthCheck.IsHealthy)
        {
            _logger.LogError("Health check failed: {Message}", healthCheck.Message);
            throw new Exception($"Health check failed: {healthCheck.Message}");
        }

        _logger.LogInformation("Health check passed:\n{Message}", healthCheck.Message);
    }

    public async Task ProcessNotificationAsync(McpRequest notification)
    {
        // Handle known notifications
        switch (notification.Method)
        {
            case "notifications/initialized":
                _logger.LogInformation("Client initialized");
                break;
            case "notifications/cancelled":
                _logger.LogInformation("Request cancelled");
                break;
            default:
                _logger.LogWarning("Unknown notification: {Method}", notification.Method);
                break;
        }

        await Task.CompletedTask;
    }

    public async Task<McpResponse> ProcessRequestAsync(McpRequest request)
    {
        return request.Method switch
        {
            "initialize" => HandleInitialize(request),
            "tools/list" => HandleListTools(request),
            "tools/call" => await HandleToolCallAsync(request),
            _ => new McpResponse
            {
                Error = new { code = -32601, message = $"Method not found: {request.Method}" },
                Id = request.Id
            }
        };
    }

    private McpResponse HandleInitialize(McpRequest request)
    {
        return new McpResponse
        {
            Result = new
            {
                protocolVersion = "2024-11-05",
                capabilities = new
                {
                    tools = new { }
                },
                serverInfo = new
                {
                    name = "kate-morrison-canonical-facts",
                    version = "1.0.0"
                }
            },
            Id = request.Id
        };
    }

    private McpResponse HandleListTools(McpRequest request)
    {
        var tools = _toolRegistry.GetAllTools().Select(t => new
        {
            name = t.Name,
            description = t.Description,
            inputSchema = new
            {
                type = t.InputSchema.Type,
                properties = t.InputSchema.Properties.ToDictionary(
                    kvp => kvp.Key,
                    kvp => new
                    {
                        type = kvp.Value.Type,
                        description = kvp.Value.Description,
                        @enum = kvp.Value.Enum
                    }
                ),
                required = t.InputSchema.Required
            }
        }).ToList();

        return new McpResponse
        {
            Result = new { tools },
            Id = request.Id
        };
    }

    private async Task<McpResponse> HandleToolCallAsync(McpRequest request)
    {
        if (request.Params == null)
        {
            return new McpResponse
            {
                Error = new { code = -32602, message = "Invalid params" },
                Id = request.Id
            };
        }

        var toolName = request.Params.Value.GetProperty("name").GetString();
        var arguments = request.Params.Value.TryGetProperty("arguments", out var args)
            ? args
            : (JsonElement?)null;

        if (string.IsNullOrEmpty(toolName))
        {
            return new McpResponse
            {
                Error = new { code = -32602, message = "Tool name is required" },
                Id = request.Id
            };
        }

        var tool = _toolRegistry.GetTool(toolName);
        if (tool == null)
        {
            return new McpResponse
            {
                Error = new { code = -32602, message = $"Tool not found: {toolName}" },
                Id = request.Id
            };
        }

        try
        {
            var result = await tool.ExecuteAsync(arguments);
            return new McpResponse
            {
                Result = new
                {
                    content = new[]
                    {
                        new
                        {
                            type = "text",
                            text = JsonSerializer.Serialize(result, new JsonSerializerOptions
                            {
                                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                WriteIndented = true
                            })
                        }
                    }
                },
                Id = request.Id
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Tool execution failed: {ToolName}", toolName);
            return new McpResponse
            {
                Error = new { code = -32603, message = $"Tool execution failed: {ex.Message}" },
                Id = request.Id
            };
        }
    }
}
