using KateMorrisonMCP.Data.Models;

namespace KateMorrisonMCP.Data.Repositories;

public interface ICharacterRepository
{
    Task<Character?> FindByNameAsync(string name);
    Task<Character?> GetByIdAsync(int id);
    Task<IEnumerable<string>> SearchNamesAsync(string query);
    Task<int> InsertAsync(Character character);
}
