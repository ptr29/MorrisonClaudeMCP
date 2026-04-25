using Microsoft.Data.Sqlite;
using Dapper;

namespace KateMorrisonMCP.Data;

/// <summary>
/// Database context for SQLite connection and query execution
/// </summary>
public class DatabaseContext : IDisposable
{
    private readonly string _connectionString;
    private SqliteConnection? _connection;
    private bool _disposed = false;

    public DatabaseContext(string databasePath)
    {
        _connectionString = $"Data Source={databasePath}";
    }

    /// <summary>
    /// Initialize database connection with WAL mode and foreign key enforcement
    /// </summary>
    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection(_connectionString);
        await _connection.OpenAsync();

        // Enable WAL mode for better concurrency
        await _connection.ExecuteAsync("PRAGMA journal_mode=WAL;");

        // Enable foreign key constraints
        await _connection.ExecuteAsync("PRAGMA foreign_keys=ON;");
    }

    /// <summary>
    /// Get the active database connection
    /// </summary>
    public SqliteConnection Connection =>
        _connection ?? throw new InvalidOperationException("Database not initialized. Call InitializeAsync first.");

    /// <summary>
    /// Execute a query and return a single result or default
    /// </summary>
    public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? param = null)
    {
        return await Connection.QuerySingleOrDefaultAsync<T>(sql, param);
    }

    /// <summary>
    /// Execute a query and return multiple results
    /// </summary>
    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
    {
        return await Connection.QueryAsync<T>(sql, param);
    }

    /// <summary>
    /// Execute a command (INSERT, UPDATE, DELETE) and return affected row count
    /// </summary>
    public async Task<int> ExecuteAsync(string sql, object? param = null)
    {
        return await Connection.ExecuteAsync(sql, param);
    }

    /// <summary>
    /// Perform health check to verify database integrity
    /// </summary>
    public async Task<HealthCheckResult> PerformHealthCheckAsync()
    {
        var results = new List<string>();

        try
        {
            // Check database connectivity
            var count = await QuerySingleAsync<int>("SELECT COUNT(*) FROM characters");
            results.Add($"✓ Database connected, {count} characters found");

            // Verify required tables
            var requiredTables = new[]
            {
                "characters", "locations", "location_rooms",
                "timeline_events", "event_participants", "relationships",
                "possessions", "schedules", "character_negatives", "education", "update_log"
            };

            foreach (var table in requiredTables)
            {
                var exists = await QuerySingleAsync<int>(
                    "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=@Table",
                    new { Table = table });

                if (exists == 0)
                    return new HealthCheckResult
                    {
                        IsHealthy = false,
                        Message = $"Missing table: {table}"
                    };
            }

            results.Add("✓ All required tables present");

            // Verify foreign keys are enabled
            var foreignKeysEnabled = await QuerySingleAsync<int>("PRAGMA foreign_keys");
            if (foreignKeysEnabled == 0)
                return new HealthCheckResult
                {
                    IsHealthy = false,
                    Message = "Foreign keys not enabled"
                };

            results.Add("✓ Foreign keys enabled");

            return new HealthCheckResult
            {
                IsHealthy = true,
                Message = string.Join("\n", results)
            };
        }
        catch (Exception ex)
        {
            return new HealthCheckResult
            {
                IsHealthy = false,
                Message = $"Health check failed: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Execute a query and return a single result (throws if not found)
    /// </summary>
    public async Task<T> QuerySingleAsync<T>(string sql, object? param = null)
    {
        return await Connection.QuerySingleAsync<T>(sql, param);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _connection?.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Health check result
/// </summary>
public class HealthCheckResult
{
    public bool IsHealthy { get; set; }
    public string Message { get; set; } = string.Empty;
}
