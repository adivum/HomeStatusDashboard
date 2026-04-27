using Core.Models;

namespace Core.Interfaces
{
    public interface IHomeAssistantService
    {
        Task<IEnumerable<HaState>> GetAllStatesAsync(CancellationToken ct = default);
        Task<HaState?> GetEntityAsync(string entityId, CancellationToken ct = default);
    }
}