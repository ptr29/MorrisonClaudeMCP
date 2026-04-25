using System.Text.Json;
using KateMorrisonMCP.Data;
using KateMorrisonMCP.Data.Repositories;
using KateMorrisonMCP.Tools.Tools;

namespace KateMorrisonMCP.Tests;

/// <summary>
/// Tests for GetScheduleTool - retrieves character schedules with filtering
/// </summary>
public class GetScheduleToolTests : IAsyncDisposable
{
    private readonly DatabaseContext _db;
    private readonly CharacterRepository _characterRepo;
    private readonly ScheduleRepository _scheduleRepo;
    private readonly GetScheduleTool _tool;
    private readonly string _testDbPath;

    public GetScheduleToolTests()
    {
        _testDbPath = Path.Combine(Path.GetTempPath(), $"test_schedule_{Guid.NewGuid()}.db");
        _db = new DatabaseContext(_testDbPath);
        _characterRepo = new CharacterRepository(_db);
        _scheduleRepo = new ScheduleRepository(_db);
        _tool = new GetScheduleTool(_scheduleRepo, _characterRepo);
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
            VALUES (1, 'Paul Rogala', 'Paul')");
        await _db.ExecuteAsync(@"
            INSERT INTO schedules
            (character_id, schedule_type, schedule_name, days_of_week, start_time, end_time, description, location_description)
            VALUES
            (1, 'work', 'Mumbai Standup', '[""Monday""]', '07:00', '16:00', 'Mumbai standup at 7 AM, work until 4 PM', 'Home Office'),
            (1, 'work', 'Mumbai Standup', '[""Tuesday""]', '07:00', '16:00', 'Mumbai standup at 7 AM, work until 4 PM', 'Home Office'),
            (1, 'exercise', 'Morning Run', '[""Monday""]', '05:40', '06:30', 'Morning run before work', 'Neighborhood'),
            (1, 'daily_routine', 'Wake Up', NULL, '05:40', '05:45', 'Wake up', 'Home')");
    }

    [Fact]
    public async Task Execute_AllSchedules_ReturnsAll()
    {
        // Arrange
        await SeedTestDataAsync();
        var args = JsonDocument.Parse(@"{
            ""character_name"": ""Paul""
        }").RootElement;

        // Act
        var result = await _tool.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        Assert.True(response.GetProperty("success").GetBoolean());
        Assert.Equal("Paul", response.GetProperty("character").GetString());

        var schedules = response.GetProperty("schedules");
        Assert.Equal(4, schedules.GetArrayLength());
    }

    [Fact]
    public async Task Execute_FilterByType_ReturnsFiltered()
    {
        // Arrange
        await SeedTestDataAsync();
        var args = JsonDocument.Parse(@"{
            ""character_name"": ""Paul"",
            ""schedule_type"": ""work""
        }").RootElement;

        // Act
        var result = await _tool.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        Assert.True(response.GetProperty("success").GetBoolean());

        var schedules = response.GetProperty("schedules");
        Assert.Equal(2, schedules.GetArrayLength());

        foreach (var schedule in schedules.EnumerateArray())
        {
            Assert.Equal("work", schedule.GetProperty("type").GetString());
        }
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
    public async Task Execute_NoSchedules_ReturnsEmpty()
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);
        await _db.ExecuteAsync(@"
            INSERT INTO characters (id, full_name, preferred_name)
            VALUES (1, 'Empty Person', 'Empty')");

        var args = JsonDocument.Parse(@"{
            ""character_name"": ""Empty""
        }").RootElement;

        // Act
        var result = await _tool.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        Assert.True(response.GetProperty("success").GetBoolean());

        var schedules = response.GetProperty("schedules");
        Assert.Equal(0, schedules.GetArrayLength());
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
        Assert.Equal("get_schedule", _tool.Name);
        Assert.Contains("schedule", _tool.Description.ToLower());

        var schema = _tool.InputSchema;
        Assert.Equal("object", schema.Type);
        Assert.Contains("character_name", schema.Properties.Keys);
        Assert.Contains("schedule_type", schema.Properties.Keys);
        Assert.Single(schema.Required);
        Assert.Contains("character_name", schema.Required);
    }
}
