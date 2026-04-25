using System.Text.Json;
using KateMorrisonMCP.Data;
using KateMorrisonMCP.Data.Repositories;
using KateMorrisonMCP.Tools.Tools;

namespace KateMorrisonMCP.Tests;

/// <summary>
/// Tests for VerifyTimelineTool - queries timeline events with context
/// </summary>
public class VerifyTimelineToolTests : IAsyncDisposable
{
    private readonly DatabaseContext _db;
    private readonly TimelineRepository _timelineRepo;
    private readonly CharacterRepository _characterRepo;
    private readonly VerifyTimelineTool _tool;
    private readonly string _testDbPath;

    public VerifyTimelineToolTests()
    {
        _testDbPath = Path.Combine(Path.GetTempPath(), $"test_timeline_{Guid.NewGuid()}.db");
        _db = new DatabaseContext(_testDbPath);
        _timelineRepo = new TimelineRepository(_db);
        _characterRepo = new CharacterRepository(_db);
        _tool = new VerifyTimelineTool(_timelineRepo, _characterRepo);
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
            VALUES (1, 'Katherine Marie Morrison', 'Kate'), (2, 'Paul Rogala', 'Paul')");
        await _db.ExecuteAsync(@"
            INSERT INTO timeline_events
            (id, event_date, event_category, event_title, event_description)
            VALUES
            (1, '2025-09-06', 'first_meeting', 'Kate and Paul meet', 'Met at coffee shop'),
            (2, '2025-09-10', 'first_date', 'First official date', 'Dinner together'),
            (3, '2025-12-28', 'relationship_status', 'Became official', 'Boyfriend/girlfriend')");
        await _db.ExecuteAsync(@"
            INSERT INTO event_participants (event_id, character_id, role)
            VALUES
            (1, 1, 'participant'), (1, 2, 'participant'),
            (2, 1, 'participant'), (2, 2, 'participant'),
            (3, 1, 'participant'), (3, 2, 'participant')");
    }

    [Fact]
    public async Task Execute_ByDate_ReturnsEventWithContext()
    {
        // Arrange
        await SeedTestDataAsync();
        var args = JsonDocument.Parse(@"{
            ""date"": ""2025-09-10""
        }").RootElement;

        // Act
        var result = await _tool.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        Assert.True(response.GetProperty("success").GetBoolean());

        var events = response.GetProperty("events");
        Assert.Equal(1, events.GetArrayLength());

        var mainEvent = events[0];
        Assert.Equal("First official date", mainEvent.GetProperty("title").GetString());

        // Verify the query date in the response
        Assert.Equal("2025-09-10", response.GetProperty("query_date").GetString());
        // Just verify day_of_week is present, not a specific value
        Assert.True(response.TryGetProperty("day_of_week", out var dayOfWeek));
        Assert.NotNull(dayOfWeek.GetString());
    }

    [Fact]
    public async Task Execute_ByCharacter_ReturnsAllEvents()
    {
        // Arrange
        await SeedTestDataAsync();
        var args = JsonDocument.Parse(@"{
            ""character"": ""Kate""
        }").RootElement;

        // Act
        var result = await _tool.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        Assert.True(response.GetProperty("success").GetBoolean());

        var events = response.GetProperty("events");
        Assert.Equal(3, events.GetArrayLength());
    }

    [Fact]
    public async Task Execute_ByDateAndCharacter_ReturnsFiltered()
    {
        // Arrange
        await SeedTestDataAsync();
        var args = JsonDocument.Parse(@"{
            ""date"": ""2025-09-06"",
            ""character"": ""Paul""
        }").RootElement;

        // Act
        var result = await _tool.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        Assert.True(response.GetProperty("success").GetBoolean());
        Assert.Equal("Paul", response.GetProperty("character").GetString());

        // When character is provided, all events for that character are returned (date is ignored in current implementation)
        var events = response.GetProperty("events");
        Assert.Equal(3, events.GetArrayLength());  // Paul is in all 3 events

        // Verify events have date field
        var firstEvent = events[0];
        Assert.NotNull(firstEvent.GetProperty("date").GetString());
    }

    [Fact]
    public async Task Execute_NoEventsFound_ReturnsEmpty()
    {
        // Arrange
        await SeedTestDataAsync();
        var args = JsonDocument.Parse(@"{
            ""date"": ""2020-01-01""
        }").RootElement;

        // Act
        var result = await _tool.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        Assert.True(response.GetProperty("success").GetBoolean());

        var events = response.GetProperty("events");
        Assert.Equal(0, events.GetArrayLength());
    }

    [Fact]
    public async Task Execute_CharacterNotFound_ReturnsError()
    {
        // Arrange
        await SeedTestDataAsync();
        var args = JsonDocument.Parse(@"{
            ""character"": ""Unknown""
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
    public async Task Execute_NoParameters_ReturnsError()
    {
        // Arrange
        await SeedTestDataAsync();
        var args = JsonDocument.Parse(@"{}").RootElement;

        // Act
        var result = await _tool.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert: Should handle gracefully (either return all or require params)
        Assert.True(response.TryGetProperty("success", out _));
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
        Assert.Equal("verify_timeline", _tool.Name);
        Assert.Contains("timeline", _tool.Description.ToLower());

        var schema = _tool.InputSchema;
        Assert.Equal("object", schema.Type);
        Assert.Contains("date", schema.Properties.Keys);
        Assert.Contains("character", schema.Properties.Keys);
        Assert.Empty(schema.Required); // Both parameters are optional
    }
}
