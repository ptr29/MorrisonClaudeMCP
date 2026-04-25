using KateMorrisonMCP.Data.Models;

namespace KateMorrisonMCP.Data.Repositories;

public interface ITimelineRepository
{
    Task<IEnumerable<TimelineEvent>> GetByDateAsync(string date);
    Task<IEnumerable<TimelineEvent>> GetByDateRangeAsync(string startDate, string endDate);
    Task<IEnumerable<TimelineEvent>> GetByCharacterAsync(int characterId);
    Task<int> InsertEventAsync(TimelineEvent timelineEvent);
    Task<int> InsertParticipantAsync(EventParticipant participant);
}
