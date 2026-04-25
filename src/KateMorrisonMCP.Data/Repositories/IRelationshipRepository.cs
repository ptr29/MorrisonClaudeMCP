using KateMorrisonMCP.Data.Models;

namespace KateMorrisonMCP.Data.Repositories;

public interface IRelationshipRepository
{
    Task<IEnumerable<Relationship>> GetBetweenCharactersAsync(int characterAId, int characterBId);
    Task<int> InsertAsync(Relationship relationship);
}
