using System.Text.Json;
using KateMorrisonMCP.Data;
using KateMorrisonMCP.Data.Repositories;
using KateMorrisonMCP.Tools.Tools;

namespace KateMorrisonMCP.Tests;

/// <summary>
/// Tests for CheckRelationshipTool - retrieves relationship data between characters
/// </summary>
public class CheckRelationshipToolTests : IAsyncDisposable
{
    private readonly DatabaseContext _db;
    private readonly RelationshipRepository _relationshipRepo;
    private readonly CharacterRepository _characterRepo;
    private readonly TimelineRepository _timelineRepo;
    private readonly CheckRelationshipTool _tool;
    private readonly string _testDbPath;

    public CheckRelationshipToolTests()
    {
        _testDbPath = Path.Combine(Path.GetTempPath(), $"test_relationship_{Guid.NewGuid()}.db");
        _db = new DatabaseContext(_testDbPath);
        _relationshipRepo = new RelationshipRepository(_db);
        _characterRepo = new CharacterRepository(_db);
        _timelineRepo = new TimelineRepository(_db);
        _tool = new CheckRelationshipTool(_relationshipRepo, _characterRepo, _timelineRepo);
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
            INSERT INTO relationships
            (character_a_id, character_b_id, relationship_type, relationship_subtype, start_date, current_status, notes)
            VALUES
            (1, 2, 'romantic_partner', 'boyfriend_girlfriend', '2025-12-28', 'boyfriend_girlfriend',
             'Met at coffee shop, became official on 2025-12-28')");
        await _db.ExecuteAsync(@"
            INSERT INTO timeline_events
            (id, event_date, event_category, event_title, event_description)
            VALUES
            (1, '2025-09-06', 'first_meeting', 'Kate and Paul meet', 'Met at coffee shop'),
            (2, '2025-12-28', 'relationship_status', 'Became official', 'Boyfriend/girlfriend')");
        await _db.ExecuteAsync(@"
            INSERT INTO event_participants (event_id, character_id, role)
            VALUES
            (1, 1, 'participant'), (1, 2, 'participant'),
            (2, 1, 'participant'), (2, 2, 'participant')");
    }

    [Fact]
    public async Task Execute_ExistingRelationship_ReturnsDetails()
    {
        // Arrange
        await SeedTestDataAsync();
        var args = JsonDocument.Parse(@"{
            ""character1"": ""Kate"",
            ""character2"": ""Paul""
        }").RootElement;

        // Act
        var result = await _tool.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        Assert.True(response.GetProperty("success").GetBoolean());
        Assert.Equal("Kate", response.GetProperty("character_a").GetString());
        Assert.Equal("Paul", response.GetProperty("character_b").GetString());

        var relationships = response.GetProperty("relationships");
        Assert.True(relationships.GetArrayLength() > 0);

        var firstRelationship = relationships[0];
        Assert.Equal("romantic_partner", firstRelationship.GetProperty("type").GetString());
        Assert.Equal("boyfriend_girlfriend", firstRelationship.GetProperty("status").GetString());
        Assert.Equal("2025-12-28", firstRelationship.GetProperty("start_date").GetString());
    }

    [Fact]
    public async Task Execute_ReverseOrder_FindsRelationship()
    {
        // Arrange: Relationship stored as (Kate, Paul) but query as (Paul, Kate)
        await SeedTestDataAsync();
        var args = JsonDocument.Parse(@"{
            ""character1"": ""Paul"",
            ""character2"": ""Kate""
        }").RootElement;

        // Act
        var result = await _tool.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert: Should find the relationship regardless of order
        Assert.True(response.GetProperty("success").GetBoolean());

        var relationships = response.GetProperty("relationships");
        Assert.True(relationships.GetArrayLength() > 0);
        Assert.Equal("romantic_partner", relationships[0].GetProperty("type").GetString());
    }

    [Fact]
    public async Task Execute_NoRelationship_ReturnsNotFound()
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);
        await _db.ExecuteAsync(@"
            INSERT INTO characters (id, full_name, preferred_name)
            VALUES (1, 'Person A', 'A'), (2, 'Person B', 'B')");

        var args = JsonDocument.Parse(@"{
            ""character1"": ""A"",
            ""character2"": ""B""
        }").RootElement;

        // Act
        var result = await _tool.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        Assert.True(response.GetProperty("success").GetBoolean());
        Assert.Equal("No relationship found between these characters", response.GetProperty("message").GetString());
        Assert.Equal("A", response.GetProperty("character_a").GetString());
        Assert.Equal("B", response.GetProperty("character_b").GetString());
    }

    [Fact]
    public async Task Execute_Character1NotFound_ReturnsError()
    {
        // Arrange
        await SeedTestDataAsync();
        var args = JsonDocument.Parse(@"{
            ""character1"": ""Unknown"",
            ""character2"": ""Paul""
        }").RootElement;

        // Act
        var result = await _tool.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        Assert.False(response.GetProperty("success").GetBoolean());
        Assert.Contains("not found", response.GetProperty("error").GetString().ToLower());
    }

    [Fact]
    public async Task Execute_Character2NotFound_ReturnsError()
    {
        // Arrange
        await SeedTestDataAsync();
        var args = JsonDocument.Parse(@"{
            ""character1"": ""Kate"",
            ""character2"": ""Unknown""
        }").RootElement;

        // Act
        var result = await _tool.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        Assert.False(response.GetProperty("success").GetBoolean());
        Assert.Contains("not found", response.GetProperty("error").GetString().ToLower());
    }

    [Fact]
    public async Task Execute_MissingCharacter1_ReturnsError()
    {
        // Arrange
        await SeedTestDataAsync();
        var args = JsonDocument.Parse(@"{
            ""character2"": ""Paul""
        }").RootElement;

        // Act
        var result = await _tool.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        Assert.False(response.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task Execute_MissingCharacter2_ReturnsError()
    {
        // Arrange
        await SeedTestDataAsync();
        var args = JsonDocument.Parse(@"{
            ""character1"": ""Kate""
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
    }

    [Fact]
    public void ToolMetadata_HasCorrectSchema()
    {
        // Assert
        Assert.Equal("check_relationship", _tool.Name);
        Assert.Contains("relationship", _tool.Description.ToLower());

        var schema = _tool.InputSchema;
        Assert.Equal("object", schema.Type);
        Assert.Contains("character1", schema.Properties.Keys);
        Assert.Contains("character2", schema.Properties.Keys);
        Assert.Equal(2, schema.Required.Count);
        Assert.Contains("character1", schema.Required);
        Assert.Contains("character2", schema.Required);
    }
}
