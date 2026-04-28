namespace Core.Models.EntityViews
{
    public abstract class EntityViewBase : IEntityView
    {
        public string EntityId { get; set; } = string.Empty;
        public string FriendlyName { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime? LastChanged { get; set; }
        public EntityType Type { get; set; }
        public IReadOnlyDictionary<string, object?> Attributes { get; init; } = new Dictionary<string, object?>();
    }
}
