using System.CommandLine;
using KateMorrisonMCP.Data;
using KateMorrisonMCP.Ingestion.Engine;
using KateMorrisonMCP.Ingestion.SchemaManagement;

namespace KateMorrisonMCP.Ingestion;

class Program
{
    static async Task<int> Main(string[] args)
    {
        // Create root command
        var rootCommand = new RootCommand("Kate Morrison MCP - Canonical Facts Ingestion Tool");

        // Database option (required)
        var databaseOption = new Option<string>(
            name: "--database",
            description: "Path to SQLite database file")
        {
            IsRequired = true
        };
        databaseOption.AddAlias("-d");

        // Source directory option (required)
        var sourceOption = new Option<string>(
            name: "--source",
            description: "Source directory containing markdown files")
        {
            IsRequired = true
        };
        sourceOption.AddAlias("-s");

        // Mode option
        var modeOption = new Option<string>(
            name: "--mode",
            description: "Ingestion mode: full (rebuild) or incremental",
            getDefaultValue: () => "incremental");
        modeOption.AddAlias("-m");

        // Verbose option
        var verboseOption = new Option<bool>(
            name: "--verbose",
            description: "Show detailed processing information",
            getDefaultValue: () => false);
        verboseOption.AddAlias("-v");

        // Purge deleted option
        var purgeOption = new Option<bool>(
            name: "--purge-deleted",
            description: "Remove records from files that no longer exist",
            getDefaultValue: () => false);

        rootCommand.AddOption(databaseOption);
        rootCommand.AddOption(sourceOption);
        rootCommand.AddOption(modeOption);
        rootCommand.AddOption(verboseOption);
        rootCommand.AddOption(purgeOption);

        rootCommand.SetHandler(async (database, source, mode, verbose, purgeDeleted) =>
        {
            await RunIngestionAsync(database, source, mode, verbose, purgeDeleted);
        }, databaseOption, sourceOption, modeOption, verboseOption, purgeOption);

        return await rootCommand.InvokeAsync(args);
    }

    static async Task RunIngestionAsync(string databasePath, string sourceDir, string mode, bool verbose, bool purgeDeleted)
    {
        try
        {
            Console.WriteLine("Kate Morrison MCP - Canonical Facts Ingestion Tool");
            Console.WriteLine("==================================================\n");

            // Validate inputs
            if (!File.Exists(databasePath))
            {
                Console.WriteLine($"Error: Database file not found: {databasePath}");
                Console.WriteLine("Please create the database first or provide a valid path.");
                Environment.Exit(1);
            }

            if (!Directory.Exists(sourceDir))
            {
                Console.WriteLine($"Error: Source directory not found: {sourceDir}");
                Environment.Exit(1);
            }

            var fullRebuild = mode.ToLowerInvariant() == "full";

            if (verbose)
            {
                Console.WriteLine($"Database: {databasePath}");
                Console.WriteLine($"Source: {sourceDir}");
                Console.WriteLine($"Mode: {mode}");
                Console.WriteLine($"Purge deleted: {purgeDeleted}");
                Console.WriteLine();
            }

            // Initialize database connection
            var db = new DatabaseContext(databasePath);
            await db.InitializeAsync();

            // Run schema updates
            Console.WriteLine("Checking schema...");
            var schemaUpdater = new SchemaUpdater(db);
            await schemaUpdater.UpdateSchemaAsync();

            // Run ingestion
            Console.WriteLine($"Starting {mode} ingestion from {sourceDir}...\n");
            var engine = new IngestionEngine(db, verbose);
            var summary = await engine.ProcessDirectoryAsync(sourceDir, fullRebuild);

            // Display summary
            Console.WriteLine(summary.ToString());

            if (summary.ErrorMessages.Count > 0)
            {
                Console.WriteLine("Errors:");
                foreach (var error in summary.ErrorMessages)
                {
                    Console.WriteLine($"  - {error}");
                }
            }

            Console.WriteLine("\nIngestion complete!");

            // TODO: Handle purge-deleted option
            if (purgeDeleted)
            {
                Console.WriteLine("\nNote: --purge-deleted is not yet implemented");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nFatal error: {ex.Message}");
            if (verbose)
            {
                Console.WriteLine($"\nStack trace:\n{ex.StackTrace}");
            }
            Environment.Exit(1);
        }
    }
}
