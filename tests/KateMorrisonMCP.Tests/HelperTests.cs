using KateMorrisonMCP.Ingestion.Helpers;
using Xunit;

namespace KateMorrisonMCP.Tests;

public class HeightParserTests
{
    [Theory]
    [InlineData("5'6\"", 66)]
    [InlineData("5'6", 66)]
    [InlineData("6'2\"", 74)]
    [InlineData("5 feet 6 inches", 66)]
    [InlineData("6 feet", 72)]
    public void Parse_ValidHeight_ReturnsCorrectInches(string input, int expected)
    {
        // Act
        var result = HeightParser.Parse(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid")]
    [InlineData("abc")]
    public void Parse_InvalidHeight_ReturnsNull(string? input)
    {
        // Act
        var result = HeightParser.Parse(input);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Format_ValidInches_ReturnsFormattedString()
    {
        // Act
        var result = HeightParser.Format(66);

        // Assert
        Assert.Equal("5'6\"", result);
    }
}

public class DimensionParserTests
{
    [Fact]
    public void Parse_ValidDimensions_ReturnsCorrectValues()
    {
        // Act
        var (width, length) = DimensionParser.Parse("14'2\" × 10'11\"");

        // Assert
        Assert.NotNull(width);
        Assert.NotNull(length);
        Assert.Equal(14.17m, width.Value, 2);
        Assert.Equal(10.92m, length.Value, 2);
    }

    [Fact]
    public void Parse_DimensionsWithX_ReturnsCorrectValues()
    {
        // Act
        var (width, length) = DimensionParser.Parse("10'0\" x 12'0\"");

        // Assert
        Assert.NotNull(width);
        Assert.NotNull(length);
        Assert.Equal(10.0m, width.Value, 2);
        Assert.Equal(12.0m, length.Value, 2);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid")]
    [InlineData("10 feet")]
    public void Parse_InvalidDimensions_ReturnsNull(string? input)
    {
        // Act
        var (width, length) = DimensionParser.Parse(input);

        // Assert
        Assert.Null(width);
        Assert.Null(length);
    }

    [Fact]
    public void Format_ValidDimensions_ReturnsFormattedString()
    {
        // Act
        var result = DimensionParser.Format(14.17m, 10.92m);

        // Assert
        Assert.Equal("14'2\" × 10'11\"", result);
    }
}

public class DaysParserTests
{
    [Fact]
    public void Parse_Weekdays_ReturnsCorrectJsonArray()
    {
        // Act
        var result = DaysParser.Parse("weekdays");

        // Assert
        Assert.Contains("monday", result);
        Assert.Contains("tuesday", result);
        Assert.Contains("wednesday", result);
        Assert.Contains("thursday", result);
        Assert.Contains("friday", result);
        Assert.DoesNotContain("saturday", result);
        Assert.DoesNotContain("sunday", result);
    }

    [Theory]
    [InlineData("weekends")]
    [InlineData("weekend")]
    public void Parse_Weekend_ReturnsCorrectJsonArray(string input)
    {
        // Act
        var result = DaysParser.Parse(input);

        // Assert
        Assert.Contains("saturday", result);
        Assert.Contains("sunday", result);
        Assert.DoesNotContain("monday", result);
    }

    [Theory]
    [InlineData("daily")]
    [InlineData("every day")]
    [InlineData("all days")]
    public void Parse_Daily_ReturnsAllDays(string input)
    {
        // Act
        var result = DaysParser.Parse(input);

        // Assert
        Assert.Contains("monday", result);
        Assert.Contains("sunday", result);
    }

    [Fact]
    public void Parse_CommaSeparatedDays_ReturnsCorrectJsonArray()
    {
        // Act
        var result = DaysParser.Parse("monday, wednesday, friday");

        // Assert
        Assert.Contains("monday", result);
        Assert.Contains("wednesday", result);
        Assert.Contains("friday", result);
        Assert.DoesNotContain("tuesday", result);
    }

    [Fact]
    public void Parse_InvalidDays_ReturnsEmptyArray()
    {
        // Act
        var result = DaysParser.Parse("funday, notaday");

        // Assert
        Assert.Equal("[]", result);
    }

    [Theory]
    [InlineData("weekdays", true)]
    [InlineData("monday, tuesday", true)]
    [InlineData("invalid", false)]
    [InlineData("", false)]
    public void IsValid_ReturnsCorrectResult(string input, bool expected)
    {
        // Act
        var result = DaysParser.IsValid(input);

        // Assert
        Assert.Equal(expected, result);
    }
}

public class JsonArrayHelperTests
{
    [Fact]
    public void ToJsonArray_CommaSeparated_ReturnsCorrectJson()
    {
        // Act
        var result = JsonArrayHelper.ToJsonArray("red eyes, tall, athletic");

        // Assert
        Assert.Contains("red eyes", result);
        Assert.Contains("tall", result);
        Assert.Contains("athletic", result);
    }

    [Fact]
    public void ToJsonArray_SemicolonSeparated_ReturnsCorrectJson()
    {
        // Act
        var result = JsonArrayHelper.ToJsonArray("item1; item2; item3", [',', ';']);

        // Assert
        Assert.Contains("item1", result);
        Assert.Contains("item2", result);
        Assert.Contains("item3", result);
    }

    [Fact]
    public void ToJsonArray_Null_ReturnsEmptyArray()
    {
        // Act
        var result = JsonArrayHelper.ToJsonArray(null);

        // Assert
        Assert.Equal("[]", result);
    }

    [Fact]
    public void ToJsonArray_RemovesDuplicates()
    {
        // Act
        var result = JsonArrayHelper.ToJsonArray("item, item, other");

        // Assert
        // Should contain both items, but "item" only once (duplicates removed)
        Assert.Contains("\"item\"", result);
        Assert.Contains("\"other\"", result);

        // Deserialize to verify duplicate was removed
        var items = System.Text.Json.JsonSerializer.Deserialize<List<string>>(result);
        Assert.NotNull(items);
        Assert.Equal(2, items.Count); // Should have 2 distinct items
        Assert.Contains("item", items);
        Assert.Contains("other", items);
    }

    [Fact]
    public void FromJsonArray_ValidJson_ReturnsCommaSeparated()
    {
        // Act
        var result = JsonArrayHelper.FromJsonArray("[\"red eyes\",\"tall\",\"athletic\"]");

        // Assert
        Assert.Equal("red eyes, tall, athletic", result);
    }

    [Fact]
    public void FromJsonArray_EmptyArray_ReturnsEmpty()
    {
        // Act
        var result = JsonArrayHelper.FromJsonArray("[]");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void IsValidJsonArray_ValidJson_ReturnsTrue()
    {
        // Act
        var result = JsonArrayHelper.IsValidJsonArray("[\"item1\",\"item2\"]");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidJsonArray_InvalidJson_ReturnsFalse()
    {
        // Act
        var result = JsonArrayHelper.IsValidJsonArray("not valid json");

        // Assert
        Assert.False(result);
    }
}
