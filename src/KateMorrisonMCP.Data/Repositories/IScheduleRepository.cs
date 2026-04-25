using KateMorrisonMCP.Data.Models;

namespace KateMorrisonMCP.Data.Repositories;

/// <summary>
/// Repository for character schedules (CRITICAL for preventing timing errors)
/// </summary>
public interface IScheduleRepository
{
    Task<IEnumerable<Schedule>> GetByCharacterAsync(int characterId, string? scheduleType = null);
    Task<int> InsertAsync(Schedule schedule);
}
