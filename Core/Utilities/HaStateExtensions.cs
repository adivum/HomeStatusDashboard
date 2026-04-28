using Core.Models;

namespace Core.Utilities;

public static class HaStateExtensions
{
    public static object? GetAttributeValueCaseInsensitive(HaState entity, string key)
    {
        if (entity?.Attributes == null || string.IsNullOrEmpty(key))
            return null;

        var kvp = entity.Attributes.FirstOrDefault(x =>
            x.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

        return kvp.Value;
    }

    public static string? GetAttributeString(HaState entity, string key)
        => GetAttributeValueCaseInsensitive(entity, key)?.ToString();
}