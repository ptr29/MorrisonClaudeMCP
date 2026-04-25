using System.Text.Json;
using KateMorrisonMCP.Data.Repositories;
using KateMorrisonMCP.Tools.Models;

namespace KateMorrisonMCP.Tools.Tools;

/// <summary>
/// Returns relationship information between two characters with milestones
/// </summary>
public class CheckRelationshipTool : ITool
{
    private readonly IRelationshipRepository _relationshipRepo;
    private readonly ICharacterRepository _characterRepo;
    private readonly ITimelineRepository _timelineRepo;

    public CheckRelationshipTool(
        IRelationshipRepository relationshipRepo,
        ICharacterRepository characterRepo,
        ITimelineRepository timelineRepo)
    {
        _relationshipRepo = relationshipRepo;
        _characterRepo = characterRepo;
        _timelineRepo = timelineRepo;
    }

    public string Name => "check_relationship";

    public string Description =>
        "Returns relationship information between two characters including type, status, and key milestones. " +
        "Example: Check relationship between Kate and Paul (romantic_partner, boyfriend_girlfriend since 2025-12-28).";

    public ToolInputSchema InputSchema => new()
    {
        Type = "object",
        Properties = new Dictionary<string, PropertySchema>
        {
            ["character1"] = new PropertySchema
            {
                Type = "string",
                Description = "First character name"
            },
            ["character2"] = new PropertySchema
            {
                Type = "string",
                Description = "Second character name"
            }
        },
        Required = new List<string> { "character1", "character2" }
    };

    public async Task<object> ExecuteAsync(JsonElement? arguments)
    {
        if (!arguments.HasValue)
        {
            return new { success = false, error = "Missing arguments" };
        }

        var args = arguments.Value;
        var char1Name = args.TryGetProperty("character1", out var char1Prop) ? char1Prop.GetString() : null;
        var char2Name = args.TryGetProperty("character2", out var char2Prop) ? char2Prop.GetString() : null;

        if (string.IsNullOrEmpty(char1Name) || string.IsNullOrEmpty(char2Name))
        {
            return new { success = false, error = "Missing required character names" };
        }

        // Find characters
        var char1 = await _characterRepo.FindByNameAsync(char1Name);
        var char2 = await _characterRepo.FindByNameAsync(char2Name);

        if (char1 == null || char2 == null)
        {
            return new
            {
                success = false,
                error = "One or both characters not found"
            };
        }

        // Get relationships
        var relationships = await _relationshipRepo.GetBetweenCharactersAsync(char1.Id, char2.Id);
        var relationshipsList = relationships.ToList();

        if (!relationshipsList.Any())
        {
            return new
            {
                success = true,
                message = "No relationship found between these characters",
                character_a = char1.PreferredName ?? char1.FullName,
                character_b = char2.PreferredName ?? char2.FullName
            };
        }

        // Build relationship objects
        var relationshipObjects = relationshipsList.Select(r => new
        {
            type = r.RelationshipType,
            subtype = r.RelationshipSubtype,
            start_date = r.StartDate,
            end_date = r.EndDate,
            status = r.CurrentStatus,
            notes = r.Notes
        }).ToList();

        return new
        {
            success = true,
            character_a = char1.PreferredName ?? char1.FullName,
            character_b = char2.PreferredName ?? char2.FullName,
            relationships = relationshipObjects
        };
    }
}
