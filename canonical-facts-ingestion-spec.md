# Canonical Facts Ingestion Process - Implementation Specification

## Overview

Build a C# tool that extracts `<!-- canonical: ... -->` tags from markdown files and populates/updates a SQLite database. This tool will be used alongside the MCP server to keep the canonical facts database synchronized with project files.

---

## Requirements

### Input
- Directory path containing markdown files (recursive scan)
- SQLite database path (create if doesn't exist)

### Output
- Populated/updated SQLite database
- Console log of operations performed
- Error report for invalid tags

### Operations
1. **Full Rebuild**: Drop and recreate all data from source files
2. **Incremental Update**: Process only files modified since last run

---

## Tag Format Specification

### Block Format (Primary)

```
<!-- canonical: [type]
field: value
field: value
field: multi-word value
field: value with special chars: colons, commas
-->
```

**Rules:**
- Opening: `<!-- canonical: ` followed by type name
- Fields: One per line, `field_name: value` format
- Field names: lowercase, underscores allowed, no spaces
- Values: Everything after `: ` to end of line (trim whitespace)
- Closing: `-->` on its own line or at end of last field line
- Multi-line values: NOT supported (use single line)

### Inline Format (Secondary)

```
<!-- canonical: [type] | field: value | field: value -->
```

**Rules:**
- Single line, pipe-delimited
- Same field format as block

### Extraction Regex

```csharp
// Block format - captures type and body
private static readonly Regex BlockTagRegex = new Regex(
    @"<!--\s*canonical:\s*(\w+)\s*\n([\s\S]*?)-->",
    RegexOptions.Compiled | RegexOptions.Multiline);

// Inline format - captures type and field string
private static readonly Regex InlineTagRegex = new Regex(
    @"<!--\s*canonical:\s*(\w+)\s*\|(.*?)-->",
    RegexOptions.Compiled);

// Field extraction from body
private static readonly Regex FieldRegex = new Regex(
    @"^(\w+):\s*(.*)$",
    RegexOptions.Compiled | RegexOptions.Multiline);

// Field extraction from inline (pipe-delimited)
private static readonly Regex InlineFieldRegex = new Regex(
    @"(\w+):\s*([^|]+)",
    RegexOptions.Compiled);
```

---

## Tag Types and Field Mappings

### 1. character

**Database Table:** `characters`

| Tag Field | DB Column | Type | Required | Notes |
|-----------|-----------|------|----------|-------|
| name | full_name | TEXT | YES | Unique identifier |
| preferred_name | preferred_name | TEXT | no | |
| age | age | INTEGER | no | |
| birthday | birthday | TEXT | no | YYYY-MM-DD or MM-DD |
| birth_year | birth_year | INTEGER | no | |
| height_inches | height_inches | INTEGER | no | |
| height | height_inches | INTEGER | no | Parse "5'6\"" to 66 |
| weight_lbs | weight_lbs | INTEGER | no | |
| build | build | TEXT | no | |
| hair_color | hair_color | TEXT | no | |
| hair_length | hair_length | TEXT | no | |
| eye_color | eye_color | TEXT | no | |
| distinctive_features | distinctive_features | TEXT | no | Store as JSON array |
| occupation | occupation | TEXT | no | |
| employer | employer | TEXT | no | |
| job_title | job_title | TEXT | no | |
| work_schedule_type | work_schedule_type | TEXT | no | |
| character_type | character_type | TEXT | no | Default: 'secondary' |

**Special Parsing:**
- `height`: Convert "5'6\"" format to inches: `feet * 12 + inches`
- `distinctive_features`: Split on comma, trim, store as JSON array

**Upsert Logic:** Match on `full_name`, update non-null fields

---

### 2. schedule

**Database Table:** `schedules`

| Tag Field | DB Column | Type | Required | Notes |
|-----------|-----------|------|----------|-------|
| character | character_id | INTEGER | YES | Lookup by name → id |
| type | schedule_type | TEXT | YES | work, exercise, daily_routine, weekly_commitment |
| name | schedule_name | TEXT | YES | |
| days | days_of_week | TEXT | no | JSON array or keyword |
| start_time | start_time | TEXT | no | HH:MM format |
| end_time | end_time | TEXT | no | HH:MM format |
| duration_minutes | duration_minutes | INTEGER | no | |
| location | location_description | TEXT | no | |
| description | description | TEXT | no | |
| exceptions | exceptions | TEXT | no | Store as JSON array |
| is_current | is_current | INTEGER | no | Default: 1 |

**Special Parsing:**
- `days`: 
  - If "daily" → `["daily"]`
  - If "weekdays" → `["monday","tuesday","wednesday","thursday","friday"]`
  - If "weekends" → `["saturday","sunday"]`
  - Otherwise split on comma, lowercase, trim → JSON array
- `exceptions`: Split on semicolon or comma (if no semicolon), trim → JSON array

**Upsert Logic:** Match on `character_id` + `schedule_name`, replace entire record

---

### 3. negative

**Database Table:** `character_negatives`

| Tag Field | DB Column | Type | Required | Notes |
|-----------|-----------|------|----------|-------|
| character | character_id | INTEGER | YES | Lookup by name → id |
| category | negative_category | TEXT | YES | exercise, food, social, work, behavior |
| behavior | negative_behavior | TEXT | YES | Should start with "Does NOT" |
| strength | strength | TEXT | YES | absolute, strong, preference |
| explanation | explanation | TEXT | no | |
| exceptions | exception_conditions | TEXT | no | Store as JSON array |

**Validation:**
- `strength` must be one of: absolute, strong, preference
- `category` should be one of: exercise, food, social, work, behavior (warn if not)

**Upsert Logic:** Match on `character_id` + `negative_behavior`, replace entire record

---

### 4. location

**Database Table:** `locations`

| Tag Field | DB Column | Type | Required | Notes |
|-----------|-----------|------|----------|-------|
| name | name | TEXT | YES | Unique identifier |
| address_street | address_street | TEXT | no | |
| address_city | address_city | TEXT | no | |
| address_state | address_state | TEXT | no | |
| address_zip | address_zip | TEXT | no | |
| location_type | location_type | TEXT | no | house, apartment, restaurant, etc. |
| building_type | building_type | TEXT | no | |
| floor_count | floor_count | INTEGER | no | |
| unit_number | unit_number | TEXT | no | |
| square_feet | square_feet | INTEGER | no | |
| owner | owner_id | INTEGER | no | Lookup character by name → id |
| ownership_type | ownership_type | TEXT | no | owned, rented |
| monthly_cost | monthly_cost | REAL | no | |
| neighborhood | neighborhood | TEXT | no | |

**Upsert Logic:** Match on `name`, update non-null fields

---

### 5. room

**Database Table:** `location_rooms`

| Tag Field | DB Column | Type | Required | Notes |
|-----------|-----------|------|----------|-------|
| location | location_id | INTEGER | YES | Lookup by name → id |
| name | room_name | TEXT | YES | |
| floor | floor_level | TEXT | YES | main, upper, lower, basement, or number |
| room_type | room_type | TEXT | no | |
| width_feet | width_feet | REAL | no | |
| length_feet | length_feet | REAL | no | |
| dimensions | (parsed) | | no | Parse "14'2\" × 10'11\"" to width/length |
| current_use | current_use | TEXT | no | |
| key_features | key_features | TEXT | no | Store as JSON array |
| furniture | furniture | TEXT | no | Store as JSON array |

**Special Parsing:**
- `dimensions`: Parse format like "14'2\" × 10'11\"" or "14.2 × 10.9"
  - Extract two measurements, convert feet'inches" to decimal feet
  - First value → width_feet, second → length_feet
- `key_features`, `furniture`: Split on comma, trim → JSON array

**Upsert Logic:** Match on `location_id` + `room_name`, replace entire record

---

### 6. timeline

**Database Table:** `timeline_events`

| Tag Field | DB Column | Type | Required | Notes |
|-----------|-----------|------|----------|-------|
| date | event_date | TEXT | YES | ISO format |
| precision | event_date_precision | TEXT | no | day, month, year. Default: day |
| title | event_title | TEXT | YES | |
| description | event_description | TEXT | no | |
| participants | (separate table) | | no | Comma-separated character names |
| location | location_description | TEXT | no | |
| category | event_category | TEXT | no | |
| significance | significance | TEXT | no | major, minor, background |

**Special Handling:**
- `participants`: After inserting timeline event, parse comma-separated names, lookup each character_id, insert into `event_participants` table

**Upsert Logic:** Match on `event_date` + `event_title`, replace record and re-create participants

---

### 7. relationship

**Database Table:** `relationships`

| Tag Field | DB Column | Type | Required | Notes |
|-----------|-----------|------|----------|-------|
| character_a | character_a_id | INTEGER | YES | Lookup by name → id |
| character_b | character_b_id | INTEGER | YES | Lookup by name → id |
| type | relationship_type | TEXT | YES | |
| subtype | relationship_subtype | TEXT | no | |
| start_date | start_date | TEXT | no | |
| end_date | end_date | TEXT | no | |
| status | current_status | TEXT | no | active, ended, estranged, deceased |
| direction | direction | TEXT | no | mutual, a_to_b, b_to_a |
| notes | notes | TEXT | no | |

**Upsert Logic:** Match on `character_a_id` + `character_b_id` + `relationship_type`, replace record

---

### 8. possession

**Database Table:** `possessions`

| Tag Field | DB Column | Type | Required | Notes |
|-----------|-----------|------|----------|-------|
| owner | owner_id | INTEGER | YES | Lookup by name → id |
| item | item_name | TEXT | YES | |
| category | item_category | TEXT | no | |
| description | item_description | TEXT | no | |
| acquisition_date | acquisition_date | TEXT | no | |
| acquisition_method | acquisition_method | TEXT | no | |
| acquisition_from | acquisition_from | TEXT | no | |
| storage_location | storage_location | TEXT | no | |
| sentimental_value | sentimental_value | TEXT | no | |
| significance_notes | significance_notes | TEXT | no | |
| is_current | is_current | INTEGER | no | Default: 1 |

**Upsert Logic:** Match on `owner_id` + `item_name`, replace record

---

### 9. education

**Database Table:** `education`

| Tag Field | DB Column | Type | Required | Notes |
|-----------|-----------|------|----------|-------|
| character | character_id | INTEGER | YES | Lookup by name → id |
| institution | institution | TEXT | YES | |
| degree_type | degree_type | TEXT | YES | BS, BA, MS, MA, PhD, etc. |
| field | field_of_study | TEXT | no | |
| start_year | start_year | INTEGER | no | |
| end_year | end_year | INTEGER | no | NULL if ongoing |
| is_completed | is_completed | INTEGER | no | 1 = completed, 0 = in progress |
| is_current | is_current | INTEGER | no | 1 = currently enrolled |
| advisor | advisor | TEXT | no | Name of advisor (for graduate degrees) |
| notes | notes | TEXT | no | Additional context |

**Example:**
```markdown
<!-- canonical: education
character: Kate Morrison
institution: Northwestern University
degree_type: PhD
field: Environmental Engineering
start_year: 2022
is_completed: 0
is_current: 1
advisor: Dr. Sarah Chen
notes: Year 4. Research focus: microplastics and PFAS in Great Lakes drinking water.
-->
```

**Upsert Logic:** Match on `character_id` + `institution` + `degree_type`, replace record

---

## Database Schema

Use this schema for table creation:

```sql
-- See mcp-seed-data.sql for complete schema
-- Key tables needed:

CREATE TABLE IF NOT EXISTS characters (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    full_name TEXT NOT NULL UNIQUE,
    preferred_name TEXT,
    age INTEGER,
    birthday TEXT,
    birth_year INTEGER,
    height_inches INTEGER,
    weight_lbs INTEGER,
    build TEXT,
    hair_color TEXT,
    hair_length TEXT,
    eye_color TEXT,
    distinctive_features TEXT,  -- JSON array
    occupation TEXT,
    employer TEXT,
    job_title TEXT,
    work_location TEXT,
    work_schedule_type TEXT,
    phone TEXT,
    email TEXT,
    residence_id INTEGER,
    character_type TEXT DEFAULT 'secondary',
    is_alive INTEGER DEFAULT 1,
    created_at TEXT DEFAULT CURRENT_TIMESTAMP,
    updated_at TEXT DEFAULT CURRENT_TIMESTAMP,
    source_file TEXT,  -- Track which file this came from
    FOREIGN KEY (residence_id) REFERENCES locations(id)
);

CREATE TABLE IF NOT EXISTS character_negatives (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    character_id INTEGER NOT NULL,
    negative_category TEXT NOT NULL,
    negative_behavior TEXT NOT NULL,
    strength TEXT DEFAULT 'strong',
    explanation TEXT,
    exception_conditions TEXT,  -- JSON array
    source_file TEXT,
    FOREIGN KEY (character_id) REFERENCES characters(id)
);

CREATE TABLE IF NOT EXISTS schedules (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    character_id INTEGER NOT NULL,
    schedule_type TEXT NOT NULL,
    schedule_name TEXT NOT NULL,
    days_of_week TEXT,  -- JSON array
    start_time TEXT,
    end_time TEXT,
    duration_minutes INTEGER,
    location_id INTEGER,
    location_description TEXT,
    description TEXT,
    exceptions TEXT,  -- JSON array
    effective_from TEXT,
    effective_until TEXT,
    is_current INTEGER DEFAULT 1,
    source_file TEXT,
    FOREIGN KEY (character_id) REFERENCES characters(id),
    FOREIGN KEY (location_id) REFERENCES locations(id)
);

CREATE TABLE IF NOT EXISTS education (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    character_id INTEGER NOT NULL,
    institution TEXT NOT NULL,
    degree_type TEXT NOT NULL,
    field_of_study TEXT,
    start_year INTEGER,
    end_year INTEGER,
    is_completed INTEGER DEFAULT 0,
    is_current INTEGER DEFAULT 0,
    advisor TEXT,
    notes TEXT,
    source_file TEXT,
    FOREIGN KEY (character_id) REFERENCES characters(id)
);

-- Add source_file TEXT column to all tables for tracking
```

---

## Implementation Architecture

### Project Structure

```
CanonicalFactsIngestion/
├── CanonicalFactsIngestion.sln
├── src/
│   ├── CanonicalFactsIngestion/
│   │   ├── Program.cs
│   │   ├── IngestionEngine.cs
│   │   ├── TagParser.cs
│   │   ├── Parsers/
│   │   │   ├── ITagParser.cs
│   │   │   ├── CharacterParser.cs
│   │   │   ├── ScheduleParser.cs
│   │   │   ├── NegativeParser.cs
│   │   │   ├── LocationParser.cs
│   │   │   ├── RoomParser.cs
│   │   │   ├── TimelineParser.cs
│   │   │   ├── RelationshipParser.cs
│   │   │   ├── PossessionParser.cs
│   │   │   └── EducationParser.cs
│   │   ├── Database/
│   │   │   ├── DatabaseContext.cs
│   │   │   ├── SchemaManager.cs
│   │   │   └── Repositories/
│   │   │       ├── CharacterRepository.cs
│   │   │       ├── ScheduleRepository.cs
│   │   │       └── ... (one per table)
│   │   ├── Models/
│   │   │   ├── CanonicalTag.cs
│   │   │   ├── ParsedCharacter.cs
│   │   │   └── ... (one per type)
│   │   └── CanonicalFactsIngestion.csproj
└── tests/
    └── CanonicalFactsIngestion.Tests/
```

### Core Classes

#### Program.cs

```csharp
using System.CommandLine;

var rootCommand = new RootCommand("Canonical Facts Ingestion Tool");

var sourceOption = new Option<DirectoryInfo>(
    "--source",
    "Source directory containing markdown files"
) { IsRequired = true };

var databaseOption = new Option<FileInfo>(
    "--database",
    "SQLite database file path"
) { IsRequired = true };

var modeOption = new Option<string>(
    "--mode",
    getDefaultValue: () => "incremental",
    "Ingestion mode: 'full' or 'incremental'"
);

var purgeDeletedOption = new Option<bool>(
    "--purge-deleted",
    getDefaultValue: () => false,
    "Remove records from files that no longer exist in source directory"
);

rootCommand.AddOption(sourceOption);
rootCommand.AddOption(databaseOption);
rootCommand.AddOption(modeOption);
rootCommand.AddOption(purgeDeletedOption);

rootCommand.SetHandler(async (source, database, mode, purgeDeleted) =>
{
    var engine = new IngestionEngine(database.FullName);
    await engine.InitializeAsync();
    
    if (purgeDeleted)
    {
        await engine.PurgeDeletedFilesAsync(source.FullName);
    }
    
    if (mode == "full")
    {
        await engine.FullRebuildAsync(source.FullName);
    }
    else
    {
        await engine.IncrementalUpdateAsync(source.FullName);
    }
}, sourceOption, databaseOption, modeOption, purgeDeletedOption);

return await rootCommand.InvokeAsync(args);
```

#### TagParser.cs

```csharp
public class TagParser
{
    private static readonly Regex BlockTagRegex = new Regex(
        @"<!--\s*canonical:\s*(\w+)\s*\n([\s\S]*?)-->",
        RegexOptions.Compiled | RegexOptions.Multiline);

    private static readonly Regex InlineTagRegex = new Regex(
        @"<!--\s*canonical:\s*(\w+)\s*\|(.*?)-->",
        RegexOptions.Compiled);

    private static readonly Regex FieldRegex = new Regex(
        @"^(\w+):\s*(.*)$",
        RegexOptions.Compiled | RegexOptions.Multiline);

    public IEnumerable<CanonicalTag> ParseFile(string filePath)
    {
        var content = File.ReadAllText(filePath);
        var tags = new List<CanonicalTag>();

        // Parse block tags
        foreach (Match match in BlockTagRegex.Matches(content))
        {
            var tag = new CanonicalTag
            {
                Type = match.Groups[1].Value.ToLowerInvariant(),
                SourceFile = filePath,
                LineNumber = GetLineNumber(content, match.Index)
            };

            var body = match.Groups[2].Value;
            foreach (Match fieldMatch in FieldRegex.Matches(body))
            {
                var fieldName = fieldMatch.Groups[1].Value.ToLowerInvariant();
                var fieldValue = fieldMatch.Groups[2].Value.Trim();
                tag.Fields[fieldName] = fieldValue;
            }

            tags.Add(tag);
        }

        // Parse inline tags
        foreach (Match match in InlineTagRegex.Matches(content))
        {
            var tag = new CanonicalTag
            {
                Type = match.Groups[1].Value.ToLowerInvariant(),
                SourceFile = filePath,
                LineNumber = GetLineNumber(content, match.Index)
            };

            var fieldString = match.Groups[2].Value;
            var fieldParts = fieldString.Split('|');
            foreach (var part in fieldParts)
            {
                var colonIndex = part.IndexOf(':');
                if (colonIndex > 0)
                {
                    var fieldName = part.Substring(0, colonIndex).Trim().ToLowerInvariant();
                    var fieldValue = part.Substring(colonIndex + 1).Trim();
                    tag.Fields[fieldName] = fieldValue;
                }
            }

            tags.Add(tag);
        }

        return tags;
    }

    private int GetLineNumber(string content, int charIndex)
    {
        return content.Substring(0, charIndex).Count(c => c == '\n') + 1;
    }
}

public class CanonicalTag
{
    public string Type { get; set; } = "";
    public Dictionary<string, string> Fields { get; set; } = new();
    public string SourceFile { get; set; } = "";
    public int LineNumber { get; set; }
}
```

#### IngestionEngine.cs

```csharp
public class IngestionEngine
{
    private readonly DatabaseContext _db;
    private readonly TagParser _tagParser;
    private readonly Dictionary<string, ITagProcessor> _processors;
    
    // Track which records were touched during processing (for orphan cleanup)
    private readonly Dictionary<string, HashSet<int>> _touchedRecords = new();

    public IngestionEngine(string databasePath)
    {
        _db = new DatabaseContext(databasePath);
        _tagParser = new TagParser();
        
        _processors = new Dictionary<string, ITagProcessor>
        {
            ["character"] = new CharacterProcessor(_db),
            ["schedule"] = new ScheduleProcessor(_db),
            ["negative"] = new NegativeProcessor(_db),
            ["location"] = new LocationProcessor(_db),
            ["room"] = new RoomProcessor(_db),
            ["timeline"] = new TimelineProcessor(_db),
            ["relationship"] = new RelationshipProcessor(_db),
            ["possession"] = new PossessionProcessor(_db),
            ["education"] = new EducationProcessor(_db),
        };
    }

    public async Task InitializeAsync()
    {
        await _db.InitializeAsync();
        await _db.EnsureSchemaAsync();
    }

    public async Task FullRebuildAsync(string sourceDirectory)
    {
        Console.WriteLine($"Starting full rebuild from {sourceDirectory}");
        
        // Clear existing data (in correct order for foreign keys)
        await _db.ExecuteAsync("DELETE FROM event_participants");
        await _db.ExecuteAsync("DELETE FROM timeline_events");
        await _db.ExecuteAsync("DELETE FROM possessions");
        await _db.ExecuteAsync("DELETE FROM relationships");
        await _db.ExecuteAsync("DELETE FROM education");
        await _db.ExecuteAsync("DELETE FROM character_negatives");
        await _db.ExecuteAsync("DELETE FROM schedules");
        await _db.ExecuteAsync("DELETE FROM location_rooms");
        await _db.ExecuteAsync("DELETE FROM locations");
        await _db.ExecuteAsync("DELETE FROM characters");
        
        await ProcessDirectoryAsync(sourceDirectory);
        
        Console.WriteLine("Full rebuild complete.");
    }

    public async Task IncrementalUpdateAsync(string sourceDirectory)
    {
        Console.WriteLine($"Starting incremental update from {sourceDirectory}");
        
        // Get last run timestamp from database
        var lastRun = await _db.GetLastIngestionTimestampAsync();
        
        await ProcessDirectoryAsync(sourceDirectory, lastRun);
        
        // Update last run timestamp
        await _db.SetLastIngestionTimestampAsync(DateTime.UtcNow);
        
        Console.WriteLine("Incremental update complete.");
    }

    private async Task ProcessDirectoryAsync(string directory, DateTime? modifiedSince = null)
    {
        var files = Directory.GetFiles(directory, "*.md", SearchOption.AllDirectories);
        
        // Group files for processing
        var filesToProcess = new List<string>();
        
        foreach (var file in files)
        {
            if (modifiedSince.HasValue)
            {
                var lastModified = File.GetLastWriteTimeUtc(file);
                if (lastModified < modifiedSince.Value)
                    continue;
            }
            filesToProcess.Add(file);
        }

        // Process each file with orphan cleanup
        foreach (var file in filesToProcess)
        {
            await ProcessFileWithCleanupAsync(file);
        }
    }

    /// <summary>
    /// Process a single file: extract tags, upsert records, delete orphans.
    /// This ensures that when a tag is removed from a file, the corresponding
    /// database record is also removed.
    /// </summary>
    private async Task ProcessFileWithCleanupAsync(string filePath)
    {
        Console.WriteLine($"Processing: {filePath}");
        
        // Step 1: Get all existing record IDs from this source file (before processing)
        var existingRecords = await GetExistingRecordsFromFileAsync(filePath);
        
        // Step 2: Reset touched records tracking for this file
        _touchedRecords.Clear();
        foreach (var table in existingRecords.Keys)
        {
            _touchedRecords[table] = new HashSet<int>();
        }
        
        // Step 3: Parse and collect tags from file
        var tags = _tagParser.ParseFile(filePath).ToList();
        
        // Step 4: Sort tags by processing priority
        var orderedTags = tags
            .Select(tag => (Priority: GetProcessingPriority(tag.Type), Tag: tag))
            .OrderBy(t => t.Priority)
            .Select(t => t.Tag)
            .ToList();
        
        // Step 5: Process each tag (upsert) and track touched record IDs
        foreach (var tag in orderedTags)
        {
            await ProcessTagAsync(tag);
        }
        
        // Step 6: Delete orphaned records (existed before but not touched during processing)
        await DeleteOrphanedRecordsAsync(filePath, existingRecords);
    }

    /// <summary>
    /// Get all record IDs currently associated with a source file, grouped by table.
    /// </summary>
    private async Task<Dictionary<string, HashSet<int>>> GetExistingRecordsFromFileAsync(string filePath)
    {
        var result = new Dictionary<string, HashSet<int>>();
        
        // Tables that track source_file
        var tables = new[]
        {
            "characters",
            "locations", 
            "location_rooms",
            "schedules",
            "character_negatives",
            "education",
            "relationships",
            "possessions",
            "timeline_events"
        };
        
        foreach (var table in tables)
        {
            var ids = await _db.QueryAsync<int>(
                $"SELECT id FROM {table} WHERE source_file = @FilePath",
                new { FilePath = filePath });
            result[table] = ids.ToHashSet();
        }
        
        return result;
    }

    /// <summary>
    /// Delete records that existed in the file before but were not touched during processing.
    /// This handles the case where a canonical tag was removed from a file.
    /// </summary>
    private async Task DeleteOrphanedRecordsAsync(
        string filePath, 
        Dictionary<string, HashSet<int>> existingRecords)
    {
        // Delete in reverse dependency order (children before parents)
        var deleteOrder = new[]
        {
            "event_participants", // References timeline_events
            "timeline_events",
            "possessions",
            "relationships",
            "education",
            "character_negatives",
            "schedules",
            "location_rooms",
            "locations",
            "characters"
        };
        
        foreach (var table in deleteOrder)
        {
            if (!existingRecords.TryGetValue(table, out var existing))
                continue;
                
            if (!_touchedRecords.TryGetValue(table, out var touched))
                touched = new HashSet<int>();
            
            // Find orphans: existed before but not touched now
            var orphanIds = existing.Except(touched).ToList();
            
            if (orphanIds.Count == 0)
                continue;
            
            // Special handling for timeline_events: delete participants first
            if (table == "timeline_events")
            {
                foreach (var id in orphanIds)
                {
                    await _db.ExecuteAsync(
                        "DELETE FROM event_participants WHERE event_id = @Id",
                        new { Id = id });
                }
            }
            
            // Delete orphaned records
            foreach (var id in orphanIds)
            {
                await _db.ExecuteAsync(
                    $"DELETE FROM {table} WHERE id = @Id",
                    new { Id = id });
                Console.WriteLine($"  Deleted orphaned {table} record (id={id}) - tag removed from file");
            }
        }
    }

    /// <summary>
    /// Record that a specific record ID was touched during processing.
    /// Called by processors after upsert operations.
    /// </summary>
    public void RecordTouched(string tableName, int recordId)
    {
        if (!_touchedRecords.ContainsKey(tableName))
            _touchedRecords[tableName] = new HashSet<int>();
        _touchedRecords[tableName].Add(recordId);
    }

    private int GetProcessingPriority(string tagType)
    {
        return tagType switch
        {
            "character" => 1,   // Process first
            "location" => 2,    // Process second
            "room" => 3,        // Needs locations
            "schedule" => 4,    // Needs characters
            "negative" => 4,    // Needs characters
            "education" => 4,   // Needs characters
            "relationship" => 5, // Needs characters
            "possession" => 5,  // Needs characters
            "timeline" => 6,    // Needs characters, locations
            _ => 99
        };
    }

    private async Task ProcessTagAsync(CanonicalTag tag)
    {
        if (!_processors.TryGetValue(tag.Type, out var processor))
        {
            Console.WriteLine($"  WARNING: Unknown tag type '{tag.Type}' at {tag.SourceFile}:{tag.LineNumber}");
            return;
        }

        try
        {
            // Process and get the record ID
            var recordId = await processor.ProcessAsync(tag);
            
            // Track that this record was touched (for orphan cleanup)
            var tableName = GetTableNameForTagType(tag.Type);
            RecordTouched(tableName, recordId);
            
            Console.WriteLine($"  Processed: {tag.Type} (id={recordId}) from {Path.GetFileName(tag.SourceFile)}:{tag.LineNumber}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ERROR processing {tag.Type} at {tag.SourceFile}:{tag.LineNumber}: {ex.Message}");
        }
    }

    private string GetTableNameForTagType(string tagType)
    {
        return tagType switch
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
    }
}
```
```

#### ITagProcessor.cs

```csharp
public interface ITagProcessor
{
    /// <summary>
    /// Process a canonical tag and upsert to database.
    /// Returns the ID of the record that was inserted/updated.
    /// </summary>
    Task<int> ProcessAsync(CanonicalTag tag);
}
```

**Important:** Each processor must return the ID of the record it created/updated so the engine can track it for orphan cleanup.

#### CharacterProcessor.cs (Example)

```csharp
public class CharacterProcessor : ITagProcessor
{
    private readonly DatabaseContext _db;

    public CharacterProcessor(DatabaseContext db)
    {
        _db = db;
    }

    public async Task<int> ProcessAsync(CanonicalTag tag)
    {
        // Validate required fields
        if (!tag.Fields.TryGetValue("name", out var name) || string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException("Character tag missing required 'name' field");
        }

        // Parse height if provided in feet'inches" format
        int? heightInches = null;
        if (tag.Fields.TryGetValue("height", out var heightStr))
        {
            heightInches = ParseHeight(heightStr);
        }
        else if (tag.Fields.TryGetValue("height_inches", out var heightInchesStr))
        {
            heightInches = int.TryParse(heightInchesStr, out var h) ? h : null;
        }

        // Parse distinctive features as JSON array
        string? distinctiveFeatures = null;
        if (tag.Fields.TryGetValue("distinctive_features", out var featuresStr))
        {
            var features = featuresStr.Split(',').Select(f => f.Trim()).ToArray();
            distinctiveFeatures = JsonSerializer.Serialize(features);
        }

        // Check if character exists
        var existing = await _db.QuerySingleOrDefaultAsync<int?>(
            "SELECT id FROM characters WHERE full_name = @Name",
            new { Name = name });

        int recordId;
        if (existing.HasValue)
        {
            // Update existing character (only non-null fields)
            await UpdateCharacterAsync(existing.Value, tag, heightInches, distinctiveFeatures);
            recordId = existing.Value;
        }
        else
        {
            // Insert new character and get the ID
            recordId = await InsertCharacterAsync(tag, heightInches, distinctiveFeatures);
        }
        
        return recordId;
    }

    private int? ParseHeight(string height)
    {
        // Parse "5'6\"" or "5'6" format
        var match = Regex.Match(height, @"(\d+)'(\d+)""?");
        if (match.Success)
        {
            var feet = int.Parse(match.Groups[1].Value);
            var inches = int.Parse(match.Groups[2].Value);
            return feet * 12 + inches;
        }
        
        // Try parsing as plain inches
        if (int.TryParse(height, out var plainInches))
            return plainInches;
            
        return null;
    }

    private async Task<int> InsertCharacterAsync(CanonicalTag tag, int? heightInches, string? distinctiveFeatures)
    {
        return await _db.ExecuteScalarAsync<int>(@"
            INSERT INTO characters (
                full_name, preferred_name, age, birthday, birth_year,
                height_inches, weight_lbs, build, hair_color, hair_length, eye_color,
                distinctive_features, occupation, employer, job_title, work_schedule_type,
                character_type, source_file, updated_at
            ) VALUES (
                @FullName, @PreferredName, @Age, @Birthday, @BirthYear,
                @HeightInches, @WeightLbs, @Build, @HairColor, @HairLength, @EyeColor,
                @DistinctiveFeatures, @Occupation, @Employer, @JobTitle, @WorkScheduleType,
                @CharacterType, @SourceFile, @UpdatedAt
            );
            SELECT last_insert_rowid();",
            new
            {
                FullName = tag.Fields.GetValueOrDefault("name"),
                PreferredName = tag.Fields.GetValueOrDefault("preferred_name"),
                Age = tag.Fields.TryGetValue("age", out var age) ? int.TryParse(age, out var a) ? a : (int?)null : null,
                Birthday = tag.Fields.GetValueOrDefault("birthday"),
                BirthYear = tag.Fields.TryGetValue("birth_year", out var by) ? int.TryParse(by, out var b) ? b : (int?)null : null,
                HeightInches = heightInches,
                WeightLbs = tag.Fields.TryGetValue("weight_lbs", out var wt) ? int.TryParse(wt, out var w) ? w : (int?)null : null,
                Build = tag.Fields.GetValueOrDefault("build"),
                HairColor = tag.Fields.GetValueOrDefault("hair_color"),
                HairLength = tag.Fields.GetValueOrDefault("hair_length"),
                EyeColor = tag.Fields.GetValueOrDefault("eye_color"),
                DistinctiveFeatures = distinctiveFeatures,
                Occupation = tag.Fields.GetValueOrDefault("occupation"),
                Employer = tag.Fields.GetValueOrDefault("employer"),
                JobTitle = tag.Fields.GetValueOrDefault("job_title"),
                WorkScheduleType = tag.Fields.GetValueOrDefault("work_schedule_type"),
                CharacterType = tag.Fields.GetValueOrDefault("character_type", "secondary"),
                SourceFile = tag.SourceFile,
                UpdatedAt = DateTime.UtcNow.ToString("o")
            });
    }

    private async Task UpdateCharacterAsync(int id, CanonicalTag tag, int? heightInches, string? distinctiveFeatures)
    {
        // Build dynamic update query with only provided fields
        var updates = new List<string>();
        var parameters = new Dictionary<string, object?> { ["Id"] = id };

        void AddIfPresent(string tagField, string dbColumn, object? parsedValue = null)
        {
            if (tag.Fields.TryGetValue(tagField, out var value))
            {
                updates.Add($"{dbColumn} = @{dbColumn}");
                parameters[dbColumn] = parsedValue ?? value;
            }
        }

        AddIfPresent("preferred_name", "preferred_name");
        AddIfPresent("age", "age", tag.Fields.TryGetValue("age", out var age) ? int.TryParse(age, out var a) ? a : null : null);
        AddIfPresent("birthday", "birthday");
        AddIfPresent("birth_year", "birth_year", tag.Fields.TryGetValue("birth_year", out var by) ? int.TryParse(by, out var b) ? b : null : null);
        
        if (heightInches.HasValue)
        {
            updates.Add("height_inches = @height_inches");
            parameters["height_inches"] = heightInches;
        }
        
        // ... add other fields similarly

        if (updates.Count == 0) return;

        updates.Add("updated_at = @updated_at");
        updates.Add("source_file = @source_file");
        parameters["updated_at"] = DateTime.UtcNow.ToString("o");
        parameters["source_file"] = tag.SourceFile;

        var sql = $"UPDATE characters SET {string.Join(", ", updates)} WHERE id = @Id";
        await _db.ExecuteAsync(sql, parameters);
    }
}
```

#### NegativeProcessor.cs (Critical Example)

```csharp
public class NegativeProcessor : ITagProcessor
{
    private readonly DatabaseContext _db;

    public NegativeProcessor(DatabaseContext db)
    {
        _db = db;
    }

    public async Task<int> ProcessAsync(CanonicalTag tag)
    {
        // Validate required fields
        if (!tag.Fields.TryGetValue("character", out var characterName))
            throw new ValidationException("Negative tag missing required 'character' field");
        if (!tag.Fields.TryGetValue("category", out var category))
            throw new ValidationException("Negative tag missing required 'category' field");
        if (!tag.Fields.TryGetValue("behavior", out var behavior))
            throw new ValidationException("Negative tag missing required 'behavior' field");
        if (!tag.Fields.TryGetValue("strength", out var strength))
            throw new ValidationException("Negative tag missing required 'strength' field");

        // Validate strength value
        var validStrengths = new[] { "absolute", "strong", "preference" };
        if (!validStrengths.Contains(strength.ToLowerInvariant()))
        {
            throw new ValidationException($"Invalid strength '{strength}'. Must be: absolute, strong, or preference");
        }

        // Validate category (warn only)
        var validCategories = new[] { "exercise", "food", "social", "work", "behavior" };
        if (!validCategories.Contains(category.ToLowerInvariant()))
        {
            Console.WriteLine($"  WARNING: Non-standard category '{category}' at {tag.SourceFile}:{tag.LineNumber}");
        }

        // Lookup character
        var characterId = await _db.QuerySingleOrDefaultAsync<int?>(
            "SELECT id FROM characters WHERE full_name = @Name OR preferred_name = @Name",
            new { Name = characterName });

        if (!characterId.HasValue)
        {
            throw new ValidationException($"Character '{characterName}' not found. Process character tags first.");
        }

        // Parse exceptions as JSON array
        string? exceptions = null;
        if (tag.Fields.TryGetValue("exceptions", out var exceptionsStr))
        {
            var exceptionList = exceptionsStr.Split(';', ',')
                .Select(e => e.Trim())
                .Where(e => !string.IsNullOrEmpty(e))
                .ToArray();
            exceptions = JsonSerializer.Serialize(exceptionList);
        }

        // Upsert: match on character_id + behavior
        var existingId = await _db.QuerySingleOrDefaultAsync<int?>(
            "SELECT id FROM character_negatives WHERE character_id = @CharId AND negative_behavior = @Behavior",
            new { CharId = characterId, Behavior = behavior });

        int recordId;
        if (existingId.HasValue)
        {
            await _db.ExecuteAsync(@"
                UPDATE character_negatives SET
                    negative_category = @Category,
                    strength = @Strength,
                    explanation = @Explanation,
                    exception_conditions = @Exceptions,
                    source_file = @SourceFile
                WHERE id = @Id",
                new
                {
                    Id = existingId,
                    Category = category.ToLowerInvariant(),
                    Strength = strength.ToLowerInvariant(),
                    Explanation = tag.Fields.GetValueOrDefault("explanation"),
                    Exceptions = exceptions,
                    SourceFile = tag.SourceFile
                });
            recordId = existingId.Value;
        }
        else
        {
            recordId = await _db.ExecuteScalarAsync<int>(@"
                INSERT INTO character_negatives (
                    character_id, negative_category, negative_behavior, strength,
                    explanation, exception_conditions, source_file
                ) VALUES (
                    @CharacterId, @Category, @Behavior, @Strength,
                    @Explanation, @Exceptions, @SourceFile
                );
                SELECT last_insert_rowid();",
                new
                {
                    CharacterId = characterId,
                    Category = category.ToLowerInvariant(),
                    Behavior = behavior,
                    Strength = strength.ToLowerInvariant(),
                    Explanation = tag.Fields.GetValueOrDefault("explanation"),
                    Exceptions = exceptions,
                    SourceFile = tag.SourceFile
                });
        }
        
        return recordId;
    }
}
```

---

## Usage

### Command Line

```bash
# Full rebuild
dotnet run -- --source /path/to/markdown/files --database /path/to/canonical_facts.db --mode full

# Incremental update (default)
dotnet run -- --source /path/to/markdown/files --database /path/to/canonical_facts.db

# Purge records from deleted files (files that no longer exist in source directory)
dotnet run -- --source /path/to/markdown/files --database /path/to/canonical_facts.db --purge-deleted

# Or after building
./CanonicalFactsIngestion --source ./project-files --database ./data/canonical_facts.db --mode full
```

### Handling Deleted Files

When a markdown file is completely deleted from the project, its records remain in the database until explicitly purged. Use `--purge-deleted` to remove records from files that no longer exist:

```csharp
public async Task PurgeDeletedFilesAsync(string sourceDirectory)
{
    Console.WriteLine("Checking for records from deleted files...");
    
    // Get all unique source files currently in database
    var dbSourceFiles = await _db.QueryAsync<string>(
        "SELECT DISTINCT source_file FROM characters " +
        "UNION SELECT DISTINCT source_file FROM locations " +
        "UNION SELECT DISTINCT source_file FROM character_negatives " +
        "UNION SELECT DISTINCT source_file FROM schedules " +
        "UNION SELECT DISTINCT source_file FROM education " +
        "UNION SELECT DISTINCT source_file FROM relationships " +
        "UNION SELECT DISTINCT source_file FROM possessions " +
        "UNION SELECT DISTINCT source_file FROM timeline_events " +
        "WHERE source_file IS NOT NULL");
    
    foreach (var sourceFile in dbSourceFiles)
    {
        if (!File.Exists(sourceFile))
        {
            Console.WriteLine($"  File deleted: {sourceFile}");
            await DeleteAllRecordsFromFileAsync(sourceFile);
        }
    }
}

private async Task DeleteAllRecordsFromFileAsync(string filePath)
{
    // Delete in reverse dependency order
    var tables = new[]
    {
        "event_participants", "timeline_events", "possessions",
        "relationships", "education", "character_negatives", "schedules",
        "location_rooms", "locations", "characters"
    };
    
    foreach (var table in tables)
    {
        var deleted = await _db.ExecuteAsync(
            $"DELETE FROM {table} WHERE source_file = @FilePath",
            new { FilePath = filePath });
        if (deleted > 0)
            Console.WriteLine($"    Deleted {deleted} records from {table}");
    }
}
```

### Expected Output

```
Starting full rebuild from /path/to/markdown/files
Processing: /path/to/kate-morrison-character-background.md
  Processed: character from kate-morrison-character-background.md:15
  Processed: character from kate-morrison-character-background.md:42
Processing: /path/to/kate-morrison-static-preferences-reference.md
  Processed: negative from kate-morrison-static-preferences-reference.md:28
  Processed: negative from kate-morrison-static-preferences-reference.md:36
  Processed: schedule from kate-morrison-static-preferences-reference.md:55
  WARNING: Non-standard category 'lifestyle' at kate-morrison-static-preferences-reference.md:72
Processing: /path/to/paul-house-skokie.md
  Processed: location from paul-house-skokie.md:8
  Processed: room from paul-house-skokie.md:45
  Processed: room from paul-house-skokie.md:62
Full rebuild complete.
```

---

## Error Handling

### Orphan Cleanup (File Update Handling)

When a file is re-ingested (modified since last run), the system handles removed tags:

**Scenario:**
1. `kate-morrison-preferences.md` contains 3 negative tags
2. Initial ingestion creates 3 records in `character_negatives` with `source_file = "kate-morrison-preferences.md"`
3. User removes one negative tag from the file
4. File is re-ingested (incremental or full rebuild)
5. System detects that only 2 tags exist now, but 3 records exist from this file
6. System deletes the orphaned record

**Implementation:**
- Every table has a `source_file` column tracking origin
- Before processing a file, query all record IDs from that source file
- After processing, compare touched IDs vs pre-existing IDs
- Delete any records that existed before but weren't touched (orphans)
- Delete in reverse dependency order to respect foreign keys

**Output Example:**
```
Processing: /path/to/kate-morrison-preferences.md
  Processed: negative (id=5) from kate-morrison-preferences.md:28
  Processed: negative (id=6) from kate-morrison-preferences.md:42
  Deleted orphaned character_negatives record (id=7) - tag removed from file
```

### Validation Errors

- Missing required fields → Skip tag, log error with file:line
- Invalid enum values → Skip tag, log error
- Character/location not found → Skip tag, log error (suggest processing order)

### Data Integrity

- Foreign key violations caught by SQLite
- Duplicate handling via upsert logic
- Source file tracking for debugging

### Recovery

- Full rebuild always available
- Individual tags can be skipped without affecting others
- Transaction per file (optional, for consistency)

---

## Testing

### Test Cases to Implement

1. **Block tag parsing** - Multi-line format with various field types
2. **Inline tag parsing** - Single-line pipe-delimited format
3. **Height parsing** - "5'6\"", "5'6", "66" formats
4. **Days parsing** - "daily", "weekdays", "monday,tuesday,friday"
5. **JSON array fields** - distinctive_features, exceptions, furniture
6. **Character lookup** - By full_name and preferred_name
7. **Upsert logic** - Update existing vs insert new
8. **Processing order** - Characters before schedules
9. **Missing required fields** - Validation errors
10. **Invalid enum values** - Validation errors
11. **Orphan cleanup - tag removed** - Record deleted when tag removed from file
12. **Orphan cleanup - file deleted** - All records from file deleted when file removed
13. **Orphan cleanup - foreign keys** - Dependent records deleted before parent records
14. **Source file tracking** - Records correctly track their source file
15. **Incremental update** - Only modified files processed
16. **Education tag processing** - Degree type, advisor, in-progress vs completed
17. **Case-insensitive character lookup** - "Kate Morrison" matches "kate morrison"
18. **Empty field values** - Skipped, don't clear existing values
19. **Cross-file references** - Schedule in File A references character in File B
20. **Conflict detection** - Same entity in multiple files with different values

### Test Data

Create test markdown files with known canonical tags, run ingestion, verify database contents match expectations.

---

## Additional Implementation Details

### Case-Insensitive Character Lookup

Character lookups should be case-insensitive to handle variations like "Kate Morrison" vs "kate morrison":

```csharp
// In all processors that look up characters
var characterId = await _db.QuerySingleOrDefaultAsync<int?>(
    "SELECT id FROM characters WHERE LOWER(full_name) = LOWER(@Name) OR LOWER(preferred_name) = LOWER(@Name)",
    new { Name = characterName });
```

### Cross-File Reference Handling

When a tag in File A references an entity defined in File B:

1. **During initial full rebuild**: Processing order ensures dependencies are created first
2. **During incremental update**: Lookups query the database, not just current processing batch
3. **If reference not found**: Log error, skip tag, continue processing other tags

```csharp
// Character lookup always queries database, not in-memory cache
var characterId = await _db.QuerySingleOrDefaultAsync<int?>(
    "SELECT id FROM characters WHERE full_name = @Name OR preferred_name = @Name",
    new { Name = characterName });

if (!characterId.HasValue)
{
    Console.WriteLine($"  ERROR: Character '{characterName}' not found. " +
        "Ensure character is defined in a file that has been processed.");
    throw new ValidationException($"Character '{characterName}' not found");
}
```

### Empty Field Value Handling

Empty or whitespace-only field values should be treated as "not provided" rather than clearing existing values:

```csharp
// In field extraction
var fieldValue = fieldMatch.Groups[2].Value.Trim();
if (string.IsNullOrWhiteSpace(fieldValue))
    continue;  // Skip empty values, don't add to Fields dictionary

// In update logic - only update fields that were provided
void AddIfPresent(string tagField, string dbColumn, object? parsedValue = null)
{
    if (tag.Fields.TryGetValue(tagField, out var value) && !string.IsNullOrWhiteSpace(value))
    {
        updates.Add($"{dbColumn} = @{dbColumn}");
        parameters[dbColumn] = parsedValue ?? value;
    }
}
```

### Conflict Detection

Detect when the same entity is defined in multiple files with different values:

```csharp
public class ConflictDetector
{
    public async Task<List<Conflict>> DetectConflictsAsync()
    {
        var conflicts = new List<Conflict>();
        
        // Check for characters defined in multiple files
        var duplicateChars = await _db.QueryAsync<(string Name, int Count)>(@"
            SELECT full_name, COUNT(DISTINCT source_file) as cnt 
            FROM characters 
            GROUP BY full_name 
            HAVING cnt > 1");
        
        foreach (var dup in duplicateChars)
        {
            var sources = await _db.QueryAsync<string>(
                "SELECT DISTINCT source_file FROM characters WHERE full_name = @Name",
                new { dup.Name });
            conflicts.Add(new Conflict("character", dup.Name, sources.ToList()));
        }
        
        // Similar checks for other entity types...
        return conflicts;
    }
}
```

### Dry Run Mode

Preview what would change without applying:

```csharp
var dryRunOption = new Option<bool>(
    "--dry-run",
    getDefaultValue: () => false,
    "Preview changes without applying them"
);

// In IngestionEngine
public bool DryRun { get; set; }

// In processors, check before writing
if (_engine.DryRun)
{
    Console.WriteLine($"  [DRY RUN] Would insert character: {name}");
    return -1;  // Return dummy ID
}
```

### Validation Summary Report

Print summary at end of run:

```csharp
public class IngestionSummary
{
    public int FilesProcessed { get; set; }
    public int TagsProcessed { get; set; }
    public int RecordsInserted { get; set; }
    public int RecordsUpdated { get; set; }
    public int RecordsDeleted { get; set; }
    public int Errors { get; set; }
    public int Warnings { get; set; }
    public List<string> ErrorMessages { get; } = new();
    public List<string> WarningMessages { get; } = new();
    
    public void Print()
    {
        Console.WriteLine("\n=== INGESTION SUMMARY ===");
        Console.WriteLine($"Files processed:    {FilesProcessed}");
        Console.WriteLine($"Tags processed:     {TagsProcessed}");
        Console.WriteLine($"Records inserted:   {RecordsInserted}");
        Console.WriteLine($"Records updated:    {RecordsUpdated}");
        Console.WriteLine($"Records deleted:    {RecordsDeleted}");
        Console.WriteLine($"Errors:             {Errors}");
        Console.WriteLine($"Warnings:           {Warnings}");
        
        if (ErrorMessages.Any())
        {
            Console.WriteLine("\nERRORS:");
            foreach (var msg in ErrorMessages)
                Console.WriteLine($"  - {msg}");
        }
        
        if (WarningMessages.Any())
        {
            Console.WriteLine("\nWARNINGS:");
            foreach (var msg in WarningMessages)
                Console.WriteLine($"  - {msg}");
        }
    }
}
```

### Ingestion Metadata Table

Track ingestion runs for incremental updates and auditing:

```sql
CREATE TABLE IF NOT EXISTS ingestion_log (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    run_timestamp TEXT NOT NULL,
    mode TEXT NOT NULL,  -- 'full' or 'incremental'
    files_processed INTEGER,
    tags_processed INTEGER,
    records_inserted INTEGER,
    records_updated INTEGER,
    records_deleted INTEGER,
    errors INTEGER,
    duration_seconds REAL,
    source_directory TEXT
);
```

### Date Validation

Validate date fields before insertion:

```csharp
private bool IsValidDate(string dateStr, out string? normalizedDate)
{
    normalizedDate = null;
    
    // Try YYYY-MM-DD
    if (DateTime.TryParseExact(dateStr, "yyyy-MM-dd", null, 
        DateTimeStyles.None, out var date))
    {
        normalizedDate = date.ToString("yyyy-MM-dd");
        return true;
    }
    
    // Try YYYY-MM
    if (DateTime.TryParseExact(dateStr, "yyyy-MM", null,
        DateTimeStyles.None, out date))
    {
        normalizedDate = date.ToString("yyyy-MM");
        return true;
    }
    
    // Try YYYY
    if (int.TryParse(dateStr, out var year) && year >= 1900 && year <= 2100)
    {
        normalizedDate = year.ToString();
        return true;
    }
    
    return false;
}
```

### Schema Versioning

Track database schema version for migrations:

```sql
CREATE TABLE IF NOT EXISTS schema_version (
    version INTEGER PRIMARY KEY,
    applied_at TEXT NOT NULL,
    description TEXT
);

-- Insert initial version
INSERT INTO schema_version (version, applied_at, description)
VALUES (1, datetime('now'), 'Initial schema');
```

```csharp
public async Task EnsureSchemaAsync()
{
    var currentVersion = await GetSchemaVersionAsync();
    
    if (currentVersion < 1)
        await ApplyMigration1Async();
    if (currentVersion < 2)
        await ApplyMigration2Async();
    // etc.
}
```

---

## Dependencies

```xml
<ItemGroup>
  <PackageReference Include="System.CommandLine" Version="2.0.0-beta4.22272.1" />
  <PackageReference Include="Microsoft.Data.Sqlite" Version="8.0.0" />
  <PackageReference Include="Dapper" Version="2.1.28" />
  <PackageReference Include="System.Text.Json" Version="8.0.0" />
</ItemGroup>
```

---

## Future Enhancements

1. **Watch mode**: Monitor directory for changes, auto-ingest
2. **HTML validation report**: Generate visual report of all tags and issues
3. **Export**: Generate SQL dump or JSON from database
4. **Pre-flight validation**: Parse all files and validate references before any writes
5. **Rollback support**: Transaction-based processing with rollback on failure
6. **Parallel processing**: Process independent files concurrently
7. **Tag linting**: Warn about common mistakes (missing "Does NOT" prefix on negatives, etc.)
8. **Duplicate tag detection**: Warn when same tag appears twice in same file
