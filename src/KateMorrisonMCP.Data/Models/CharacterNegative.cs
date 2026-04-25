using System.Text.Json;

namespace KateMorrisonMCP.Data.Models;

/// <summary>
/// CRITICAL MODEL: Represents behaviors a character explicitly does NOT do
/// Used for error prevention in narrative writing
/// </summary>
public class CharacterNegative
{
    private int _id;
    private int _characterId;
    private string _negativeCategory = string.Empty;
    private string _negativeBehavior = string.Empty;
    private string _strength = "strong";
    private string? _explanation;
    private string? _exceptionConditions;

    // Public properties (PascalCase for C# conventions)
    public int Id
    {
        get => _id;
        set => _id = value;
    }

    public int CharacterId
    {
        get => _characterId;
        set => _characterId = value;
    }

    public string NegativeCategory
    {
        get => _negativeCategory;
        set => _negativeCategory = value;
    }

    public string NegativeBehavior
    {
        get => _negativeBehavior;
        set => _negativeBehavior = value;
    }

    public string Strength
    {
        get => _strength;
        set => _strength = value;
    }

    public string? Explanation
    {
        get => _explanation;
        set => _explanation = value;
    }

    public List<string>? ExceptionConditions
    {
        get => string.IsNullOrEmpty(_exceptionConditions)
            ? null
            : JsonSerializer.Deserialize<List<string>>(_exceptionConditions);
        set => _exceptionConditions = value == null ? null : JsonSerializer.Serialize(value);
    }

    // Dapper mappings for snake_case database columns
    public int id
    {
        get => _id;
        set => _id = value;
    }

    public int character_id
    {
        get => _characterId;
        set => _characterId = value;
    }

    public string negative_category
    {
        get => _negativeCategory;
        set => _negativeCategory = value;
    }

    public string negative_behavior
    {
        get => _negativeBehavior;
        set => _negativeBehavior = value;
    }

    public string strength
    {
        get => _strength;
        set => _strength = value;
    }

    public string? explanation
    {
        get => _explanation;
        set => _explanation = value;
    }

    public string? exception_conditions
    {
        get => _exceptionConditions;
        set => _exceptionConditions = value;
    }
}
