using KateMorrisonMCP.Data;
using KateMorrisonMCP.Data.Models;

namespace KateMorrisonMCP.Tests;

/// <summary>
/// Tests for DatabaseContext initialization and health checks
/// </summary>
public class DatabaseContextTests : IAsyncDisposable
{
    private readonly DatabaseContext _db;
    private readonly string _testDbPath;

    public DatabaseContextTests()
    {
        _testDbPath = Path.Combine(Path.GetTempPath(), $"test_db_{Guid.NewGuid()}.db");
        _db = new DatabaseContext(_testDbPath);
    }

    public async ValueTask DisposeAsync()
    {
        if (File.Exists(_testDbPath))
        {
            File.Delete(_testDbPath);
        }
        await Task.CompletedTask;
    }

    [Fact]
    public async Task Initialize_CreatesDatabase()
    {
        // Act
        await TestHelpers.CreateTestSchemaAsync(_db);

        // Assert
        Assert.True(File.Exists(_testDbPath));
    }

    [Fact]
    public async Task HealthCheck_EmptyDatabase_Succeeds()
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);

        // Act
        var health = await _db.PerformHealthCheckAsync();

        // Assert: Health check succeeds even with 0 characters as long as schema is valid
        Assert.True(health.IsHealthy);
        Assert.Contains("0 characters found", health.Message);
        Assert.Contains("All required tables present", health.Message);
    }

    [Fact]
    public async Task HealthCheck_WithCharacters_Succeeds()
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);
        await _db.ExecuteAsync(@"
            INSERT INTO characters (id, full_name, preferred_name)
            VALUES (1, 'Test Kate', 'Kate'), (2, 'Test Paul', 'Paul')");

        // Act
        var health = await _db.PerformHealthCheckAsync();

        // Assert
        Assert.True(health.IsHealthy);
        Assert.Contains("2 characters found", health.Message);
        Assert.Contains("Database connected", health.Message);
    }

    [Fact]
    public async Task HealthCheck_VerifiesForeignKeys()
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);
        await _db.ExecuteAsync(@"
            INSERT INTO characters (id, full_name, preferred_name)
            VALUES (1, 'Test Kate', 'Kate')");

        // Act
        var health = await _db.PerformHealthCheckAsync();

        // Assert
        Assert.True(health.IsHealthy);
        Assert.Contains("Foreign keys enabled", health.Message);
    }

    [Fact]
    public async Task HealthCheck_VerifiesRequiredTables()
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);
        await _db.ExecuteAsync(@"
            INSERT INTO characters (id, full_name, preferred_name)
            VALUES (1, 'Test Kate', 'Kate')");

        // Act
        var health = await _db.PerformHealthCheckAsync();

        // Assert
        Assert.True(health.IsHealthy);
        Assert.Contains("All required tables present", health.Message);
    }

    [Fact]
    public async Task Query_CanReadCharacterNegatives()
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);
        await _db.ExecuteAsync(@"
            INSERT INTO characters (id, full_name, preferred_name)
            VALUES (1, 'Test Kate', 'Kate')");
        await _db.ExecuteAsync(@"
            INSERT INTO character_negatives
            (character_id, negative_category, negative_behavior, strength, explanation)
            VALUES (1, 'exercise', 'Does NOT go to gyms', 'absolute', 'Test explanation')");

        // Act
        var negatives = await _db.QueryAsync<CharacterNegative>(
            "SELECT * FROM character_negatives WHERE character_id = @Id",
            new { Id = 1 });

        // Assert
        var negative = negatives.FirstOrDefault();
        Assert.NotNull(negative);
        Assert.Equal("exercise", negative.NegativeCategory);
        Assert.Equal("Does NOT go to gyms", negative.NegativeBehavior);
        Assert.Equal("absolute", negative.Strength);
    }

    [Fact]
    public async Task Query_ColumnMapping_WorksCorrectly()
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);
        await _db.ExecuteAsync(@"
            INSERT INTO characters (id, full_name, preferred_name)
            VALUES (1, 'Test Kate', 'Kate')");
        await _db.ExecuteAsync(@"
            INSERT INTO character_negatives
            (character_id, negative_category, negative_behavior, strength, explanation)
            VALUES (1, 'exercise', 'Does NOT go to gyms', 'absolute', 'Test explanation')");

        // Act: Query using snake_case columns
        var negatives = await _db.QueryAsync<CharacterNegative>(
            "SELECT id, character_id, negative_category, negative_behavior, strength, explanation FROM character_negatives",
            null);

        // Assert: Verify PascalCase properties are populated from snake_case columns
        var negative = negatives.FirstOrDefault();
        Assert.NotNull(negative);
        Assert.Equal(1, negative.Id);
        Assert.Equal(1, negative.CharacterId);
        Assert.Equal("exercise", negative.NegativeCategory);
        Assert.Equal("Does NOT go to gyms", negative.NegativeBehavior);
        Assert.Equal("absolute", negative.Strength);
        Assert.Equal("Test explanation", negative.Explanation);
    }

    [Fact]
    public async Task Execute_CanInsertData()
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);

        // Act
        var rowsAffected = await _db.ExecuteAsync(@"
            INSERT INTO characters (id, full_name, preferred_name)
            VALUES (1, 'Test Kate', 'Kate')");

        // Assert
        Assert.Equal(1, rowsAffected);

        var characters = await _db.QueryAsync<Character>(
            "SELECT * FROM characters WHERE id = 1", null);
        Assert.Single(characters);
    }
}
