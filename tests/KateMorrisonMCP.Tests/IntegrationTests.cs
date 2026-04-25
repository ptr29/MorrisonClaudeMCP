using System.Text.Json;
using KateMorrisonMCP.Data;
using KateMorrisonMCP.Data.Repositories;
using KateMorrisonMCP.Tools;
using KateMorrisonMCP.Tools.Tools;

namespace KateMorrisonMCP.Tests;

/// <summary>
/// End-to-end integration tests using the real seed data
/// Tests the complete workflow from database to tool execution
/// </summary>
public class IntegrationTests : IAsyncDisposable
{
    private readonly DatabaseContext _db;
    private readonly ToolRegistry _registry;
    private readonly string _testDbPath;

    public IntegrationTests()
    {
        _testDbPath = Path.Combine(Path.GetTempPath(), $"test_integration_{Guid.NewGuid()}.db");
        _db = new DatabaseContext(_testDbPath);
        _registry = new ToolRegistry();
    }

    public async ValueTask DisposeAsync()
    {
        if (File.Exists(_testDbPath))
        {
            File.Delete(_testDbPath);
        }
        await Task.CompletedTask;
    }

    private async Task SeedFullDataAsync()
    {
        await TestHelpers.CreateTestSchemaAsync(_db);

        // Seed Kate
        await _db.ExecuteAsync(@"
            INSERT INTO characters (id, full_name, preferred_name, age, birthday)
            VALUES (1, 'Katherine Marie Morrison', 'Kate', 32, '1993-03-15')");

        // Seed Kate's negatives with exception conditions
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
             'Kate has celiac disease and must avoid all gluten', NULL),
            (1, 'food', 'Does NOT drink regular coffee after 2 PM', 'strong',
             'Kate avoids caffeine in afternoon to protect sleep quality',
             '[""may have decaf after 2 PM"", ""exceptions for special occasions""]')");
    }

    [Fact]
    public async Task ToolRegistry_RegistersAllTools()
    {
        // Arrange
        await SeedFullDataAsync();
        var characterRepo = new CharacterRepository(_db);
        var negativeRepo = new NegativeRepository(_db);
        var scheduleRepo = new ScheduleRepository(_db);
        var locationRepo = new LocationRepository(_db);
        var timelineRepo = new TimelineRepository(_db);
        var relationshipRepo = new RelationshipRepository(_db);

        // Act: Register all 6 tools
        _registry.RegisterTool(new CheckNegativeTool(negativeRepo, characterRepo));
        _registry.RegisterTool(new GetScheduleTool(scheduleRepo, characterRepo));
        _registry.RegisterTool(new GetCharacterFactsTool(characterRepo));
        _registry.RegisterTool(new GetLocationLayoutTool(locationRepo));
        _registry.RegisterTool(new VerifyTimelineTool(timelineRepo, characterRepo));
        _registry.RegisterTool(new CheckRelationshipTool(relationshipRepo, characterRepo, timelineRepo));

        // Assert
        var tools = _registry.GetAllTools();
        Assert.Equal(6, tools.Count());

        var toolNames = tools.Select(t => t.Name).ToList();
        Assert.Contains("check_negative", toolNames);
        Assert.Contains("get_schedule", toolNames);
        Assert.Contains("get_character_facts", toolNames);
        Assert.Contains("get_location_layout", toolNames);
        Assert.Contains("verify_timeline", toolNames);
        Assert.Contains("check_relationship", toolNames);
    }

    [Fact]
    public async Task EndToEnd_GymQuestion_ReturnsViolation()
    {
        // Arrange
        await SeedFullDataAsync();
        var characterRepo = new CharacterRepository(_db);
        var negativeRepo = new NegativeRepository(_db);
        _registry.RegisterTool(new CheckNegativeTool(negativeRepo, characterRepo));

        var tool = _registry.GetTool("check_negative");
        var args = JsonDocument.Parse(@"{
            ""character_name"": ""Kate"",
            ""behavior"": ""goes to gym""
        }").RootElement;

        // Act
        var result = await tool!.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert: Should detect violation
        Assert.True(response.GetProperty("success").GetBoolean());
        Assert.True(response.GetProperty("is_negative").GetBoolean());

        var violation = response.GetProperty("violation");
        Assert.Equal("Does NOT go to gyms", violation.GetProperty("matching_negative").GetString());
        Assert.Equal("absolute", violation.GetProperty("strength").GetString());

        var warning = response.GetProperty("warning").GetString();
        Assert.Contains("CANONICAL VIOLATION", warning);
        Assert.Contains("ABSOLUTE negative", warning);
        Assert.Contains("NO exceptions", warning);
    }

    [Fact]
    public async Task EndToEnd_TreadmillQuestion_HasException()
    {
        // Arrange
        await SeedFullDataAsync();
        var characterRepo = new CharacterRepository(_db);
        var negativeRepo = new NegativeRepository(_db);
        _registry.RegisterTool(new CheckNegativeTool(negativeRepo, characterRepo));

        var tool = _registry.GetTool("check_negative");
        var args = JsonDocument.Parse(@"{
            ""character_name"": ""Kate"",
            ""behavior"": ""runs on treadmill""
        }").RootElement;

        // Act
        var result = await tool!.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert: Should detect violation with exceptions
        Assert.True(response.GetProperty("success").GetBoolean());
        Assert.True(response.GetProperty("is_negative").GetBoolean());

        var violation = response.GetProperty("violation");
        Assert.Equal("Does NOT run on treadmills", violation.GetProperty("matching_negative").GetString());
        Assert.Equal("strong", violation.GetProperty("strength").GetString());

        var exceptions = violation.GetProperty("exceptions");
        Assert.True(exceptions.GetArrayLength() > 0);
        Assert.Contains("15°F", exceptions[0].GetString());

        var warning = response.GetProperty("warning").GetString();
        Assert.Contains("limited exceptions", warning);
    }

    [Fact]
    public async Task EndToEnd_GlutenQuestion_DetectsAbsoluteViolation()
    {
        // Arrange
        await SeedFullDataAsync();
        var characterRepo = new CharacterRepository(_db);
        var negativeRepo = new NegativeRepository(_db);
        _registry.RegisterTool(new CheckNegativeTool(negativeRepo, characterRepo));

        var tool = _registry.GetTool("check_negative");
        var args = JsonDocument.Parse(@"{
            ""character_name"": ""Kate"",
            ""behavior"": ""eats gluten"",
            ""category"": ""food""
        }").RootElement;

        // Act
        var result = await tool!.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert: Should detect gluten violation
        Assert.True(response.GetProperty("success").GetBoolean());
        Assert.True(response.GetProperty("is_negative").GetBoolean());

        var violation = response.GetProperty("violation");
        Assert.Equal("Does NOT eat gluten", violation.GetProperty("matching_negative").GetString());
        Assert.Equal("absolute", violation.GetProperty("strength").GetString());
        Assert.Contains("celiac disease", violation.GetProperty("explanation").GetString());
    }

    [Fact]
    public async Task EndToEnd_CharacterFacts_RetrievesData()
    {
        // Arrange
        await SeedFullDataAsync();
        var characterRepo = new CharacterRepository(_db);
        _registry.RegisterTool(new GetCharacterFactsTool(characterRepo));

        var tool = _registry.GetTool("get_character_facts");
        var args = JsonDocument.Parse(@"{
            ""character_name"": ""Kate""
        }").RootElement;

        // Act
        var result = await tool!.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        Assert.True(response.GetProperty("success").GetBoolean());

        var character = response.GetProperty("character");
        Assert.Equal("Kate", character.GetProperty("preferred_name").GetString());
        Assert.Equal(32, character.GetProperty("biographical").GetProperty("age").GetInt32());
    }

    [Fact]
    public async Task EndToEnd_ToolNotFound_ReturnsNull()
    {
        // Act
        var tool = _registry.GetTool("nonexistent_tool");

        // Assert
        Assert.Null(tool);
    }
}
