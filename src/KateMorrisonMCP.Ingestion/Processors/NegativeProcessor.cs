using KateMorrisonMCP.Data;
using KateMorrisonMCP.Ingestion.Helpers;
using KateMorrisonMCP.Ingestion.Parsing;

namespace KateMorrisonMCP.Ingestion.Processors;

/// <summary>
/// Processes negative behavior tags
/// </summary>
public class NegativeProcessor : ITagProcessor
{
    private readonly DatabaseContext _db;
    private readonly CharacterLookup _characterLookup;

    public string TagType => "negative";
    public int Priority => 4; // Negatives depend on characters

    private static readonly string[] ValidStrengths = ["absolute", "strong", "preference"];
    private static readonly string[] ValidCategories = ["exercise", "food", "social", "work", "behavior"];

    public NegativeProcessor(DatabaseContext db, CharacterLookup characterLookup)
    {
        _db = db;
        _characterLookup = characterLookup;
    }

    public async Task<int> ProcessAsync(CanonicalTag tag)
    {
        var characterName = tag.GetRequired("character");
        var negativeBehavior = tag.GetRequired("negative_behavior");

        // Look up character_id
        var characterId = await _characterLookup.GetRequiredIdAsync(characterName, tag.SourceFile);

        // Validate strength (required, must be absolute|strong|preference)
        var strength = tag.GetOptional("strength");
        if (string.IsNullOrWhiteSpace(strength) || !ValidStrengths.Contains(strength.ToLowerInvariant()))
        {
            throw new ArgumentException($"Invalid or missing strength value. Must be one of: {string.Join(", ", ValidStrengths)}");
        }

        // Validate category (should be valid, but only warn)
        var category = tag.GetOptional("category");
        if (!string.IsNullOrWhiteSpace(category) && !ValidCategories.Contains(category.ToLowerInvariant()))
        {
            Console.WriteLine($"Warning: Unusual category '{category}' in {tag.SourceFile}:{tag.LineNumber}. Expected: {string.Join(", ", ValidCategories)}");
        }

        // Check if negative exists
        var existingId = await _db.QuerySingleOrDefaultAsync<int?>(
            "SELECT id FROM character_negatives WHERE character_id = @CharacterId AND LOWER(negative_behavior) = LOWER(@NegativeBehavior)",
            new { CharacterId = characterId, NegativeBehavior = negativeBehavior });

        if (existingId.HasValue)
        {
            // Update existing negative
            await _db.ExecuteAsync(@"
                UPDATE character_negatives SET
                    strength = @Strength,
                    negative_category = COALESCE(@NegativeCategory, negative_category),
                    explanation = COALESCE(@Explanation, explanation),
                    source_file = @SourceFile
                WHERE id = @Id",
                new
                {
                    Id = existingId.Value,
                    Strength = strength.ToLowerInvariant(),
                    NegativeCategory = category?.ToLowerInvariant(),
                    Explanation = tag.GetOptional("context_notes"),
                    SourceFile = tag.SourceFile
                });

            return existingId.Value;
        }
        else
        {
            // Insert new negative
            await _db.ExecuteAsync(@"
                INSERT INTO character_negatives (
                    character_id, negative_behavior, strength, negative_category, explanation, source_file
                ) VALUES (
                    @CharacterId, @NegativeBehavior, @Strength, @NegativeCategory, @Explanation, @SourceFile
                )",
                new
                {
                    CharacterId = characterId,
                    NegativeBehavior = negativeBehavior,
                    Strength = strength.ToLowerInvariant(),
                    NegativeCategory = category?.ToLowerInvariant() ?? "behavior",
                    Explanation = tag.GetOptional("context_notes"),
                    SourceFile = tag.SourceFile
                });

            var newId = await _db.QuerySingleOrDefaultAsync<int>(
                "SELECT last_insert_rowid()");

            return newId;
        }
    }
}
