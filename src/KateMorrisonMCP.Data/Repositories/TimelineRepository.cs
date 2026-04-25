using KateMorrisonMCP.Data.Models;

namespace KateMorrisonMCP.Data.Repositories;

public class TimelineRepository : ITimelineRepository
{
    private readonly DatabaseContext _db;

    public TimelineRepository(DatabaseContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<TimelineEvent>> GetByDateAsync(string date)
    {
        var sql = "SELECT * FROM timeline_events WHERE event_date = @Date ORDER BY event_date";
        return await _db.QueryAsync<TimelineEvent>(sql, new { Date = date });
    }

    public async Task<IEnumerable<TimelineEvent>> GetByDateRangeAsync(string startDate, string endDate)
    {
        var sql = @"
            SELECT * FROM timeline_events
            WHERE event_date >= @StartDate AND event_date <= @EndDate
            ORDER BY event_date";

        return await _db.QueryAsync<TimelineEvent>(sql, new { StartDate = startDate, EndDate = endDate });
    }

    public async Task<IEnumerable<TimelineEvent>> GetByCharacterAsync(int characterId)
    {
        var sql = @"
            SELECT te.* FROM timeline_events te
            JOIN event_participants ep ON te.id = ep.event_id
            WHERE ep.character_id = @CharacterId
            ORDER BY te.event_date";

        return await _db.QueryAsync<TimelineEvent>(sql, new { CharacterId = characterId });
    }

    public async Task<int> InsertEventAsync(TimelineEvent timelineEvent)
    {
        var sql = @"
            INSERT INTO timeline_events
            (event_date, event_date_precision, event_title, event_description,
             location_id, location_description, event_category, significance,
             is_canonical, source_file)
            VALUES
            (@event_date, @event_date_precision, @event_title, @event_description,
             @location_id, @location_description, @event_category, @significance,
             @is_canonical, @source_file);
            SELECT last_insert_rowid();";

        return await _db.QuerySingleAsync<int>(sql, timelineEvent);
    }

    public async Task<int> InsertParticipantAsync(EventParticipant participant)
    {
        var sql = @"
            INSERT INTO event_participants (event_id, character_id, role)
            VALUES (@event_id, @character_id, @role)";

        return await _db.ExecuteAsync(sql, participant);
    }
}
