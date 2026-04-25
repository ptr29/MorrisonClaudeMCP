using KateMorrisonMCP.Ingestion.Parsing;
using Xunit;

namespace KateMorrisonMCP.Tests;

public class TagParserTests
{
    private readonly TagParser _parser = new();

    [Fact]
    public void ParseFile_BlockFormat_ExtractsCorrectly()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, @"
# Test Document

<!-- canonical: character
name: Kate Morrison
age: 29
height: 5'6""
occupation: Private Investigator
-->

Some content here.
");

        try
        {
            // Act
            var tags = _parser.ParseFile(tempFile).ToList();

            // Assert
            Assert.Single(tags);
            var tag = tags[0];
            Assert.Equal("character", tag.Type);
            Assert.Equal("Kate Morrison", tag.GetRequired("name"));
            Assert.Equal("29", tag.GetOptional("age"));
            Assert.Equal("5'6\"", tag.GetOptional("height"));
            Assert.Equal("Private Investigator", tag.GetOptional("occupation"));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void ParseFile_InlineFormat_ExtractsCorrectly()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, @"
# Test Document

<!-- canonical: character | name: Sarah Chen | age: 32 | occupation: Detective -->

Some content here.
");

        try
        {
            // Act
            var tags = _parser.ParseFile(tempFile).ToList();

            // Assert
            Assert.Single(tags);
            var tag = tags[0];
            Assert.Equal("character", tag.Type);
            Assert.Equal("Sarah Chen", tag.GetRequired("name"));
            Assert.Equal("32", tag.GetOptional("age"));
            Assert.Equal("Detective", tag.GetOptional("occupation"));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void ParseFile_MultipleTags_ExtractsAll()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, @"
<!-- canonical: character
name: Kate Morrison
age: 29
-->

<!-- canonical: location | name: Detective Agency | city: Seattle -->

<!-- canonical: negative
character: Kate Morrison
negative_behavior: Does NOT go to gyms
strength: absolute
-->
");

        try
        {
            // Act
            var tags = _parser.ParseFile(tempFile).ToList();

            // Assert
            Assert.Equal(3, tags.Count);
            Assert.Contains(tags, t => t.Type == "character");
            Assert.Contains(tags, t => t.Type == "location");
            Assert.Contains(tags, t => t.Type == "negative");
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void ParseFile_NoTags_ReturnsEmpty()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, @"
# Test Document

Just regular markdown content with no canonical tags.
");

        try
        {
            // Act
            var tags = _parser.ParseFile(tempFile).ToList();

            // Assert
            Assert.Empty(tags);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void CanonicalTag_GetRequired_ThrowsWhenMissing()
    {
        // Arrange
        var tag = new CanonicalTag
        {
            Type = "character",
            SourceFile = "test.md",
            LineNumber = 1,
            Fields = new Dictionary<string, string>()
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => tag.GetRequired("name"));
    }

    [Fact]
    public void CanonicalTag_GetOptional_ReturnsNullWhenMissing()
    {
        // Arrange
        var tag = new CanonicalTag
        {
            Type = "character",
            SourceFile = "test.md",
            LineNumber = 1,
            Fields = new Dictionary<string, string>()
        };

        // Act
        var result = tag.GetOptional("name");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void CanonicalTag_GetOptionalInt_ReturnsNullForInvalidInt()
    {
        // Arrange
        var tag = new CanonicalTag
        {
            Type = "character",
            SourceFile = "test.md",
            LineNumber = 1,
            Fields = new Dictionary<string, string> { ["age"] = "not a number" }
        };

        // Act
        var result = tag.GetOptionalInt("age");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void CanonicalTag_GetOptionalInt_ReturnsValueForValidInt()
    {
        // Arrange
        var tag = new CanonicalTag
        {
            Type = "character",
            SourceFile = "test.md",
            LineNumber = 1,
            Fields = new Dictionary<string, string> { ["age"] = "29" }
        };

        // Act
        var result = tag.GetOptionalInt("age");

        // Assert
        Assert.Equal(29, result);
    }
}
