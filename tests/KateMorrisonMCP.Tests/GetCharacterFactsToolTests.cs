using System.Text.Json;
using KateMorrisonMCP.Data;
using KateMorrisonMCP.Data.Repositories;
using KateMorrisonMCP.Tools.Tools;

namespace KateMorrisonMCP.Tests;

/// <summary>
/// Tests for GetCharacterFactsTool - retrieves biographical and physical data
/// </summary>
public class GetCharacterFactsToolTests : IAsyncDisposable
{
    private readonly DatabaseContext _db;
    private readonly CharacterRepository _characterRepo;
    private readonly GetCharacterFactsTool _tool;
    private readonly string _testDbPath;

    public GetCharacterFactsToolTests()
    {
        _testDbPath = Path.Combine(Path.GetTempPath(), $"test_facts_{Guid.NewGuid()}.db");
        _db = new DatabaseContext(_testDbPath);
        _characterRepo = new CharacterRepository(_db);
        _tool = new GetCharacterFactsTool(_characterRepo);
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
            INSERT INTO characters
            (id, full_name, preferred_name, age, birthday, height_inches, weight_lbs,
             build, hair_color, hair_length, eye_color, occupation, employer)
            VALUES
            (1, 'Katherine Marie Morrison', 'Kate', 32, '1993-03-15', 66, 128,
             'slim', 'blonde', 'shoulder-length', 'green',
             'PhD Student', 'Northwestern University')");
    }

    [Fact]
    public async Task Execute_AllCategories_ReturnsComplete()
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
        Assert.True(response.GetProperty("success").GetBoolean());

        var character = response.GetProperty("character");
        Assert.Equal("Kate", character.GetProperty("preferred_name").GetString());

        var biographical = character.GetProperty("biographical");
        Assert.Equal(32, biographical.GetProperty("age").GetInt32());
        Assert.Equal("1993-03-15", biographical.GetProperty("birthday").GetString());

        var physical = character.GetProperty("physical");
        Assert.Equal(66, physical.GetProperty("height_inches").GetInt32());
        Assert.Equal("slim", physical.GetProperty("build").GetString());

        var occupation = character.GetProperty("occupation");
        Assert.Equal("PhD Student", occupation.GetProperty("occupation").GetString());
    }

    [Fact]
    public async Task Execute_BiographicalOnly_ReturnsFiltered()
    {
        // Arrange
        await SeedTestDataAsync();
        var args = JsonDocument.Parse(@"{
            ""character_name"": ""Kate"",
            ""category"": ""biographical""
        }").RootElement;

        // Act
        var result = await _tool.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        Assert.True(response.GetProperty("success").GetBoolean());

        var character = response.GetProperty("character");
        Assert.True(character.TryGetProperty("biographical", out _));
    }

    [Fact]
    public async Task Execute_PhysicalOnly_ReturnsFiltered()
    {
        // Arrange
        await SeedTestDataAsync();
        var args = JsonDocument.Parse(@"{
            ""character_name"": ""Kate"",
            ""category"": ""physical""
        }").RootElement;

        // Act
        var result = await _tool.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        Assert.True(response.GetProperty("success").GetBoolean());

        var character = response.GetProperty("character");
        Assert.True(character.TryGetProperty("physical", out _));
    }

    [Fact]
    public async Task Execute_OccupationOnly_ReturnsFiltered()
    {
        // Arrange
        await SeedTestDataAsync();
        var args = JsonDocument.Parse(@"{
            ""character_name"": ""Kate"",
            ""category"": ""occupation""
        }").RootElement;

        // Act
        var result = await _tool.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        Assert.True(response.GetProperty("success").GetBoolean());

        var character = response.GetProperty("character");
        Assert.True(character.TryGetProperty("occupation", out _));
    }

    [Fact]
    public async Task Execute_CharacterNotFound_ReturnsError()
    {
        // Arrange
        await SeedTestDataAsync();
        var args = JsonDocument.Parse(@"{
            ""character_name"": ""Unknown""
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
    public async Task Execute_FuzzyNameMatching()
    {
        // Arrange
        await SeedTestDataAsync();
        var args = JsonDocument.Parse(@"{
            ""character_name"": ""katherine""
        }").RootElement;

        // Act
        var result = await _tool.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        Assert.True(response.GetProperty("success").GetBoolean());
        var character = response.GetProperty("character");
        Assert.Equal("Kate", character.GetProperty("preferred_name").GetString());
    }

    [Fact]
    public async Task Execute_MissingCharacterName_ReturnsError()
    {
        // Arrange
        await SeedTestDataAsync();
        var args = JsonDocument.Parse(@"{}").RootElement;

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
    }

    [Fact]
    public void ToolMetadata_HasCorrectSchema()
    {
        // Assert
        Assert.Equal("get_character_facts", _tool.Name);
        Assert.Contains("character", _tool.Description.ToLower());

        var schema = _tool.InputSchema;
        Assert.Equal("object", schema.Type);
        Assert.Contains("character_name", schema.Properties.Keys);
        Assert.Contains("category", schema.Properties.Keys);
        Assert.Single(schema.Required);
        Assert.Contains("character_name", schema.Required);
    }
}
