using Core.Models.EntityViews;

namespace Core.Interfaces
{
    public interface IHomeAssistantService
    {
        Task<IEnumerable<IEntityView>> GetDashboardViewsAsync(CancellationToken ct = default);
        Task<IEnumerable<IEntityView>> GetEntitiesListViewsAsync(CancellationToken ct = default);
        Task<IEntityView?> GetEntityViewAsync(string entityId, CancellationToken ct = default);
    }
}