using System.Text.Json;
using KateMorrisonMCP.Data.Repositories;
using KateMorrisonMCP.Tools.Models;

namespace KateMorrisonMCP.Tools.Tools;

/// <summary>
/// Returns structured character data with optional category filtering
/// </summary>
public class GetCharacterFactsTool : ITool
{
    private readonly ICharacterRepository _characterRepo;

    public GetCharacterFactsTool(ICharacterRepository characterRepo)
    {
        _characterRepo = characterRepo;
    }

    public string Name => "get_character_facts";

    public string Description =>
        "Retrieves structured character data including biographical info, physical description, occupation, and residence. " +
        "Supports fuzzy name matching (e.g., 'Kate' matches 'Katherine Marie Morrison').";

    public ToolInputSchema InputSchema => new()
    {
        Type = "object",
        Properties = new Dictionary<string, PropertySchema>
        {
            ["character_name"] = new PropertySchema
            {
                Type = "string",
                Description = "Full name or preferred name of character"
            },
            ["category"] = new PropertySchema
            {
                Type = "string",
                Description = "Optional: Filter to specific category",
                Enum = new List<string> { "biographical", "physical", "occupation", "all" }
            }
        },
        Required = new List<string> { "character_name" }
    };

    public async Task<object> ExecuteAsync(JsonElement? arguments)
    {
        if (!arguments.HasValue)
        {
            return new { success = false, error = "Missing arguments" };
        }

        var args = arguments.Value;
        var characterName = args.TryGetProperty("character_name", out var charProp) ? charProp.GetString() : null;
        var category = args.TryGetProperty("category", out var cat) ? cat.GetString() : "all";

        if (string.IsNullOrEmpty(characterName))
        {
            return new { success = false, error = "Missing character_name" };
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

        // Build response based on category
        var response = new Dictionary<string, object>
        {
            ["success"] = true,
            ["character"] = new Dictionary<string, object>
            {
                ["id"] = character.Id,
                ["full_name"] = character.FullName,
                ["preferred_name"] = character.PreferredName ?? ""
            }
        };

        var characterData = (Dictionary<string, object>)response["character"];

        if (category == "all" || category == "biographical")
        {
            characterData["biographical"] = new
            {
                age = character.Age,
                birthday = character.Birthday,
                birth_year = character.BirthYear
            };
        }

        if (category == "all" || category == "physical")
        {
            characterData["physical"] = new
            {
                height_inches = character.HeightInches,
                height_formatted = character.FormattedHeight,
                weight_lbs = character.WeightLbs,
                build = character.Build,
                hair_color = character.HairColor,
                hair_length = character.HairLength,
                eye_color = character.EyeColor,
                distinctive_features = character.DistinctiveFeatures
            };
        }

        if (category == "all" || category == "occupation")
        {
            characterData["occupation"] = new
            {
                occupation = character.Occupation,
                employer = character.Employer,
                job_title = character.JobTitle,
                work_schedule_type = character.WorkScheduleType
            };
        }

        return response;
    }
}
