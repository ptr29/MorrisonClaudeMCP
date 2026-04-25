using KateMorrisonMCP.Data.Models;

namespace KateMorrisonMCP.Data.Repositories;

public interface ILocationRepository
{
    Task<Location?> GetByNameOrAddressAsync(string nameOrAddress);
    Task<IEnumerable<LocationRoom>> GetRoomsByLocationAsync(int locationId, string? floor = null);
    Task<int> InsertLocationAsync(Location location);
    Task<int> InsertRoomAsync(LocationRoom room);
}
