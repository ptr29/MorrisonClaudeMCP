using KateMorrisonMCP.Data;
using KateMorrisonMCP.Ingestion.Engine;
using KateMorrisonMCP.Ingestion.SchemaManagement;
using Xunit;

namespace KateMorrisonMCP.Tests;

/// <summary>
/// Critical tests for the orphan cleanup algorithm
/// Ensures that database records are deleted when canonical tags are removed from files
/// </summary>
public class OrphanCleanupTests : IAsyncDisposable
{
    private readonly DatabaseContext _db;
    private readonly string _testDbPath;
    private readonly string _testDir;

    public OrphanCleanupTests()
    {
        _testDbPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.db");
        _testDir = Path.Combine(Path.GetTempPath(), $"test_md_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDir);
        _db = new DatabaseContext(_testDbPath);
    }

    public async ValueTask DisposeAsync()
    {
        if (File.Exists(_testDbPath))
            File.Delete(_testDbPath);
        if (Directory.Exists(_testDir))
            Directory.Delete(_testDir, true);
        await Task.CompletedTask;
    }

    [Fact]
    public async Task ProcessFile_TagRemoved_DeletesOrphanedRecord()
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);
        await new SchemaUpdater(_db).EnsureSourceFileColumnsAsync();

        var testFile = Path.Combine(_testDir, "test.md");

        // Initial file with 3 character tags
        File.WriteAllText(testFile, @"
<!-- canonical: character
name: Kate Morrison
age: 29
-->

<!-- canonical: character
name: Sarah Chen
age: 32
-->

<!-- canonical: character
name: Mike Davis
age: 35
-->
");

        var engine = new IngestionEngine(_db, verbose: false);

        // First ingestion - should create 3 characters
        var result1 = await engine.ProcessDirectoryAsync(_testDir);

        Assert.Equal(3, result1.RecordsInserted);
        Assert.Equal(0, result1.RecordsDeleted);

        // Verify 3 characters exist
        var count1 = await _db.QuerySingleOrDefaultAsync<int>(
            "SELECT COUNT(*) FROM characters WHERE source_file = @SourceFile",
            new { SourceFile = testFile });
        Assert.Equal(3, count1);

        // Act - Remove Sarah Chen tag
        File.WriteAllText(testFile, @"
<!-- canonical: character
name: Kate Morrison
age: 29
-->

<!-- REMOVED Sarah Chen -->

<!-- canonical: character
name: Mike Davis
age: 35
-->
");

        var result2 = await engine.ProcessDirectoryAsync(_testDir);

        // Assert
        Assert.Equal(0, result2.RecordsInserted);
        Assert.Equal(2, result2.RecordsUpdated); // Kate and Mike updated
        Assert.Equal(1, result2.RecordsDeleted); // Sarah deleted

        // Verify only 2 characters remain
        var count2 = await _db.QuerySingleOrDefaultAsync<int>(
            "SELECT COUNT(*) FROM characters WHERE source_file = @SourceFile",
            new { SourceFile = testFile });
        Assert.Equal(2, count2);

        // Verify Sarah is gone, Kate and Mike remain
        var kate = await _db.QuerySingleOrDefaultAsync<dynamic>(
            "SELECT * FROM characters WHERE full_name = 'Kate Morrison'");
        Assert.NotNull(kate);

        var mike = await _db.QuerySingleOrDefaultAsync<dynamic>(
            "SELECT * FROM characters WHERE full_name = 'Mike Davis'");
        Assert.NotNull(mike);

        var sarah = await _db.QuerySingleOrDefaultAsync<dynamic>(
            "SELECT * FROM characters WHERE full_name = 'Sarah Chen'");
        Assert.Null(sarah);
    }

    [Fact]
    public async Task ProcessFile_AllTagsRemoved_DeletesAllRecords()
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);
        await new SchemaUpdater(_db).EnsureSourceFileColumnsAsync();

        var testFile = Path.Combine(_testDir, "test.md");

        // Initial file with characters
        File.WriteAllText(testFile, @"
<!-- canonical: character
name: Kate Morrison
age: 29
-->

<!-- canonical: character
name: Sarah Chen
age: 32
-->
");

        var engine = new IngestionEngine(_db, verbose: false);
        var result1 = await engine.ProcessDirectoryAsync(_testDir);

        Assert.Equal(2, result1.RecordsInserted);

        // Act - Remove all canonical tags
        File.WriteAllText(testFile, @"
# Just a regular markdown file now

No canonical tags here.
");

        var result2 = await engine.ProcessDirectoryAsync(_testDir);

        // Assert
        Assert.Equal(0, result2.RecordsInserted);
        Assert.Equal(0, result2.RecordsUpdated);
        Assert.Equal(2, result2.RecordsDeleted);

        var count = await _db.QuerySingleOrDefaultAsync<int>(
            "SELECT COUNT(*) FROM characters WHERE source_file = @SourceFile",
            new { SourceFile = testFile });
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task ProcessFile_DependentRecords_DeletesInCorrectOrder()
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);
        await new SchemaUpdater(_db).EnsureSourceFileColumnsAsync();

        var testFile = Path.Combine(_testDir, "test.md");

        // Initial file with character and negative
        File.WriteAllText(testFile, @"
<!-- canonical: character
name: Kate Morrison
age: 29
-->

<!-- canonical: negative
character: Kate Morrison
negative_behavior: Does NOT go to gyms
strength: absolute
category: exercise
-->
");

        var engine = new IngestionEngine(_db, verbose: false);
        var result1 = await engine.ProcessDirectoryAsync(_testDir);

        Assert.Equal(2, result1.RecordsInserted); // Character + negative

        // Act - Remove negative tag
        File.WriteAllText(testFile, @"
<!-- canonical: character
name: Kate Morrison
age: 29
-->

<!-- Removed negative tag -->
");

        var result2 = await engine.ProcessDirectoryAsync(_testDir);

        // Assert
        Assert.Equal(1, result2.RecordsDeleted); // Negative deleted

        // Character still exists
        var character = await _db.QuerySingleOrDefaultAsync<dynamic>(
            "SELECT * FROM characters WHERE full_name = 'Kate Morrison'");
        Assert.NotNull(character);

        // Negative is gone
        var negativeCount = await _db.QuerySingleOrDefaultAsync<int>(
            "SELECT COUNT(*) FROM character_negatives WHERE character_id = @CharacterId",
            new { CharacterId = character.id });
        Assert.Equal(0, negativeCount);
    }

    [Fact]
    public async Task ProcessFile_MultipleFilesSameDatabase_OrphansOnlyAffectSourceFile()
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);
        await new SchemaUpdater(_db).EnsureSourceFileColumnsAsync();

        var file1 = Path.Combine(_testDir, "file1.md");
        var file2 = Path.Combine(_testDir, "file2.md");

        // File 1 has Kate
        File.WriteAllText(file1, @"
<!-- canonical: character
name: Kate Morrison
age: 29
-->
");

        // File 2 has Sarah
        File.WriteAllText(file2, @"
<!-- canonical: character
name: Sarah Chen
age: 32
-->
");

        var engine = new IngestionEngine(_db, verbose: false);
        var result1 = await engine.ProcessDirectoryAsync(_testDir);

        Assert.Equal(2, result1.RecordsInserted);

        // Act - Remove Kate from file1, but leave file2 unchanged
        File.WriteAllText(file1, "# Empty file now");

        var result2 = await engine.ProcessDirectoryAsync(_testDir);

        // Assert
        Assert.Equal(1, result2.RecordsDeleted); // Only Kate deleted

        // Sarah still exists (from file2)
        var sarah = await _db.QuerySingleOrDefaultAsync<dynamic>(
            "SELECT * FROM characters WHERE full_name = 'Sarah Chen'");
        Assert.NotNull(sarah);

        // Kate is gone
        var kate = await _db.QuerySingleOrDefaultAsync<dynamic>(
            "SELECT * FROM characters WHERE full_name = 'Kate Morrison'");
        Assert.Null(kate);
    }

    [Fact]
    public async Task ProcessFile_TagMovedToUpdate_UpdatesCorrectly()
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);
        await new SchemaUpdater(_db).EnsureSourceFileColumnsAsync();

        var testFile = Path.Combine(_testDir, "test.md");

        File.WriteAllText(testFile, @"
<!-- canonical: character
name: Kate Morrison
age: 29
occupation: Detective
-->
");

        var engine = new IngestionEngine(_db, verbose: false);
        await engine.ProcessDirectoryAsync(_testDir);

        // Act - Update the tag (change age and occupation)
        File.WriteAllText(testFile, @"
<!-- canonical: character
name: Kate Morrison
age: 30
occupation: Private Investigator
-->
");

        var result = await engine.ProcessDirectoryAsync(_testDir);

        // Assert
        Assert.Equal(0, result.RecordsInserted);
        Assert.Equal(1, result.RecordsUpdated);
        Assert.Equal(0, result.RecordsDeleted);

        var character = await _db.QuerySingleOrDefaultAsync<dynamic>(
            "SELECT * FROM characters WHERE full_name = 'Kate Morrison'");

        Assert.NotNull(character);
        Assert.Equal(30, character.age);
        Assert.Equal("Private Investigator", character.occupation);
    }
}
