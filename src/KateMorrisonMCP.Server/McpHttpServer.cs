using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using KateMorrisonMCP.Server.Models;

namespace KateMorrisonMCP.Server;

/// <summary>
/// HTTP/SSE transport for MCP server (for containerized deployments)
/// Requires X-API-Key authentication when API key is configured
/// </summary>
public class McpHttpServer
{
    private readonly ILogger<McpHttpServer> _logger;
    private readonly McpRequestProcessor _processor;
    private readonly string? _apiKey;
    private readonly int _port;

    // JSON options for MCP protocol
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public McpHttpServer(
        ILogger<McpHttpServer> logger,
        McpRequestProcessor processor,
        int port,
        string? apiKey = null)
    {
        _logger = logger;
        _processor = processor;
        _port = port;
        _apiKey = apiKey;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting MCP HTTP Server on port {Port}...", _port);

        if (!string.IsNullOrEmpty(_apiKey))
        {
            _logger.LogInformation("API Key authentication enabled");
        }
        else
        {
            _logger.LogWarning("API Key authentication is DISABLED - server is unauthenticated!");
        }

        // Initialize the request processor
        await _processor.InitializeAsync();

        // Build minimal ASP.NET Core app
        var builder = WebApplication.CreateBuilder();

        // Configure to listen on specified port
        builder.WebHost.UseUrls($"http://0.0.0.0:{_port}");

        var app = builder.Build();

        // Add authentication middleware
        if (!string.IsNullOrEmpty(_apiKey))
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/sse"))
                {
                    if (!context.Request.Headers.TryGetValue("X-API-Key", out var providedKey) ||
                        providedKey != _apiKey)
                    {
                        _logger.LogWarning("Unauthorized access attempt from {IP}", context.Connection.RemoteIpAddress);
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            error = "Unauthorized - Invalid or missing X-API-Key header"
                        });
                        return;
                    }
                }
                await next();
            });
        }

        // SSE endpoint for MCP protocol
        app.MapPost("/sse", async (HttpContext context) =>
        {
            try
            {
                var request = await context.Request.ReadFromJsonAsync<McpRequest>(JsonOptions);
                if (request == null)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsJsonAsync(new { error = "Invalid request" });
                    return;
                }

                _logger.LogDebug("Received request: {Method}", request.Method);

                // Check if this is a notification (no id field)
                if (request.Id == null)
                {
                    await _processor.ProcessNotificationAsync(request);
                    context.Response.StatusCode = 204; // No content for notifications
                    return;
                }

                // Process request and return response
                var response = await _processor.ProcessRequestAsync(request);
                await context.Response.WriteAsJsonAsync(response, JsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing HTTP request");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new McpResponse
                {
                    Error = new { code = -32603, message = $"Internal error: {ex.Message}" },
                    Id = null
                });
            }
        });

        // Health check endpoint (no auth required)
        app.MapGet("/health", () => new
        {
            status = "healthy",
            server = "kate-morrison-canonical-facts",
            version = "1.0.0",
            transport = "http/sse",
            authEnabled = !string.IsNullOrEmpty(_apiKey)
        });

        // Root endpoint with usage info
        app.MapGet("/", () => new
        {
            server = "Kate Morrison MCP Server",
            version = "1.0.0",
            transport = "HTTP/SSE",
            endpoints = new
            {
                sse = "/sse (POST) - Send MCP requests here",
                health = "/health (GET) - Health check"
            },
            authentication = !string.IsNullOrEmpty(_apiKey)
                ? "X-API-Key header required"
                : "No authentication required",
            documentation = "https://modelcontextprotocol.io"
        });

        _logger.LogInformation("MCP HTTP Server started successfully");
        _logger.LogInformation("SSE endpoint: http://0.0.0.0:{Port}/sse", _port);
        _logger.LogInformation("Health check: http://0.0.0.0:{Port}/health", _port);

        // Register cancellation token to stop the server
        cancellationToken.Register(() =>
        {
            _logger.LogInformation("Shutting down MCP HTTP Server...");
            app.StopAsync().Wait();
        });

        await app.RunAsync();
    }
}
