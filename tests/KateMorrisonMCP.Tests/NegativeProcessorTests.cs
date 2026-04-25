using KateMorrisonMCP.Data;
using KateMorrisonMCP.Ingestion.Helpers;
using KateMorrisonMCP.Ingestion.Parsing;
using KateMorrisonMCP.Ingestion.Processors;
using KateMorrisonMCP.Ingestion.SchemaManagement;
using Xunit;

namespace KateMorrisonMCP.Tests;

public class NegativeProcessorTests : IAsyncDisposable
{
    private readonly DatabaseContext _db;
    private readonly string _testDbPath;
    private readonly NegativeProcessor _processor;

    public NegativeProcessorTests()
    {
        _testDbPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.db");
        _db = new DatabaseContext(_testDbPath);
        var characterLookup = new CharacterLookup(_db);
        _processor = new NegativeProcessor(_db, characterLookup);
    }

    public async ValueTask DisposeAsync()
    {
        if (File.Exists(_testDbPath))
            File.Delete(_testDbPath);
        await Task.CompletedTask;
    }

    [Fact]
    public async Task ProcessAsync_ValidNegative_InsertsSuccessfully()
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);
        await new SchemaUpdater(_db).EnsureSourceFileColumnsAsync();

        // Create character first
        await _db.ExecuteAsync(@"
            INSERT INTO characters (full_name, age, created_at, updated_at)
            VALUES ('Kate Morrison', 29, datetime('now'), datetime('now'))");

        var tag = new CanonicalTag
        {
            Type = "negative",
            SourceFile = "test.md",
            LineNumber = 1,
            Fields = new Dictionary<string, string>
            {
                ["character"] = "Kate Morrison",
                ["negative_behavior"] = "Does NOT go to gyms",
                ["strength"] = "absolute",
                ["category"] = "exercise"
            }
        };

        // Act
        var recordId = await _processor.ProcessAsync(tag);

        // Assert
        Assert.True(recordId > 0);

        var negative = await _db.QuerySingleOrDefaultAsync<dynamic>(
            "SELECT * FROM character_negatives WHERE id = @Id",
            new { Id = recordId });

        Assert.NotNull(negative);
        Assert.Equal("Does NOT go to gyms", negative.negative_behavior);
        Assert.Equal("absolute", negative.strength);
        Assert.Equal("exercise", negative.negative_category);
        Assert.Equal("test.md", negative.source_file);
    }

    [Fact]
    public async Task ProcessAsync_InvalidStrength_ThrowsException()
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);
        await new SchemaUpdater(_db).EnsureSourceFileColumnsAsync();

        await _db.ExecuteAsync(@"
            INSERT INTO characters (full_name, age, created_at, updated_at)
            VALUES ('Kate Morrison', 29, datetime('now'), datetime('now'))");

        var tag = new CanonicalTag
        {
            Type = "negative",
            SourceFile = "test.md",
            LineNumber = 1,
            Fields = new Dictionary<string, string>
            {
                ["character"] = "Kate Morrison",
                ["negative_behavior"] = "Does NOT go to gyms",
                ["strength"] = "invalid_strength", // Invalid
                ["category"] = "exercise"
            }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _processor.ProcessAsync(tag));
        Assert.Contains("strength", exception.Message);
    }

    [Fact]
    public async Task ProcessAsync_MissingStrength_ThrowsException()
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);
        await new SchemaUpdater(_db).EnsureSourceFileColumnsAsync();

        await _db.ExecuteAsync(@"
            INSERT INTO characters (full_name, age, created_at, updated_at)
            VALUES ('Kate Morrison', 29, datetime('now'), datetime('now'))");

        var tag = new CanonicalTag
        {
            Type = "negative",
            SourceFile = "test.md",
            LineNumber = 1,
            Fields = new Dictionary<string, string>
            {
                ["character"] = "Kate Morrison",
                ["negative_behavior"] = "Does NOT go to gyms",
                // Missing strength field
                ["category"] = "exercise"
            }
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _processor.ProcessAsync(tag));
    }

    [Fact]
    public async Task ProcessAsync_CharacterNotFound_ThrowsException()
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);

        // No character created

        var tag = new CanonicalTag
        {
            Type = "negative",
            SourceFile = "test.md",
            LineNumber = 1,
            Fields = new Dictionary<string, string>
            {
                ["character"] = "Nonexistent Character",
                ["negative_behavior"] = "Does NOT go to gyms",
                ["strength"] = "absolute",
                ["category"] = "exercise"
            }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _processor.ProcessAsync(tag));
        Assert.Contains("not found", exception.Message.ToLower());
    }

    [Theory]
    [InlineData("absolute")]
    [InlineData("strong")]
    [InlineData("preference")]
    public async Task ProcessAsync_ValidStrengthValues_Succeed(string strength)
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);
        await new SchemaUpdater(_db).EnsureSourceFileColumnsAsync();

        await _db.ExecuteAsync(@"
            INSERT INTO characters (full_name, age, created_at, updated_at)
            VALUES ('Kate Morrison', 29, datetime('now'), datetime('now'))");

        var tag = new CanonicalTag
        {
            Type = "negative",
            SourceFile = "test.md",
            LineNumber = 1,
            Fields = new Dictionary<string, string>
            {
                ["character"] = "Kate Morrison",
                ["negative_behavior"] = "Does NOT go to gyms",
                ["strength"] = strength,
                ["category"] = "exercise"
            }
        };

        // Act
        var recordId = await _processor.ProcessAsync(tag);

        // Assert
        Assert.True(recordId > 0);

        var negative = await _db.QuerySingleOrDefaultAsync<dynamic>(
            "SELECT strength FROM character_negatives WHERE id = @Id",
            new { Id = recordId });

        Assert.Equal(strength.ToLowerInvariant(), negative.strength);
    }

    [Fact]
    public async Task ProcessAsync_CaseInsensitiveCharacterLookup_FindsCharacter()
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);
        await new SchemaUpdater(_db).EnsureSourceFileColumnsAsync();

        // Character stored as "Kate Morrison"
        await _db.ExecuteAsync(@"
            INSERT INTO characters (full_name, age, created_at, updated_at)
            VALUES ('Kate Morrison', 29, datetime('now'), datetime('now'))");

        var tag = new CanonicalTag
        {
            Type = "negative",
            SourceFile = "test.md",
            LineNumber = 1,
            Fields = new Dictionary<string, string>
            {
                ["character"] = "kate morrison", // Different case
                ["negative_behavior"] = "Does NOT go to gyms",
                ["strength"] = "absolute",
                ["category"] = "exercise"
            }
        };

        // Act
        var recordId = await _processor.ProcessAsync(tag);

        // Assert
        Assert.True(recordId > 0);
    }

    [Fact]
    public async Task ProcessAsync_ExistingNegative_UpdatesSuccessfully()
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);
        await new SchemaUpdater(_db).EnsureSourceFileColumnsAsync();

        // Create character
        await _db.ExecuteAsync(@"
            INSERT INTO characters (full_name, age, created_at, updated_at)
            VALUES ('Kate Morrison', 29, datetime('now'), datetime('now'))");

        var characterId = await _db.QuerySingleOrDefaultAsync<int>(
            "SELECT id FROM characters WHERE full_name = 'Kate Morrison'");

        // Insert initial negative
        await _db.ExecuteAsync(@"
            INSERT INTO character_negatives (character_id, negative_behavior, strength, negative_category, source_file)
            VALUES (@CharacterId, 'Does NOT go to gyms', 'strong', 'exercise', 'test.md')",
            new { CharacterId = characterId });

        var tag = new CanonicalTag
        {
            Type = "negative",
            SourceFile = "test.md",
            LineNumber = 1,
            Fields = new Dictionary<string, string>
            {
                ["character"] = "Kate Morrison",
                ["negative_behavior"] = "Does NOT go to gyms",
                ["strength"] = "absolute", // Updated from "strong"
                ["category"] = "exercise"
            }
        };

        // Act
        var recordId = await _processor.ProcessAsync(tag);

        // Assert
        var negative = await _db.QuerySingleOrDefaultAsync<dynamic>(
            "SELECT * FROM character_negatives WHERE id = @Id",
            new { Id = recordId });

        Assert.NotNull(negative);
        Assert.Equal("absolute", negative.strength); // Updated
    }
}
