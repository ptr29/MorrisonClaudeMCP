using System.Text.Json;
using KateMorrisonMCP.Data;
using KateMorrisonMCP.Data.Repositories;
using KateMorrisonMCP.Tools.Tools;

namespace KateMorrisonMCP.Tests;

/// <summary>
/// Tests for CheckNegativeTool - the critical error prevention tool
/// Tests the full end-to-end flow from tool arguments to response
/// </summary>
public class CheckNegativeToolTests : IAsyncDisposable
{
    private readonly DatabaseContext _db;
    private readonly CharacterRepository _characterRepo;
    private readonly NegativeRepository _negativeRepo;
    private readonly CheckNegativeTool _tool;
    private readonly string _testDbPath;

    public CheckNegativeToolTests()
    {
        _testDbPath = Path.Combine(Path.GetTempPath(), $"test_tool_{Guid.NewGuid()}.db");
        _db = new DatabaseContext(_testDbPath);
        _characterRepo = new CharacterRepository(_db);
        _negativeRepo = new NegativeRepository(_db);
        _tool = new CheckNegativeTool(_negativeRepo, _characterRepo);
    }

    public async ValueTask DisposeAsync()
    {
        if (File.Exists(_testDbPath))
        {
            File.Delete(_testDbPath);
        }
        await Task.CompletedTask;
    }

    private async Task SeedTestDataAsync()
    {
        await TestHelpers.CreateTestSchemaAsync(_db);
        await _db.ExecuteAsync(@"
            INSERT INTO characters (id, full_name, preferred_name)
            VALUES (1, 'Katherine Marie Morrison', 'Kate')");
        await _db.ExecuteAsync(@"
            INSERT INTO character_negatives
            (character_id, negative_category, negative_behavior, strength, explanation, exception_conditions)
            VALUES
            (1, 'exercise', 'Does NOT go to gyms', 'absolute',
             'Kate strongly prefers outdoor running and never uses gym facilities', NULL),
            (1, 'exercise', 'Does NOT run on treadmills', 'strong',
             'Kate hates treadmills and finds them unbearable',
             '[""will use treadmill ONLY if temperature is below 15°F (-9°C)""]'),
            (1, 'food', 'Does NOT eat gluten', 'absolute',
             'Kate has celiac disease and must avoid all gluten', NULL)");
    }

    [Fact]
    public async Task Execute_ViolationFound_ReturnsWarning()
    {
        // Arrange
        await SeedTestDataAsync();
        var args = JsonDocument.Parse(@"{
            ""character_name"": ""Kate"",
            ""behavior"": ""goes to gym""
        }").RootElement;

        // Act
        var result = await _tool.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        Assert.True(response.GetProperty("success").GetBoolean());
        Assert.True(response.GetProperty("is_negative").GetBoolean());

        var violation = response.GetProperty("violation");
        Assert.Equal("Kate", violation.GetProperty("character").GetString());
        Assert.Equal("goes to gym", violation.GetProperty("behavior_checked").GetString());
        Assert.Equal("Does NOT go to gyms", violation.GetProperty("matching_negative").GetString());
        Assert.Equal("absolute", violation.GetProperty("strength").GetString());

        var warning = response.GetProperty("warning").GetString();
        Assert.Contains("CANONICAL VIOLATION", warning);
        Assert.Contains("Kate", warning);
        Assert.Contains("Does NOT go to gyms", warning);
    }

    [Fact]
    public async Task Execute_NoViolation_ReturnsRelatedNegatives()
    {
        // Arrange
        await SeedTestDataAsync();
        var args = JsonDocument.Parse(@"{
            ""character_name"": ""Kate"",
            ""behavior"": ""goes swimming""
        }").RootElement;

        // Act
        var result = await _tool.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        Assert.True(response.GetProperty("success").GetBoolean());
        Assert.False(response.GetProperty("is_negative").GetBoolean());
        Assert.Equal("No canonical negative found for this behavior",
                     response.GetProperty("message").GetString());

        var relatedNegatives = response.GetProperty("related_negatives");
        Assert.True(relatedNegatives.GetArrayLength() >= 3); // Should have all 3 negatives
    }

    [Fact]
    public async Task Execute_WithCategory_FiltersNegatives()
    {
        // Arrange
        await SeedTestDataAsync();
        var args = JsonDocument.Parse(@"{
            ""character_name"": ""Kate"",
            ""behavior"": ""goes swimming"",
            ""category"": ""exercise""
        }").RootElement;

        // Act
        var result = await _tool.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        Assert.True(response.GetProperty("success").GetBoolean());
        Assert.False(response.GetProperty("is_negative").GetBoolean());

        var relatedNegatives = response.GetProperty("related_negatives");
        Assert.Equal(2, relatedNegatives.GetArrayLength()); // Only exercise negatives
    }

    [Fact]
    public async Task Execute_CharacterNotFound_ReturnsError()
    {
        // Arrange
        await SeedTestDataAsync();
        var args = JsonDocument.Parse(@"{
            ""character_name"": ""Bob"",
            ""behavior"": ""goes to gym""
        }").RootElement;

        // Act
        var result = await _tool.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        Assert.False(response.GetProperty("success").GetBoolean());
        Assert.Equal("Character not found", response.GetProperty("error").GetString());
    }

    [Fact]
    public async Task Execute_MissingArguments_ReturnsError()
    {
        // Arrange
        await SeedTestDataAsync();
        var args = JsonDocument.Parse(@"{
            ""character_name"": ""Kate""
        }").RootElement;

        // Act
        var result = await _tool.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        Assert.False(response.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task Execute_NullArguments_ReturnsError()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var result = await _tool.ExecuteAsync(null);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        Assert.False(response.GetProperty("success").GetBoolean());
        Assert.Equal("Missing arguments", response.GetProperty("error").GetString());
    }

    [Fact]
    public async Task Execute_FuzzyNameMatching()
    {
        // Arrange
        await SeedTestDataAsync();
        var args = JsonDocument.Parse(@"{
            ""character_name"": ""kate"",
            ""behavior"": ""goes to gym""
        }").RootElement;

        // Act: Lowercase "kate" should still find "Kate"
        var result = await _tool.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        Assert.True(response.GetProperty("success").GetBoolean());
        Assert.True(response.GetProperty("is_negative").GetBoolean());
    }

    [Fact]
    public async Task ToolMetadata_HasCorrectSchema()
    {
        // Assert
        Assert.Equal("check_negative", _tool.Name);
        Assert.Contains("CRITICAL ERROR PREVENTION", _tool.Description);

        var schema = _tool.InputSchema;
        Assert.Equal("object", schema.Type);
        Assert.Contains("character_name", schema.Properties.Keys);
        Assert.Contains("behavior", schema.Properties.Keys);
        Assert.Contains("category", schema.Properties.Keys);
        Assert.Equal(2, schema.Required.Count);
        Assert.Contains("character_name", schema.Required);
        Assert.Contains("behavior", schema.Required);
    }
}
