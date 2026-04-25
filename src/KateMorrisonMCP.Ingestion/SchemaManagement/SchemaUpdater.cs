using KateMorrisonMCP.Data;

namespace KateMorrisonMCP.Ingestion.SchemaManagement;

/// <summary>
/// Manages schema updates for the ingestion system
/// Adds source_file columns to tables that don't have them
/// Creates ingestion metadata tables
/// </summary>
public class SchemaUpdater
{
    private readonly DatabaseContext _db;

    public SchemaUpdater(DatabaseContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Ensures all tables have source_file column for orphan tracking
    /// </summary>
    public async Task EnsureSourceFileColumnsAsync()
    {
        var tables = new[]
        {
            "characters",
            "locations",
            "location_rooms",
            "schedules",
            "character_negatives",
            "education",
            "relationships",
            "possessions"
        };

        foreach (var table in tables)
        {
            var hasColumn = await _db.QuerySingleOrDefaultAsync<int?>(
                $"SELECT COUNT(*) FROM pragma_table_info('{table}') WHERE name='source_file'");

            if (hasColumn == 0)
            {
                await _db.ExecuteAsync($"ALTER TABLE {table} ADD COLUMN source_file TEXT");
                Console.WriteLine($"✓ Added source_file column to {table}");
            }
        }
    }

    /// <summary>
    /// Creates metadata tables for tracking ingestion runs
    /// </summary>
    public async Task EnsureMetadataTablesAsync()
    {
        // Ingestion run history
        await _db.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS ingestion_metadata (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                run_timestamp TEXT NOT NULL,
                mode TEXT NOT NULL,
                source_directory TEXT NOT NULL,
                files_processed INTEGER DEFAULT 0,
                tags_processed INTEGER DEFAULT 0,
                records_inserted INTEGER DEFAULT 0,
                records_updated INTEGER DEFAULT 0,
                records_deleted INTEGER DEFAULT 0,
                errors INTEGER DEFAULT 0,
                duration_seconds REAL
            )");

        // Last successful run tracking
        await _db.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS last_run_state (
                id INTEGER PRIMARY KEY DEFAULT 1,
                last_successful_run TEXT,
                CHECK (id = 1)
            )");

        // Initialize last_run_state if empty
        var hasState = await _db.QuerySingleOrDefaultAsync<int?>(
            "SELECT COUNT(*) FROM last_run_state");

        if (hasState == 0)
        {
            await _db.ExecuteAsync(
                "INSERT INTO last_run_state (id, last_successful_run) VALUES (1, NULL)");
        }

        Console.WriteLine("✓ Metadata tables ready");
    }

    /// <summary>
    /// Runs all schema updates
    /// </summary>
    public async Task UpdateSchemaAsync()
    {
        Console.WriteLine("Checking schema updates...");
        await EnsureSourceFileColumnsAsync();
        await EnsureMetadataTablesAsync();
        Console.WriteLine("Schema updates complete\n");
    }
}
