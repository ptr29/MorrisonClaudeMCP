using KateMorrisonMCP.Data;
using KateMorrisonMCP.Ingestion.Parsing;
using KateMorrisonMCP.Ingestion.Processors;
using KateMorrisonMCP.Ingestion.SchemaManagement;
using Xunit;

namespace KateMorrisonMCP.Tests;

public class CharacterProcessorTests : IAsyncDisposable
{
    private readonly DatabaseContext _db;
    private readonly string _testDbPath;
    private readonly CharacterProcessor _processor;

    public CharacterProcessorTests()
    {
        _testDbPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.db");
        _db = new DatabaseContext(_testDbPath);
        _processor = new CharacterProcessor(_db);
    }

    public async ValueTask DisposeAsync()
    {
        if (File.Exists(_testDbPath))
            File.Delete(_testDbPath);
        await Task.CompletedTask;
    }

    [Fact]
    public async Task ProcessAsync_NewCharacter_InsertsSuccessfully()
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);
        await new SchemaUpdater(_db).EnsureSourceFileColumnsAsync();

        var tag = new CanonicalTag
        {
            Type = "character",
            SourceFile = "test.md",
            LineNumber = 1,
            Fields = new Dictionary<string, string>
            {
                ["name"] = "Kate Morrison",
                ["age"] = "29",
                ["height"] = "5'6\"",
                ["occupation"] = "Private Investigator"
            }
        };

        // Act
        var recordId = await _processor.ProcessAsync(tag);

        // Assert
        Assert.True(recordId > 0);

        var character = await _db.QuerySingleOrDefaultAsync<dynamic>(
            "SELECT * FROM characters WHERE id = @Id",
            new { Id = recordId });

        Assert.NotNull(character);
        Assert.Equal("Kate Morrison", character.full_name);
        Assert.Equal(29, character.age);
        Assert.Equal(66, character.height_inches); // 5'6" = 66 inches
        Assert.Equal("Private Investigator", character.occupation);
        Assert.Equal("test.md", character.source_file);
    }

    [Fact]
    public async Task ProcessAsync_ExistingCharacter_UpdatesSuccessfully()
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);
        await new SchemaUpdater(_db).EnsureSourceFileColumnsAsync();

        // Insert initial character
        await _db.ExecuteAsync(@"
            INSERT INTO characters (full_name, age, occupation, source_file, created_at, updated_at)
            VALUES ('Kate Morrison', 28, 'Detective', 'test.md', datetime('now'), datetime('now'))");

        var tag = new CanonicalTag
        {
            Type = "character",
            SourceFile = "test.md",
            LineNumber = 1,
            Fields = new Dictionary<string, string>
            {
                ["name"] = "Kate Morrison",
                ["age"] = "29",
                ["occupation"] = "Private Investigator"
            }
        };

        // Act
        var recordId = await _processor.ProcessAsync(tag);

        // Assert
        var character = await _db.QuerySingleOrDefaultAsync<dynamic>(
            "SELECT * FROM characters WHERE id = @Id",
            new { Id = recordId });

        Assert.NotNull(character);
        Assert.Equal("Kate Morrison", character.full_name);
        Assert.Equal(29, character.age); // Updated
        Assert.Equal("Private Investigator", character.occupation); // Updated
    }

    [Fact]
    public async Task ProcessAsync_DistinctiveFeatures_ParsesAsJsonArray()
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);
        await new SchemaUpdater(_db).EnsureSourceFileColumnsAsync();

        var tag = new CanonicalTag
        {
            Type = "character",
            SourceFile = "test.md",
            LineNumber = 1,
            Fields = new Dictionary<string, string>
            {
                ["name"] = "Kate Morrison",
                ["distinctive_features"] = "red eyes, tall, athletic"
            }
        };

        // Act
        var recordId = await _processor.ProcessAsync(tag);

        // Assert
        var character = await _db.QuerySingleOrDefaultAsync<dynamic>(
            "SELECT distinctive_features FROM characters WHERE id = @Id",
            new { Id = recordId });

        Assert.NotNull(character);
        var features = (string)character.distinctive_features;
        Assert.Contains("red eyes", features);
        Assert.Contains("tall", features);
        Assert.Contains("athletic", features);
    }

    [Fact]
    public async Task ProcessAsync_MissingRequiredField_ThrowsException()
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);

        var tag = new CanonicalTag
        {
            Type = "character",
            SourceFile = "test.md",
            LineNumber = 1,
            Fields = new Dictionary<string, string>
            {
                ["age"] = "29" // Missing required "name" field
            }
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _processor.ProcessAsync(tag));
    }
}
