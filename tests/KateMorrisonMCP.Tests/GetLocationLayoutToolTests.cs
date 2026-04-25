using System.Text.Json;
using KateMorrisonMCP.Data;
using KateMorrisonMCP.Data.Repositories;
using KateMorrisonMCP.Tools.Tools;

namespace KateMorrisonMCP.Tests;

/// <summary>
/// Tests for GetLocationLayoutTool - retrieves room-by-room layouts
/// </summary>
public class GetLocationLayoutToolTests : IAsyncDisposable
{
    private readonly DatabaseContext _db;
    private readonly LocationRepository _locationRepo;
    private readonly GetLocationLayoutTool _tool;
    private readonly string _testDbPath;

    public GetLocationLayoutToolTests()
    {
        _testDbPath = Path.Combine(Path.GetTempPath(), $"test_location_{Guid.NewGuid()}.db");
        _db = new DatabaseContext(_testDbPath);
        _locationRepo = new LocationRepository(_db);
        _tool = new GetLocationLayoutTool(_locationRepo);
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
            INSERT INTO locations
            (id, name, address_street, address_city, location_type, floor_count)
            VALUES
            (1, 'Paul''s House', '9636 N Kostner Ave', 'Skokie', 'residence', 2)");
        await _db.ExecuteAsync(@"
            INSERT INTO location_rooms
            (location_id, room_name, floor_level, room_type, description)
            VALUES
            (1, 'Kitchen', 'main', 'kitchen', 'Modern kitchen with island'),
            (1, 'Office', 'main', 'office', 'Paul''s workspace'),
            (1, 'Master Bedroom', 'lower', 'bedroom', 'Kate''s workspace in here'),
            (1, 'Living Room', 'main', 'living', 'Main gathering space')");
    }

    [Fact]
    public async Task Execute_AllFloors_ReturnsAll()
    {
        // Arrange
        await SeedTestDataAsync();
        var args = JsonDocument.Parse(@"{
            ""location_name"": ""Paul's House""
        }").RootElement;

        // Act
        var result = await _tool.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        Assert.True(response.GetProperty("success").GetBoolean());

        var location = response.GetProperty("location");
        Assert.Equal("Paul's House", location.GetProperty("name").GetString());
        Assert.Equal(2, location.GetProperty("floor_count").GetInt32());

        var floors = response.GetProperty("floors");
        // Should have main and lower floors
        Assert.True(floors.TryGetProperty("main", out var mainFloor));
        Assert.True(floors.TryGetProperty("lower", out var lowerFloor));

        // Main floor has 3 rooms, lower has 1
        Assert.Equal(3, mainFloor.GetArrayLength());
        Assert.Equal(1, lowerFloor.GetArrayLength());
    }

    [Fact]
    public async Task Execute_SpecificFloor_ReturnsFiltered()
    {
        // Arrange
        await SeedTestDataAsync();
        var args = JsonDocument.Parse(@"{
            ""location_name"": ""Paul's House"",
            ""floor"": ""main""
        }").RootElement;

        // Act
        var result = await _tool.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        Assert.True(response.GetProperty("success").GetBoolean());

        var floors = response.GetProperty("floors");
        // Should only have main floor
        Assert.True(floors.TryGetProperty("main", out var mainFloor));
        Assert.Equal(3, mainFloor.GetArrayLength());

        // Verify rooms are on main floor
        foreach (var room in mainFloor.EnumerateArray())
        {
            var roomName = room.GetProperty("name").GetString();
            Assert.Contains(roomName, new[] { "Kitchen", "Office", "Living Room" });
        }
    }

    [Fact]
    public async Task Execute_LowerFloor_ReturnsFiltered()
    {
        // Arrange
        await SeedTestDataAsync();
        var args = JsonDocument.Parse(@"{
            ""location_name"": ""Paul's House"",
            ""floor"": ""lower""
        }").RootElement;

        // Act
        var result = await _tool.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        Assert.True(response.GetProperty("success").GetBoolean());

        var floors = response.GetProperty("floors");
        // Should only have lower floor
        Assert.True(floors.TryGetProperty("lower", out var lowerFloor));
        Assert.Equal(1, lowerFloor.GetArrayLength());
        Assert.Equal("Master Bedroom", lowerFloor[0].GetProperty("name").GetString());
    }

    [Fact]
    public async Task Execute_LocationNotFound_ReturnsError()
    {
        // Arrange
        await SeedTestDataAsync();
        var args = JsonDocument.Parse(@"{
            ""location_name"": ""Unknown Location""
        }").RootElement;

        // Act
        var result = await _tool.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        Assert.False(response.GetProperty("success").GetBoolean());
        Assert.Equal("Location not found", response.GetProperty("error").GetString());
    }

    [Fact]
    public async Task Execute_NoRooms_ReturnsEmpty()
    {
        // Arrange
        await TestHelpers.CreateTestSchemaAsync(_db);
        await _db.ExecuteAsync(@"
            INSERT INTO locations
            (id, name, location_type)
            VALUES (2, 'Empty House', 'residence')");

        var args = JsonDocument.Parse(@"{
            ""location_name"": ""Empty House""
        }").RootElement;

        // Act
        var result = await _tool.ExecuteAsync(args);
        var json = JsonSerializer.Serialize(result);
        var response = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        Assert.True(response.GetProperty("success").GetBoolean());

        var floors = response.GetProperty("floors");
        // Empty floors dictionary should be returned as empty object
        Assert.Equal(JsonValueKind.Object, floors.ValueKind);
    }

    [Fact]
    public async Task Execute_MissingLocationName_ReturnsError()
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
        Assert.Equal("get_location_layout", _tool.Name);
        Assert.Contains("layout", _tool.Description.ToLower());

        var schema = _tool.InputSchema;
        Assert.Equal("object", schema.Type);
        Assert.Contains("location_name", schema.Properties.Keys);
        Assert.Contains("floor", schema.Properties.Keys);
        Assert.Single(schema.Required);
        Assert.Contains("location_name", schema.Required);
    }
}
