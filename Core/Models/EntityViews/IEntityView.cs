namespace Core.Models.EntityViews
{
    public interface IEntityView
    {
        string EntityId { get; }
        string FriendlyName { get; }
        string State { get; }
        string? Icon { get; }
        DateTime LastUpdated { get; }
        DateTime? LastChanged { get; }
        EntityType Type { get; }
        IReadOnlyDictionary<string, object?> Attributes { get; }
    }
}
