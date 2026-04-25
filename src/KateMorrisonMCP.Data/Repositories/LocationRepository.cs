using KateMorrisonMCP.Data.Models;

namespace KateMorrisonMCP.Data.Repositories;

public class LocationRepository : ILocationRepository
{
    private readonly DatabaseContext _db;

    public LocationRepository(DatabaseContext db)
    {
        _db = db;
    }

    public async Task<Location?> GetByNameOrAddressAsync(string nameOrAddress)
    {
        var sql = @"
            SELECT * FROM locations
            WHERE name LIKE '%' || @Query || '%'
               OR address_street LIKE '%' || @Query || '%'
               OR address_city LIKE '%' || @Query || '%'
            LIMIT 1";

        return await _db.QuerySingleOrDefaultAsync<Location>(sql, new { Query = nameOrAddress });
    }

    public async Task<IEnumerable<LocationRoom>> GetRoomsByLocationAsync(int locationId, string? floor = null)
    {
        var sql = floor == null
            ? "SELECT * FROM location_rooms WHERE location_id = @LocationId ORDER BY floor_level, room_name"
            : "SELECT * FROM location_rooms WHERE location_id = @LocationId AND floor_level = @Floor ORDER BY room_name";

        return await _db.QueryAsync<LocationRoom>(sql, new { LocationId = locationId, Floor = floor });
    }

    public async Task<int> InsertLocationAsync(Location location)
    {
        var sql = @"
            INSERT INTO locations
            (name, address_street, address_city, address_state, address_zip, location_type,
             building_type, floor_count, unit_number, square_feet, owner_id, ownership_type,
             monthly_cost, purchase_date, neighborhood, distance_to_chicago_loop,
             nearby_landmarks, is_fictional)
            VALUES
            (@name, @address_street, @address_city, @address_state, @address_zip, @location_type,
             @building_type, @floor_count, @unit_number, @square_feet, @owner_id, @ownership_type,
             @monthly_cost, @purchase_date, @neighborhood, @distance_to_chicago_loop,
             @nearby_landmarks, @is_fictional);
            SELECT last_insert_rowid();";

        return await _db.QuerySingleAsync<int>(sql, location);
    }

    public async Task<int> InsertRoomAsync(LocationRoom room)
    {
        var sql = @"
            INSERT INTO location_rooms
            (location_id, room_name, floor_level, room_type, width_feet, length_feet,
             width_inches, length_inches, current_use, key_features, furniture,
             adjacent_to, has_exterior_window, window_direction)
            VALUES
            (@location_id, @room_name, @floor_level, @room_type, @width_feet, @length_feet,
             @width_inches, @length_inches, @current_use, @key_features, @furniture,
             @adjacent_to, @has_exterior_window, @window_direction)";

        return await _db.ExecuteAsync(sql, room);
    }
}
