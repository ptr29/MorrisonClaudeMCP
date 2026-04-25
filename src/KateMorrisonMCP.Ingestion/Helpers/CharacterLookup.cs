using KateMorrisonMCP.Data;
using KateMorrisonMCP.Data.Models;

namespace KateMorrisonMCP.Ingestion.Helpers;

/// <summary>
/// Case-insensitive character lookup helper
/// </summary>
public class CharacterLookup
{
    private readonly DatabaseContext _db;

    public CharacterLookup(DatabaseContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Finds a character by full name (case-insensitive)
    /// </summary>
    public async Task<Character?> FindByNameAsync(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return null;
        }

        var character = await _db.QuerySingleOrDefaultAsync<Character>(
            "SELECT * FROM characters WHERE LOWER(full_name) = LOWER(@FullName)",
            new { FullName = fullName.Trim() });

        return character;
    }

    /// <summary>
    /// Finds a character by full name and throws if not found
    /// </summary>
    public async Task<Character> GetRequiredAsync(string fullName, string context = "")
    {
        var character = await FindByNameAsync(fullName);

        if (character == null)
        {
            var message = string.IsNullOrEmpty(context)
                ? $"Character not found: {fullName}"
                : $"Character not found: {fullName} (referenced in {context})";
            throw new ArgumentException(message);
        }

        return character;
    }

    /// <summary>
    /// Gets character ID by name, returns null if not found
    /// </summary>
    public async Task<int?> GetIdAsync(string fullName)
    {
        var character = await FindByNameAsync(fullName);
        return character?.Id;
    }

    /// <summary>
    /// Gets character ID by name, throws if not found
    /// </summary>
    public async Task<int> GetRequiredIdAsync(string fullName, string context = "")
    {
        var character = await GetRequiredAsync(fullName, context);
        return character.Id;
    }

    /// <summary>
    /// Checks if a character exists
    /// </summary>
    public async Task<bool> ExistsAsync(string fullName)
    {
        var character = await FindByNameAsync(fullName);
        return character != null;
    }
}
