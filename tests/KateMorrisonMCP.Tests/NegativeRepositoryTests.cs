using KateMorrisonMCP.Data;
using KateMorrisonMCP.Data.Repositories;
using KateMorrisonMCP.Data.Models;

namespace KateMorrisonMCP.Tests;

/// <summary>
/// Tests for NegativeRepository semantic matching logic
/// CRITICAL: Tests the verb normalization and keyword matching that prevents false negatives
/// </summary>
public class NegativeRepositoryTests : IAsyncDisposable
{
    private readonly DatabaseContext _db;
    private readonly NegativeRepository _repo;
    private readonly string _testDbPath;

    public NegativeRepositoryTests()
    {
        // Create unique test database for each test run
        _testDbPath = Path.Combine(Path.GetTempPath(), $"test_negatives_{Guid.NewGuid()}.db");
        _db = new DatabaseContext(_testDbPath);
        _repo = new NegativeRepository(_db);
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
    public async Task FindMatchingNegative_VerbConjugation_GoesVsGo()
    {
        // Arrange: Seed test data
        await TestHelpers.CreateTestSchemaAsync(_db);
        await _db.ExecuteAsync(@"
            INSERT INTO characters (id, full_name, preferred_name)
            VALUES (1, 'Test Kate', 'Kate')");
        await _db.ExecuteAsync(@"
            INSERT INTO character_negatives
            (character_id, negative_category, negative_behavior, strength, explanation)
            VALUES (1, 'exercise', 'Does NOT go to gyms', 'absolute', 'Test explanation')");

        // Act: Test that "goes to gym" matches "Does NOT go to gyms"
        var result = await _repo.FindMatchingNegativeAsync(1, "goes to gym");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Does NOT go to gyms", result.NegativeBehavior);
        Assert.Equal("absolute", result.Strength);
    }

    [Fact]
    public async Task FindMatchingNegative_PluralMatching_GymVsGyms()
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

        // Act: Singular "gym" should match plural "gyms"
        var result = await _repo.FindMatchingNegativeAsync(1, "go to gym");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Does NOT go to gyms", result.NegativeBehavior);
    }

    [Fact]
    public async Task FindMatchingNegative_MultipleVerbs_RunsVsRun()
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);
        await _db.ExecuteAsync(@"
            INSERT INTO characters (id, full_name, preferred_name)
            VALUES (1, 'Test Kate', 'Kate')");
        await _db.ExecuteAsync(@"
            INSERT INTO character_negatives
            (character_id, negative_category, negative_behavior, strength, explanation)
            VALUES (1, 'exercise', 'Does NOT run on treadmills', 'strong', 'Test explanation')");

        // Act: "runs on treadmill" should match "Does NOT run on treadmills"
        var result = await _repo.FindMatchingNegativeAsync(1, "runs on treadmill");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Does NOT run on treadmills", result.NegativeBehavior);
    }

    [Fact]
    public async Task FindMatchingNegative_VerbWithEs_EatsVsEat()
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);
        await _db.ExecuteAsync(@"
            INSERT INTO characters (id, full_name, preferred_name)
            VALUES (1, 'Test Kate', 'Kate')");
        await _db.ExecuteAsync(@"
            INSERT INTO character_negatives
            (character_id, negative_category, negative_behavior, strength, explanation)
            VALUES (1, 'food', 'Does NOT eat gluten', 'absolute', 'Celiac disease')");

        // Act: "eats gluten" should match "Does NOT eat gluten"
        var result = await _repo.FindMatchingNegativeAsync(1, "eats gluten");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Does NOT eat gluten", result.NegativeBehavior);
    }

    [Fact]
    public async Task FindMatchingNegative_NoMatch_ReturnNull()
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

        // Act: Completely unrelated behavior
        var result = await _repo.FindMatchingNegativeAsync(1, "eats pizza");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task FindMatchingNegative_SingleKeyword_Matches()
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);
        await _db.ExecuteAsync(@"
            INSERT INTO characters (id, full_name, preferred_name)
            VALUES (1, 'Test Kate', 'Kate')");
        await _db.ExecuteAsync(@"
            INSERT INTO character_negatives
            (character_id, negative_category, negative_behavior, strength, explanation)
            VALUES (1, 'exercise', 'Does NOT do yoga', 'preference', 'Test explanation')");

        // Act: Single keyword "yoga" should match
        var result = await _repo.FindMatchingNegativeAsync(1, "yoga");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Does NOT do yoga", result.NegativeBehavior);
    }

    [Fact]
    public async Task GetByCharacter_ReturnsAllNegatives()
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);
        await _db.ExecuteAsync(@"
            INSERT INTO characters (id, full_name, preferred_name)
            VALUES (1, 'Test Kate', 'Kate')");
        await _db.ExecuteAsync(@"
            INSERT INTO character_negatives
            (character_id, negative_category, negative_behavior, strength, explanation)
            VALUES
            (1, 'exercise', 'Does NOT go to gyms', 'absolute', 'Test 1'),
            (1, 'exercise', 'Does NOT run on treadmills', 'strong', 'Test 2'),
            (1, 'food', 'Does NOT eat gluten', 'absolute', 'Test 3')");

        // Act
        var results = await _repo.GetByCharacterAsync(1);

        // Assert
        Assert.Equal(3, results.Count());
    }

    [Fact]
    public async Task GetByCharacter_FilterByCategory()
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);
        await _db.ExecuteAsync(@"
            INSERT INTO characters (id, full_name, preferred_name)
            VALUES (1, 'Test Kate', 'Kate')");
        await _db.ExecuteAsync(@"
            INSERT INTO character_negatives
            (character_id, negative_category, negative_behavior, strength, explanation)
            VALUES
            (1, 'exercise', 'Does NOT go to gyms', 'absolute', 'Test 1'),
            (1, 'exercise', 'Does NOT run on treadmills', 'strong', 'Test 2'),
            (1, 'food', 'Does NOT eat gluten', 'absolute', 'Test 3')");

        // Act
        var results = await _repo.GetByCharacterAsync(1, "exercise");

        // Assert
        Assert.Equal(2, results.Count());
        Assert.All(results, n => Assert.Equal("exercise", n.NegativeCategory));
    }

    [Fact]
    public async Task FindMatchingNegative_CaseInsensitive()
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

        // Act: Test case insensitivity
        var result = await _repo.FindMatchingNegativeAsync(1, "GOES TO GYM");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Does NOT go to gyms", result.NegativeBehavior);
    }
}
