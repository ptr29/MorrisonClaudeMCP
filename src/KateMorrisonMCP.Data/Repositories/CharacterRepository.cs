using KateMorrisonMCP.Data.Models;

namespace KateMorrisonMCP.Data.Repositories;

public class CharacterRepository : ICharacterRepository
{
    private readonly DatabaseContext _db;

    public CharacterRepository(DatabaseContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Fuzzy name matching: handles "Kate" vs "Katherine Marie Morrison"
    /// </summary>
    public async Task<Character?> FindByNameAsync(string name)
    {
        // Try exact match first (full name or preferred name)
        var sql = @"
            SELECT * FROM characters
            WHERE full_name = @Name OR preferred_name = @Name
            LIMIT 1";

        var character = await _db.QuerySingleOrDefaultAsync<Character>(sql, new { Name = name });

        if (character != null)
            return character;

        // Try fuzzy match (contains)
        sql = @"
            SELECT * FROM characters
            WHERE full_name LIKE '%' || @Name || '%' OR preferred_name LIKE '%' || @Name || '%'
            LIMIT 1";

        return await _db.QuerySingleOrDefaultAsync<Character>(sql, new { Name = name });
    }

    public async Task<Character?> GetByIdAsync(int id)
    {
        var sql = "SELECT * FROM characters WHERE id = @Id";
        return await _db.QuerySingleOrDefaultAsync<Character>(sql, new { Id = id });
    }

    /// <summary>
    /// Return name suggestions for "not found" errors
    /// </summary>
    public async Task<IEnumerable<string>> SearchNamesAsync(string query)
    {
        var sql = @"
            SELECT COALESCE(preferred_name, full_name) as name
            FROM characters
            WHERE full_name LIKE '%' || @Query || '%' OR preferred_name LIKE '%' || @Query || '%'
            LIMIT 5";

        return await _db.QueryAsync<string>(sql, new { Query = query });
    }

    public async Task<int> InsertAsync(Character character)
    {
        var sql = @"
            INSERT INTO characters
            (full_name, preferred_name, age, birthday, birth_year, height_inches, weight_lbs,
             build, hair_color, hair_length, eye_color, distinctive_features, occupation,
             employer, job_title, work_location, work_schedule_type, phone, email,
             residence_id, character_type, is_alive)
            VALUES
            (@full_name, @preferred_name, @age, @birthday, @birth_year, @height_inches, @weight_lbs,
             @build, @hair_color, @hair_length, @eye_color, @distinctive_features, @occupation,
             @employer, @job_title, @work_location, @work_schedule_type, @phone, @email,
             @residence_id, @character_type, @is_alive)";

        return await _db.ExecuteAsync(sql, character);
    }
}
