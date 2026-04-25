using KateMorrisonMCP.Data;
using KateMorrisonMCP.Ingestion.Helpers;
using KateMorrisonMCP.Ingestion.Parsing;
using KateMorrisonMCP.Ingestion.Processors;

namespace KateMorrisonMCP.Ingestion.Engine;

/// <summary>
/// Main ingestion engine that orchestrates tag processing and orphan cleanup
/// </summary>
public class IngestionEngine
{
    private readonly DatabaseContext _db;
    private readonly TagParser _tagParser;
    private readonly List<ITagProcessor> _processors;
    private readonly bool _verbose;

    // Track touched records per table for orphan cleanup
    private readonly Dictionary<string, HashSet<int>> _touchedRecords = new();

    public IngestionEngine(DatabaseContext db, bool verbose = false)
    {
        _db = db;
        _tagParser = new TagParser();
        _verbose = verbose;

        var characterLookup = new CharacterLookup(db);

        // Initialize all processors
        _processors = new List<ITagProcessor>
        {
            new CharacterProcessor(db),
            new LocationProcessor(db),
            new RoomProcessor(db),
            new ScheduleProcessor(db, characterLookup),
            new NegativeProcessor(db, characterLookup),
            new EducationProcessor(db, characterLookup),
            new RelationshipProcessor(db, characterLookup),
            new PossessionProcessor(db, characterLookup),
            new TimelineProcessor(db, characterLookup)
        };
    }

    /// <summary>
    /// Processes all markdown files in a directory
    /// </summary>
    public async Task<IngestionSummary> ProcessDirectoryAsync(string sourceDirectory, bool fullRebuild = false)
    {
        var summary = new IngestionSummary { StartTime = DateTime.UtcNow };

        if (!Directory.Exists(sourceDirectory))
        {
            throw new DirectoryNotFoundException($"Source directory not found: {sourceDirectory}");
        }

        // Find all markdown files
        var markdownFiles = Directory.GetFiles(sourceDirectory, "*.md", SearchOption.AllDirectories);

        if (_verbose)
        {
            Console.WriteLine($"Found {markdownFiles.Length} markdown files");
        }

        foreach (var filePath in markdownFiles)
        {
            try
            {
                if (_verbose)
                {
                    Console.WriteLine($"\nProcessing: {Path.GetFileName(filePath)}");
                }

                var fileResult = await ProcessFileWithCleanupAsync(filePath);

                summary.FilesProcessed++;
                summary.TagsProcessed += fileResult.TagsProcessed;
                summary.RecordsInserted += fileResult.RecordsInserted;
                summary.RecordsUpdated += fileResult.RecordsUpdated;
                summary.RecordsDeleted += fileResult.RecordsDeleted;
            }
            catch (Exception ex)
            {
                summary.Errors++;
                summary.ErrorMessages.Add($"{filePath}: {ex.Message}");
                Console.WriteLine($"Error processing {Path.GetFileName(filePath)}: {ex.Message}");
            }
        }

        summary.EndTime = DateTime.UtcNow;
        return summary;
    }

    /// <summary>
    /// Processes a single file with orphan cleanup
    /// This is the critical algorithm for ensuring deleted tags are removed from database
    /// </summary>
    private async Task<IngestionSummary> ProcessFileWithCleanupAsync(string filePath)
    {
        var result = new IngestionSummary { StartTime = DateTime.UtcNow };

        // STEP 1: Query existing record IDs from this source_file
        var existingRecords = await GetExistingRecordsFromFileAsync(filePath);

        // STEP 2: Initialize touched records tracking
        _touchedRecords.Clear();
        foreach (var tableName in existingRecords.Keys)
        {
            _touchedRecords[tableName] = new HashSet<int>();
        }

        // STEP 3: Parse tags from file
        var tags = _tagParser.ParseFile(filePath).ToList();
        result.TagsProcessed = tags.Count;

        if (_verbose && tags.Count > 0)
        {
            Console.WriteLine($"  Found {tags.Count} canonical tags");
        }

        // STEP 4: Sort by processing priority (dependency order)
        var orderedTags = tags
            .Select(tag => new
            {
                Tag = tag,
                Processor = _processors.FirstOrDefault(p => p.TagType == tag.Type)
            })
            .Where(x => x.Processor != null)
            .OrderBy(x => x.Processor!.Priority)
            .ToList();

        // STEP 5: Process each tag, track touched IDs
        foreach (var item in orderedTags)
        {
            try
            {
                var recordId = await item.Processor!.ProcessAsync(item.Tag);
                var tableName = GetTableName(item.Tag.Type);

                // Track this record as touched
                if (!_touchedRecords.ContainsKey(tableName))
                {
                    _touchedRecords[tableName] = new HashSet<int>();
                }
                _touchedRecords[tableName].Add(recordId);

                // Determine if insert or update
                if (existingRecords.ContainsKey(tableName) && existingRecords[tableName].Contains(recordId))
                {
                    result.RecordsUpdated++;
                }
                else
                {
                    result.RecordsInserted++;
                }

                if (_verbose)
                {
                    Console.WriteLine($"  ✓ {item.Tag.Type}: {item.Tag.GetRequired(GetPrimaryFieldName(item.Tag.Type))}");
                }
            }
            catch (Exception ex)
            {
                result.Errors++;
                result.ErrorMessages.Add($"{item.Tag.SourceFile}:{item.Tag.LineNumber} - {ex.Message}");
                Console.WriteLine($"  ✗ Error at line {item.Tag.LineNumber}: {ex.Message}");
            }
        }

        // STEP 6: Delete orphaned records (existed but not touched)
        var deletedCount = await DeleteOrphanedRecordsAsync(filePath, existingRecords);
        result.RecordsDeleted = deletedCount;

        if (_verbose && deletedCount > 0)
        {
            Console.WriteLine($"  Deleted {deletedCount} orphaned records");
        }

        result.EndTime = DateTime.UtcNow;
        return result;
    }

    /// <summary>
    /// Gets all existing record IDs from a source file, grouped by table
    /// </summary>
    private async Task<Dictionary<string, HashSet<int>>> GetExistingRecordsFromFileAsync(string filePath)
    {
        var result = new Dictionary<string, HashSet<int>>();

        var tables = new[]
        {
            "characters", "locations", "location_rooms", "schedules",
            "character_negatives", "education", "relationships",
            "possessions", "timeline_events"
        };

        foreach (var table in tables)
        {
            var ids = await _db.QueryAsync<int>(
                $"SELECT id FROM {table} WHERE source_file = @SourceFile",
                new { SourceFile = filePath });

            if (ids.Any())
            {
                result[table] = new HashSet<int>(ids);
            }
        }

        return result;
    }

    /// <summary>
    /// Deletes orphaned records (records that existed in file but were not touched during processing)
    /// </summary>
    private async Task<int> DeleteOrphanedRecordsAsync(string filePath, Dictionary<string, HashSet<int>> existingRecords)
    {
        int totalDeleted = 0;

        // Delete in reverse dependency order to avoid foreign key violations
        var tableDeletionOrder = new[]
        {
            "timeline_events",      // Priority 6
            "relationships",        // Priority 5
            "possessions",          // Priority 5
            "schedules",            // Priority 4
            "character_negatives",  // Priority 4
            "education",            // Priority 4
            "location_rooms",       // Priority 3
            "locations",            // Priority 2
            "characters"            // Priority 1
        };

        foreach (var table in tableDeletionOrder)
        {
            if (!existingRecords.ContainsKey(table))
            {
                continue;
            }

            var existingIds = existingRecords[table];
            var touchedIds = _touchedRecords.ContainsKey(table) ? _touchedRecords[table] : new HashSet<int>();

            // Orphans = existed but not touched
            var orphanIds = existingIds.Except(touchedIds).ToList();

            if (orphanIds.Count > 0)
            {
                // Delete orphaned records
                foreach (var id in orphanIds)
                {
                    await _db.ExecuteAsync($"DELETE FROM {table} WHERE id = @Id", new { Id = id });
                    totalDeleted++;
                }

                if (_verbose)
                {
                    Console.WriteLine($"  Deleted {orphanIds.Count} orphaned records from {table}");
                }
            }
        }

        return totalDeleted;
    }

    /// <summary>
    /// Maps tag type to database table name
    /// </summary>
    private string GetTableName(string tagType) => tagType switch
    {
        "character" => "characters",
        "location" => "locations",
        "room" => "location_rooms",
        "schedule" => "schedules",
        "negative" => "character_negatives",
        "education" => "education",
        "relationship" => "relationships",
        "possession" => "possessions",
        "timeline" => "timeline_events",
        _ => throw new ArgumentException($"Unknown tag type: {tagType}")
    };

    /// <summary>
    /// Gets the primary identifying field name for a tag type
    /// </summary>
    private string GetPrimaryFieldName(string tagType) => tagType switch
    {
        "character" => "name",
        "location" => "name",
        "room" => "room_name",
        "schedule" => "schedule_name",
        "negative" => "negative_behavior",
        "education" => "institution",
        "relationship" => "relationship_type",
        "possession" => "item_name",
        "timeline" => "event_title",
        _ => "name"
    };
}
