using KateMorrisonMCP.Data.Models;

namespace KateMorrisonMCP.Data.Repositories;

public class PossessionRepository : IPossessionRepository
{
    private readonly DatabaseContext _db;

    public PossessionRepository(DatabaseContext db)
    {
        _db = db;
    }

    public async Task<Possession?> FindByItemNameAsync(string itemName)
    {
        var sql = @"
            SELECT * FROM possessions
            WHERE item_name LIKE '%' || @ItemName || '%' AND is_current = 1
            LIMIT 1";

        return await _db.QuerySingleOrDefaultAsync<Possession>(sql, new { ItemName = itemName });
    }

    public async Task<IEnumerable<Possession>> GetByOwnerAsync(int ownerId)
    {
        var sql = "SELECT * FROM possessions WHERE owner_id = @OwnerId AND is_current = 1";
        return await _db.QueryAsync<Possession>(sql, new { OwnerId = ownerId });
    }

    public async Task<int> InsertAsync(Possession possession)
    {
        var sql = @"
            INSERT INTO possessions
            (owner_id, item_name, item_category, item_description, acquisition_date,
             acquisition_method, acquisition_from, location_id, storage_location,
             monetary_value, sentimental_value, significance_notes, is_current)
            VALUES
            (@owner_id, @item_name, @item_category, @item_description, @acquisition_date,
             @acquisition_method, @acquisition_from, @location_id, @storage_location,
             @monetary_value, @sentimental_value, @significance_notes, @is_current)";

        return await _db.ExecuteAsync(sql, possession);
    }
}
