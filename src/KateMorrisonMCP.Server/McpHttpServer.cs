using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using KateMorrisonMCP.Server.Models;

namespace KateMorrisonMCP.Server;

/// <summary>
/// HTTP/SSE transport for MCP server (for containerized deployments).
/// Authentication:
///   - /health: X-API-Key header (when MCP_API_KEY is set)
///   - /sse, /: Authorization: Bearer token (unless AUTH_DISABLED=true)
///   - /.well-known/oauth-protected-resource: always public
/// </summary>
public class McpHttpServer
{
    private readonly ILogger<McpHttpServer> _logger;
    private readonly McpRequestProcessor _processor;
    private readonly string? _apiKey;          // X-API-Key for /health
    private readonly string? _oauthToken;      // Bearer token for /sse and /
    private readonly string? _oauthResourceUrl;
    private readonly bool _authDisabled;
    private readonly int _port;

    private const string DefaultResourceUrl = "https://mcp-morrison.thresholdwater.com";
    private const string AuthorizationServer = "https://auth.thresholdwater.com";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public McpHttpServer(
        ILogger<McpHttpServer> logger,
        McpRequestProcessor processor,
        int port,
        string? apiKey = null,
        string? oauthToken = null,
        string? oauthResourceUrl = null,
        bool authDisabled = false)
    {
        _logger = logger;
        _processor = processor;
        _port = port;
        _apiKey = apiKey;
        _oauthToken = oauthToken;
        _oauthResourceUrl = oauthResourceUrl;
        _authDisabled = authDisabled;
    }

    private static bool SafeCompare(string a, string b)
    {
        var hashA = SHA256.HashData(Encoding.UTF8.GetBytes(a));
        var hashB = SHA256.HashData(Encoding.UTF8.GetBytes(b));
        return CryptographicOperations.FixedTimeEquals(hashA, hashB);
    }

    private string ResourceUrl => (_oauthResourceUrl ?? DefaultResourceUrl).TrimEnd('/');

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting MCP HTTP Server on port {Port}...", _port);

        if (_authDisabled)
        {
            _logger.LogWarning("Bearer token authentication is DISABLED (AUTH_DISABLED=true)");
        }
        else
        {
            _logger.LogInformation("Bearer token authentication enabled");
        }

        if (!string.IsNullOrEmpty(_apiKey))
        {
            _logger.LogInformation("X-API-Key authentication enabled for /health");
        }
        else
        {
            _logger.LogWarning("/health endpoint is unauthenticated (MCP_API_KEY not set)");
        }

        await _processor.InitializeAsync();

        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls($"http://0.0.0.0:{_port}");
        var app = builder.Build();

        // Auth middleware — runs before all endpoint handlers
        app.Use(async (context, next) =>
        {
            var path = context.Request.Path;

            // /.well-known is always public
            if (path.StartsWithSegments("/.well-known"))
            {
                await next();
                return;
            }

            // /health: X-API-Key when MCP_API_KEY is set
            if (path.StartsWithSegments("/health"))
            {
                if (!string.IsNullOrEmpty(_apiKey))
                {
                    if (!context.Request.Headers.TryGetValue("X-API-Key", out var key)
                        || !SafeCompare(key.ToString(), _apiKey))
                    {
                        _logger.LogWarning("Unauthorized /health access from {IP}", context.Connection.RemoteIpAddress);
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            error = "Unauthorized - Invalid or missing X-API-Key header"
                        });
                        return;
                    }
                }
                await next();
                return;
            }

            // All other endpoints: Bearer token
            if (!_authDisabled)
            {
                var authHeader = context.Request.Headers.Authorization.ToString();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    _logger.LogWarning("Unauthorized access attempt (missing Bearer) from {IP}", context.Connection.RemoteIpAddress);
                    context.Response.Headers["WWW-Authenticate"] =
                        $"Bearer realm=\"MCP\", resource_metadata=\"{ResourceUrl}/.well-known/oauth-protected-resource\"";
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new { error = "Unauthorized" });
                    return;
                }

                var token = authHeader[7..];
                if (!SafeCompare(token, _oauthToken ?? string.Empty))
                {
                    _logger.LogWarning("Unauthorized access attempt (invalid token) from {IP}", context.Connection.RemoteIpAddress);
                    context.Response.Headers["WWW-Authenticate"] =
                        $"Bearer realm=\"MCP\", resource_metadata=\"{ResourceUrl}/.well-known/oauth-protected-resource\"";
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new { error = "Unauthorized" });
                    return;
                }
            }

            await next();
        });

        // Public: OAuth protected resource metadata
        app.MapGet("/.well-known/oauth-protected-resource", () => new
        {
            resource = ResourceUrl,
            authorization_servers = new[] { AuthorizationServer }
        });

        // Protected: MCP SSE endpoint
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

                if (request.Id == null)
                {
                    await _processor.ProcessNotificationAsync(request);
                    context.Response.StatusCode = 204;
                    return;
                }

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

        // X-API-Key protected: health check
        app.MapGet("/health", () => new
        {
            status = "healthy",
            server = "kate-morrison-canonical-facts",
            version = "1.0.0",
            transport = "http/sse",
            healthAuth = !string.IsNullOrEmpty(_apiKey) ? "X-API-Key required" : "unauthenticated",
            mcpAuth = _authDisabled ? "disabled" : "Bearer token required"
        });

        // Protected: server info
        app.MapGet("/", () => new
        {
            server = "Kate Morrison MCP Server",
            version = "1.0.0",
            transport = "HTTP/SSE",
            endpoints = new
            {
                wellKnown = "/.well-known/oauth-protected-resource (GET) - OAuth metadata",
                sse = "/sse (POST) - Send MCP requests here",
                health = "/health (GET) - Health check"
            },
            authentication = new
            {
                mcp = _authDisabled ? "disabled" : "Authorization: Bearer <token> required",
                health = !string.IsNullOrEmpty(_apiKey) ? "X-API-Key header required" : "unauthenticated"
            },
            documentation = "https://modelcontextprotocol.io"
        });

        _logger.LogInformation("MCP HTTP Server started successfully");
        _logger.LogInformation("SSE endpoint: http://0.0.0.0:{Port}/sse", _port);
        _logger.LogInformation("Health check: http://0.0.0.0:{Port}/health", _port);
        _logger.LogInformation("OAuth metadata: http://0.0.0.0:{Port}/.well-known/oauth-protected-resource", _port);

        cancellationToken.Register(() =>
        {
            _logger.LogInformation("Shutting down MCP HTTP Server...");
            app.StopAsync().Wait();
        });

        await app.RunAsync();
    }
}
