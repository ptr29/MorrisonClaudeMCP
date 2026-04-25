using KateMorrisonMCP.Data.Models;

namespace KateMorrisonMCP.Data.Repositories;

public class RelationshipRepository : IRelationshipRepository
{
    private readonly DatabaseContext _db;

    public RelationshipRepository(DatabaseContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Relationship>> GetBetweenCharactersAsync(int characterAId, int characterBId)
    {
        var sql = @"
            SELECT * FROM relationships
            WHERE (character_a_id = @CharacterAId AND character_b_id = @CharacterBId)
               OR (character_a_id = @CharacterBId AND character_b_id = @CharacterAId)";

        return await _db.QueryAsync<Relationship>(sql, new { CharacterAId = characterAId, CharacterBId = characterBId });
    }

    public async Task<int> InsertAsync(Relationship relationship)
    {
        var sql = @"
            INSERT INTO relationships
            (character_a_id, character_b_id, relationship_type, relationship_subtype,
             start_date, end_date, current_status, direction, notes)
            VALUES
            (@character_a_id, @character_b_id, @relationship_type, @relationship_subtype,
             @start_date, @end_date, @current_status, @direction, @notes)";

        return await _db.ExecuteAsync(sql, relationship);
    }
}
