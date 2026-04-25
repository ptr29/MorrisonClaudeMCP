using KateMorrisonMCP.Data.Models;

namespace KateMorrisonMCP.Data.Repositories;

/// <summary>
/// HIGH PRIORITY REPOSITORY: Prevents timing errors like Paul's 7 AM vs 9 AM work start
/// </summary>
public class ScheduleRepository : IScheduleRepository
{
    private readonly DatabaseContext _db;

    public ScheduleRepository(DatabaseContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Schedule>> GetByCharacterAsync(int characterId, string? scheduleType = null)
    {
        var sql = scheduleType == null
            ? "SELECT * FROM schedules WHERE character_id = @CharacterId AND is_current = 1 ORDER BY start_time"
            : "SELECT * FROM schedules WHERE character_id = @CharacterId AND schedule_type = @ScheduleType AND is_current = 1 ORDER BY start_time";

        return await _db.QueryAsync<Schedule>(sql, new { CharacterId = characterId, ScheduleType = scheduleType });
    }

    public async Task<int> InsertAsync(Schedule schedule)
    {
        var sql = @"
            INSERT INTO schedules
            (character_id, schedule_type, schedule_name, days_of_week, start_time, end_time,
             duration_minutes, location_id, location_description, description, exceptions,
             effective_from, effective_until, is_current)
            VALUES
            (@character_id, @schedule_type, @schedule_name, @days_of_week, @start_time, @end_time,
             @duration_minutes, @location_id, @location_description, @description, @exceptions,
             @effective_from, @effective_until, @is_current)";

        return await _db.ExecuteAsync(sql, schedule);
    }
}
