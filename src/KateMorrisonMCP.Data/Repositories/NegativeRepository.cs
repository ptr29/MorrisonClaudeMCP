using KateMorrisonMCP.Data.Models;

namespace KateMorrisonMCP.Data.Repositories;

/// <summary>
/// CRITICAL REPOSITORY: Implements semantic matching for character negatives
/// Prevents narrative errors like "Kate goes to gym" when she explicitly does NOT
/// </summary>
public class NegativeRepository : INegativeRepository
{
    private readonly DatabaseContext _db;

    public NegativeRepository(DatabaseContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<CharacterNegative>> GetByCharacterAsync(int characterId, string? category = null)
    {
        var sql = category == null
            ? "SELECT * FROM character_negatives WHERE character_id = @CharacterId"
            : "SELECT * FROM character_negatives WHERE character_id = @CharacterId AND negative_category = @Category";

        return await _db.QueryAsync<CharacterNegative>(sql, new { CharacterId = characterId, Category = category });
    }

    /// <summary>
    /// CRITICAL METHOD: Semantic matching for behavior violations
    /// Example: "goes to gym" should match "Does NOT go to gyms"
    /// </summary>
    public async Task<CharacterNegative?> FindMatchingNegativeAsync(int characterId, string behavior)
    {
        var negatives = await GetByCharacterAsync(characterId);

        foreach (var negative in negatives)
        {
            if (BehaviorMatches(behavior, negative.NegativeBehavior))
            {
                return negative;
            }
        }

        return null;
    }

    /// <summary>
    /// Semantic matching using keyword intersection
    /// "goes to gym" matches "Does NOT go to gyms" because keywords [go, gym] overlap
    /// </summary>
    private bool BehaviorMatches(string behavior, string negativeBehavior)
    {
        var behaviorKeywords = ExtractKeywords(behavior);
        var negativeKeywords = ExtractKeywords(negativeBehavior);

        // Match if at least 2 keywords overlap (or 1 if one behavior has only 1 keyword)
        var intersection = behaviorKeywords.Intersect(negativeKeywords).ToList();

        // Handle single keyword cases (e.g., "gym" should match "gyms")
        if (behaviorKeywords.Count == 1 || negativeKeywords.Count == 1)
        {
            return intersection.Count >= 1;
        }

        // For multi-word behaviors, require at least 2 matching keywords
        return intersection.Count >= 2;
    }

    /// <summary>
    /// Extract meaningful keywords from behavior text
    /// Removes stop words and normalizes to singular/base form
    /// </summary>
    private HashSet<string> ExtractKeywords(string text)
    {
        var stopWords = new HashSet<string>
        {
            "does", "not", "do", "to", "the", "a", "an", "is", "are",
            "was", "were", "be", "been", "being", "have", "has", "had",
            "on", "at", "in", "for", "with", "of", "from", "by"
        };

        return text
            .ToLowerInvariant()
            .Split(new[] { ' ', ',', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(w => w.Trim())
            .Where(w => !stopWords.Contains(w))
            .Select(NormalizePlural) // gym/gyms both become gym
            .ToHashSet();
    }

    /// <summary>
    /// Normalizes words to base form (handles plurals and common verb conjugations)
    /// </summary>
    private string NormalizePlural(string word)
    {
        // Handle common verb conjugations (goes → go, runs → run, eats → eat)
        if (word.EndsWith("es") && word.Length > 3)
        {
            // Check if removing 'es' gives us a valid base form
            var withoutEs = word.Substring(0, word.Length - 2);
            // goes → go, does → do, watches → watch
            return withoutEs;
        }

        // Handle plural -ies → -y (berries → berry, flies → fly)
        if (word.EndsWith("ies") && word.Length > 4)
            return word.Substring(0, word.Length - 3) + "y";

        // Handle regular plurals (gyms → gym, treadmills → treadmill)
        if (word.EndsWith("s") && word.Length > 2 && !word.EndsWith("ss"))
            return word.Substring(0, word.Length - 1);

        return word;
    }

    public async Task<int> InsertAsync(CharacterNegative negative)
    {
        var sql = @"
            INSERT INTO character_negatives
            (character_id, negative_category, negative_behavior, strength, explanation, exception_conditions)
            VALUES
            (@character_id, @negative_category, @negative_behavior, @strength, @explanation, @exception_conditions)";

        return await _db.ExecuteAsync(sql, negative);
    }
}
