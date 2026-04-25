using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using KateMorrisonMCP.Server;
using KateMorrisonMCP.Data;
using KateMorrisonMCP.Data.Repositories;
using KateMorrisonMCP.Tools;
using KateMorrisonMCP.Tools.Tools;

// Build configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

// Get database path from config or environment
var databasePath = Environment.GetEnvironmentVariable("DATABASE_PATH")
    ?? configuration["Database:Path"]
    ?? "./data/canonical_facts.db";

// Expand to absolute path
databasePath = Path.GetFullPath(databasePath);

// Check for HTTP/SSE mode configuration
var mcpPort = Environment.GetEnvironmentVariable("MCP_PORT");
var mcpApiKey = Environment.GetEnvironmentVariable("MCP_API_KEY");
var oauthToken = Environment.GetEnvironmentVariable("OAUTH_TOKEN");
var oauthResourceUrl = Environment.GetEnvironmentVariable("OAUTH_RESOURCE_URL");
var authDisabled = Environment.GetEnvironmentVariable("AUTH_DISABLED") == "true";
var useHttpMode = !string.IsNullOrEmpty(mcpPort);

// Fail fast if OAuth token is not configured in HTTP mode
if (useHttpMode && !authDisabled && string.IsNullOrWhiteSpace(oauthToken))
{
    Console.Error.WriteLine(
        "[ERROR] OAUTH_TOKEN must be set when auth is enabled. " +
        "Set AUTH_DISABLED=true to disable authentication.");
    return 1;
}

// Setup dependency injection
var services = new ServiceCollection();

// Logging - CRITICAL: MCP requires stdout for JSON-RPC only, so logs MUST go to stderr
services.AddLogging(builder =>
{
    builder.AddConsole(options =>
    {
        // Force all console logging to stderr (MCP protocol requirement)
        options.LogToStandardErrorThreshold = LogLevel.Trace;
    });
    builder.SetMinimumLevel(LogLevel.Information);
});

// Database
services.AddSingleton(new DatabaseContext(databasePath));

// Repositories
services.AddSingleton<ICharacterRepository, CharacterRepository>();
services.AddSingleton<INegativeRepository, NegativeRepository>();
services.AddSingleton<IScheduleRepository, ScheduleRepository>();
services.AddSingleton<ILocationRepository, LocationRepository>();
services.AddSingleton<ITimelineRepository, TimelineRepository>();
services.AddSingleton<IRelationshipRepository, RelationshipRepository>();
services.AddSingleton<IPossessionRepository, PossessionRepository>();

// Tool Registry
services.AddSingleton(sp =>
{
    var registry = new ToolRegistry();

    // Register 6 MVP tools in priority order
    registry.RegisterTool(new CheckNegativeTool(
        sp.GetRequiredService<INegativeRepository>(),
        sp.GetRequiredService<ICharacterRepository>()));

    registry.RegisterTool(new GetScheduleTool(
        sp.GetRequiredService<IScheduleRepository>(),
        sp.GetRequiredService<ICharacterRepository>()));

    registry.RegisterTool(new GetCharacterFactsTool(
        sp.GetRequiredService<ICharacterRepository>()));

    registry.RegisterTool(new GetLocationLayoutTool(
        sp.GetRequiredService<ILocationRepository>()));

    registry.RegisterTool(new VerifyTimelineTool(
        sp.GetRequiredService<ITimelineRepository>(),
        sp.GetRequiredService<ICharacterRepository>()));

    registry.RegisterTool(new CheckRelationshipTool(
        sp.GetRequiredService<IRelationshipRepository>(),
        sp.GetRequiredService<ICharacterRepository>(),
        sp.GetRequiredService<ITimelineRepository>()));

    return registry;
});

// MCP Request Processor (shared between stdio and HTTP modes)
services.AddSingleton<McpRequestProcessor>();

// MCP Server (stdio or HTTP based on configuration)
if (useHttpMode)
{
    services.AddSingleton(sp => new McpHttpServer(
        sp.GetRequiredService<ILogger<McpHttpServer>>(),
        sp.GetRequiredService<McpRequestProcessor>(),
        int.Parse(mcpPort!),
        mcpApiKey,
        oauthToken,
        oauthResourceUrl,
        authDisabled));
}
else
{
    services.AddSingleton<McpServer>();
}

// Build service provider
var serviceProvider = services.BuildServiceProvider();

// Get logger
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
logger.LogInformation("MCP Canonical Facts Server starting...");
logger.LogInformation("Database path: {DatabasePath}", databasePath);
logger.LogInformation("Transport mode: {Mode}", useHttpMode ? "HTTP/SSE" : "stdio");

if (useHttpMode)
{
    logger.LogInformation("MCP auth: {Mode}", authDisabled ? "DISABLED" : "Bearer token required");
    logger.LogInformation("Health auth: {Mode}", !string.IsNullOrEmpty(mcpApiKey) ? "X-API-Key required" : "unauthenticated");
}

// Start server based on mode
var cts = new CancellationTokenSource();

Console.CancelKeyPress += (s, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

try
{
    if (useHttpMode)
    {
        var httpServer = serviceProvider.GetRequiredService<McpHttpServer>();
        await httpServer.StartAsync(cts.Token);
    }
    else
    {
        var stdioServer = serviceProvider.GetRequiredService<McpServer>();
        await stdioServer.StartAsync(cts.Token);
    }
}
catch (Exception ex)
{
    logger.LogCritical(ex, "Server failed to start");
    return 1;
}

return 0;
