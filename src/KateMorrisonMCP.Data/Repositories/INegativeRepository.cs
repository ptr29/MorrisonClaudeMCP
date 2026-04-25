using KateMorrisonMCP.Data.Models;

namespace KateMorrisonMCP.Data.Repositories;

/// <summary>
/// Repository for character negatives (CRITICAL for error prevention)
/// </summary>
public interface INegativeRepository
{
    Task<IEnumerable<CharacterNegative>> GetByCharacterAsync(int characterId, string? category = null);
    Task<CharacterNegative?> FindMatchingNegativeAsync(int characterId, string behavior);
    Task<int> InsertAsync(CharacterNegative negative);
}
