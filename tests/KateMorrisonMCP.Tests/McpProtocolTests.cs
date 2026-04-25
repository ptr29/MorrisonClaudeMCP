using System.Text.Json;
using KateMorrisonMCP.Server.Models;

namespace KateMorrisonMCP.Tests;

/// <summary>
/// Tests for MCP protocol JSON-RPC 2.0 message handling
/// CRITICAL: Tests that we properly handle notifications vs requests
/// </summary>
public class McpProtocolTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    [Fact]
    public void McpRequest_WithId_IsRequest()
    {
        // Arrange
        var json = @"{
            ""jsonrpc"": ""2.0"",
            ""id"": 1,
            ""method"": ""tools/list"",
            ""params"": {}
        }";

        // Act
        var request = JsonSerializer.Deserialize<McpRequest>(json, JsonOptions);

        // Assert
        Assert.NotNull(request);
        Assert.NotNull(request.Id);
        Assert.Equal("tools/list", request.Method);
    }

    [Fact]
    public void McpRequest_WithoutId_IsNotification()
    {
        // Arrange
        var json = @"{
            ""jsonrpc"": ""2.0"",
            ""method"": ""notifications/initialized""
        }";

        // Act
        var request = JsonSerializer.Deserialize<McpRequest>(json, JsonOptions);

        // Assert
        Assert.NotNull(request);
        Assert.Null(request.Id);
        Assert.Equal("notifications/initialized", request.Method);
    }

    [Fact]
    public void McpRequest_StringId_Supported()
    {
        // Arrange: JSON-RPC 2.0 supports both string and number IDs
        var json = @"{
            ""jsonrpc"": ""2.0"",
            ""id"": ""request-123"",
            ""method"": ""tools/list"",
            ""params"": {}
        }";

        // Act
        var request = JsonSerializer.Deserialize<McpRequest>(json, JsonOptions);

        // Assert
        Assert.NotNull(request);
        Assert.NotNull(request.Id);
        Assert.Equal("tools/list", request.Method);
    }

    [Fact]
    public void McpResponse_IdAlwaysSerialized()
    {
        // Arrange
        var response = new McpResponse
        {
            Result = new { test = "value" },
            Id = null // Even null should be serialized
        };

        // Act
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Assert: Verify "id":null is in the JSON (not omitted)
        Assert.Contains("\"id\":null", json);
    }

    [Fact]
    public void McpResponse_ResultNotError()
    {
        // Arrange
        var response = new McpResponse
        {
            Result = new { test = "value" },
            Id = 1
        };

        // Act
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        // Assert: Should have result but not error
        Assert.Contains("\"result\"", json);
        Assert.DoesNotContain("\"error\"", json);
    }

    [Fact]
    public void McpResponse_ErrorNotResult()
    {
        // Arrange
        var response = new McpResponse
        {
            Error = new { code = -32601, message = "Method not found" },
            Id = 1
        };

        // Act
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        // Assert: Should have error but not result
        Assert.Contains("\"error\"", json);
        Assert.DoesNotContain("\"result\"", json);
    }

    [Fact]
    public void McpResponse_MatchesRequestId()
    {
        // Arrange
        var requestId = "test-123";
        var response = new McpResponse
        {
            Result = new { test = "value" },
            Id = requestId
        };

        // Act
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        var deserialized = JsonSerializer.Deserialize<McpResponse>(json, JsonOptions);

        // Assert: ID should match exactly
        Assert.Equal(requestId, deserialized?.Id?.ToString());
    }

    [Fact]
    public void McpResponse_HasJsonRpc20()
    {
        // Arrange
        var response = new McpResponse
        {
            Result = new { test = "value" },
            Id = 1
        };

        // Act
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Assert
        Assert.Contains("\"jsonrpc\":\"2.0\"", json);
    }

    [Theory]
    [InlineData(-32700, "Parse error")]
    [InlineData(-32600, "Invalid Request")]
    [InlineData(-32601, "Method not found")]
    [InlineData(-32602, "Invalid params")]
    [InlineData(-32603, "Internal error")]
    public void McpResponse_ErrorCodes_Standard(int code, string message)
    {
        // Arrange
        var response = new McpResponse
        {
            Error = new { code, message },
            Id = 1
        };

        // Act
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Assert
        Assert.Contains($"\"code\":{code}", json);
        Assert.Contains(message, json);
    }

    [Fact]
    public void McpRequest_ParamsOptional()
    {
        // Arrange: Some methods don't require params
        var json = @"{
            ""jsonrpc"": ""2.0"",
            ""id"": 1,
            ""method"": ""tools/list""
        }";

        // Act
        var request = JsonSerializer.Deserialize<McpRequest>(json, JsonOptions);

        // Assert
        Assert.NotNull(request);
        Assert.Null(request.Params);
    }

    [Fact]
    public void McpRequest_ParamsCanBeObject()
    {
        // Arrange
        var json = @"{
            ""jsonrpc"": ""2.0"",
            ""id"": 2,
            ""method"": ""tools/call"",
            ""params"": {
                ""name"": ""check_negative"",
                ""arguments"": {
                    ""character_name"": ""Kate"",
                    ""behavior"": ""goes to gym""
                }
            }
        }";

        // Act
        var request = JsonSerializer.Deserialize<McpRequest>(json, JsonOptions);

        // Assert
        Assert.NotNull(request);
        Assert.NotNull(request.Params);
        var name = request.Params.Value.GetProperty("name").GetString();
        Assert.Equal("check_negative", name);
    }
}
