using KateMorrisonMCP.Data.Models;

namespace KateMorrisonMCP.Data.Repositories;

public interface IPossessionRepository
{
    Task<Possession?> FindByItemNameAsync(string itemName);
    Task<IEnumerable<Possession>> GetByOwnerAsync(int ownerId);
    Task<int> InsertAsync(Possession possession);
}
