using System.Text.Json;
using KateMorrisonMCP.Data.Repositories;
using KateMorrisonMCP.Tools.Models;

namespace KateMorrisonMCP.Tools.Tools;

/// <summary>
/// CRITICAL TOOL: Checks if a behavior violates a character's explicit negatives
/// Prevents narrative errors like "Kate goes to gym" when she explicitly does NOT
/// </summary>
public class CheckNegativeTool : ITool
{
    private readonly INegativeRepository _negativeRepo;
    private readonly ICharacterRepository _characterRepo;

    public CheckNegativeTool(INegativeRepository negativeRepo, ICharacterRepository characterRepo)
    {
        _negativeRepo = negativeRepo;
        _characterRepo = characterRepo;
    }

    public string Name => "check_negative";

    public string Description =>
        "CRITICAL ERROR PREVENTION: Checks if a behavior is explicitly something a character does NOT do. " +
        "Always call this before writing scenes involving exercise, food choices, social behaviors, or daily routines. " +
        "Example: 'Kate goes to gym' will return a violation because Kate does NOT go to gyms.";

    public ToolInputSchema InputSchema => new()
    {
        Type = "object",
        Properties = new Dictionary<string, PropertySchema>
        {
            ["character_name"] = new PropertySchema
            {
                Type = "string",
                Description = "Name of the character (e.g., 'Kate', 'Paul')"
            },
            ["behavior"] = new PropertySchema
            {
                Type = "string",
                Description = "The behavior to check (e.g., 'goes to gym', 'eats pasta', 'runs on treadmill')"
            },
            ["category"] = new PropertySchema
            {
                Type = "string",
                Description = "Optional category to narrow search",
                Enum = new List<string> { "exercise", "food", "social", "work", "behavior" }
            }
        },
        Required = new List<string> { "character_name", "behavior" }
    };

    public async Task<object> ExecuteAsync(JsonElement? arguments)
    {
        if (!arguments.HasValue)
        {
            return new { success = false, error = "Missing arguments" };
        }

        var args = arguments.Value;
        var characterName = args.TryGetProperty("character_name", out var charProp) ? charProp.GetString() : null;
        var behavior = args.TryGetProperty("behavior", out var behaviorProp) ? behaviorProp.GetString() : null;
        var category = args.TryGetProperty("category", out var cat) ? cat.GetString() : null;

        if (string.IsNullOrEmpty(characterName) || string.IsNullOrEmpty(behavior))
        {
            return new { success = false, error = "Missing required parameters" };
        }

        // Find character
        var character = await _characterRepo.FindByNameAsync(characterName);
        if (character == null)
        {
            var suggestions = await _characterRepo.SearchNamesAsync(characterName);
            return new
            {
                success = false,
                error = "Character not found",
                suggestions = suggestions.ToList()
            };
        }

        // Search for matching negative
        var matchingNegative = await _negativeRepo.FindMatchingNegativeAsync(character.Id, behavior);

        if (matchingNegative != null)
        {
            // VIOLATION FOUND
            var hasExceptions = matchingNegative.ExceptionConditions?.Count > 0;
            var strengthLabel = matchingNegative.Strength == "absolute" ? "an ABSOLUTE" : "a STRONG";

            return new
            {
                success = true,
                is_negative = true,
                violation = new
                {
                    character = character.PreferredName ?? character.FullName,
                    behavior_checked = behavior,
                    matching_negative = matchingNegative.NegativeBehavior,
                    strength = matchingNegative.Strength,
                    explanation = matchingNegative.Explanation,
                    exceptions = matchingNegative.ExceptionConditions
                },
                warning = $"⚠️ CANONICAL VIOLATION: {character.PreferredName ?? character.FullName} " +
                         $"{matchingNegative.NegativeBehavior}. " +
                         $"This is {strengthLabel} negative" +
                         (hasExceptions ? " with limited exceptions." : " with NO exceptions.")
            };
        }

        // No violation - return related negatives for context
        var allNegatives = await _negativeRepo.GetByCharacterAsync(character.Id, category);
        var relatedNegatives = allNegatives
            .Select(n => new { behavior = n.NegativeBehavior, category = n.NegativeCategory })
            .ToList();

        return new
        {
            success = true,
            is_negative = false,
            message = "No canonical negative found for this behavior",
            related_negatives = relatedNegatives
        };
    }
}
