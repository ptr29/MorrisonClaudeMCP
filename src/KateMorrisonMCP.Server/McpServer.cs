using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using KateMorrisonMCP.Server.Models;

namespace KateMorrisonMCP.Server;

/// <summary>
/// MCP protocol 2024-11-05 server implementation
/// Handles stdio communication with Claude Desktop (local mode)
/// </summary>
public class McpServer
{
    private readonly ILogger<McpServer> _logger;
    private readonly McpRequestProcessor _processor;

    // JSON serialization options for MCP protocol (camelCase required)
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    public McpServer(
        ILogger<McpServer> logger,
        McpRequestProcessor processor)
    {
        _logger = logger;
        _processor = processor;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting MCP Canonical Facts Server (stdio mode)...");

        // Initialize the processor
        await _processor.InitializeAsync();

        // Start listening for MCP requests via stdio
        await ListenForRequestsAsync(cancellationToken);
    }

    private async Task ListenForRequestsAsync(CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(Console.OpenStandardInput());
        using var writer = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };

        while (!cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (line == null) break;

            try
            {
                _logger.LogDebug("Received: {Line}", line);

                var request = JsonSerializer.Deserialize<McpRequest>(line, JsonOptions);
                if (request == null)
                {
                    _logger.LogWarning("Failed to deserialize request");
                    continue;
                }

                // Check if this is a notification (no id field)
                // Notifications must not receive a response per JSON-RPC 2.0 spec
                var isNotification = request.Id == null;

                if (isNotification)
                {
                    _logger.LogDebug("Received notification: {Method}", request.Method);
                    // Process notification but don't send response
                    await _processor.ProcessNotificationAsync(request);
                    continue;
                }

                // Regular request - process and send response
                var response = await _processor.ProcessRequestAsync(request);
                var responseJson = JsonSerializer.Serialize(response, JsonOptions);

                _logger.LogDebug("Sending: {Response}", responseJson);
                await writer.WriteLineAsync(responseJson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request");
                // Only send error response if we can't determine if it was a notification
                // In case of parse errors, we send a generic error with null id
                var errorResponse = new McpResponse
                {
                    Error = new { code = -32700, message = $"Parse error: {ex.Message}" },
                    Id = null
                };
                await writer.WriteLineAsync(JsonSerializer.Serialize(errorResponse, JsonOptions));
            }
        }
    }

}
