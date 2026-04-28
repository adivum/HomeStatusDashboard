using Core.Constants;
using Core.Interfaces;
using Core.Models;
using Core.Models.EntityViews;
using Core.Utilities;

namespace Core.Services
{
    public class EntityViewFactory : IEntityViewFactory
    {
        private readonly IEntityTypeDetector _typeDetector;

        public EntityViewFactory(IEntityTypeDetector typeDetector)
        {
            _typeDetector = typeDetector;
        }

        public IEntityView CreateView(HaState entity)
        {
            var entityType = _typeDetector.Detect(entity);

            return entityType switch
            {
                EntityType.Temperature  => CreateNumericView(entity, EntityType.Temperature),
                EntityType.Humidity     => CreateNumericView(entity, EntityType.Humidity),
                EntityType.Pressure     => CreateNumericView(entity, EntityType.Pressure),
                EntityType.NumericSensor => CreateNumericView(entity, EntityType.NumericSensor),
                EntityType.Timestamp    => CreateTimestampView(entity),
                EntityType.BinarySensor => CreateBinaryView<BinaryEntityView>(entity, EntityType.BinarySensor),
                EntityType.Switch       => CreateBinaryView<SwitchEntityView>(entity, EntityType.Switch),
                EntityType.Light        => CreateLightView(entity),
                _                       => CreateBinaryView<BinaryEntityView>(entity, EntityType.Generic)
            };
        }

        private NumericEntityView CreateNumericView(HaState entity, EntityType type)
        {
            return new NumericEntityView
            {
                EntityId = entity.EntityId,
                FriendlyName = GetFriendlyName(entity),
                State = entity.State ?? string.Empty,
                Icon = GetIcon(entity),
                LastUpdated = entity.LastUpdated,
                LastChanged = entity.LastChanged,
                Type = type,
                Attributes = entity.Attributes,
                Value = ParseNumericValue(entity.State),
                Unit = GetUnit(entity)
            };
        }

        private TView CreateBinaryView<TView>(HaState entity, EntityType type)
            where TView : BinaryEntityView, new()
        {
            return new TView
            {
                EntityId = entity.EntityId,
                FriendlyName = GetFriendlyName(entity),
                State = entity.State ?? string.Empty,
                Icon = GetIcon(entity),
                LastUpdated = entity.LastUpdated,
                LastChanged = entity.LastChanged,
                Type = type,
                Attributes = entity.Attributes,
                IsOn = IsStateOn(entity.State)
            };
        }

        private LightEntityView CreateLightView(HaState entity)
        {
            return new LightEntityView
            {
                EntityId = entity.EntityId,
                FriendlyName = GetFriendlyName(entity),
                State = entity.State ?? string.Empty,
                Icon = GetIcon(entity),
                LastUpdated = entity.LastUpdated,
                LastChanged = entity.LastChanged,
                Type = EntityType.Light,
                Attributes = entity.Attributes,
                IsOn = IsStateOn(entity.State),
                Brightness = GetBrightness(entity)
            };
        }

        private TimestampEntityView CreateTimestampView(HaState entity)
        {
            DateTime? timestamp = DateTime.TryParse(entity.State, out var parsedDate) ? parsedDate : null;

            return new TimestampEntityView
            {
                EntityId = entity.EntityId,
                FriendlyName = GetFriendlyName(entity),
                State = entity.State ?? string.Empty,
                Icon = GetIcon(entity),
                LastUpdated = entity.LastUpdated,
                LastChanged = entity.LastChanged,
                Type = EntityType.Timestamp,
                Attributes = entity.Attributes,
                Timestamp = timestamp
            };
        }

        private string GetFriendlyName(HaState entity)
        {
            if (entity == null)
                return "Unknown";

            var friendlyName = HaStateExtensions.GetAttributeValueCaseInsensitive(entity, HomeAssistantAttributeKeys.FriendlyName)?.ToString()?.Trim();
            if (!string.IsNullOrEmpty(friendlyName))
                return friendlyName;

            if (!string.IsNullOrEmpty(entity.EntityId))
            {
                var parts = entity.EntityId.Split('.');
                if (parts.Length == 2)
                    return $"{parts[0].ToUpperInvariant()} {FormatName(parts[1])}";
            }

            return entity.EntityId ?? "Unknown";
        }

        private string? GetUnit(HaState entity)
            => HaStateExtensions.GetAttributeValueCaseInsensitive(entity, HomeAssistantAttributeKeys.UnitOfMeasurement)?.ToString();

        private string? GetIcon(HaState entity)
            => HaStateExtensions.GetAttributeValueCaseInsensitive(entity, "icon")?.ToString();

        private bool IsStateOn(string? state)
        {
            if (string.IsNullOrEmpty(state))
                return false;

            var lowerState = state.ToLower();
            return HomeAssistantStateValues.BinaryValues.Contains(lowerState) && 
                   (lowerState == HomeAssistantStateValues.On || 
                    lowerState == HomeAssistantStateValues.True || 
                    lowerState == HomeAssistantStateValues.Open ||
                    lowerState == HomeAssistantStateValues.Locked ||
                    lowerState == HomeAssistantStateValues.Motion);
        }

        private decimal ParseNumericValue(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;

            if (decimal.TryParse(value, out var numericValue))
                return Math.Round(numericValue, 2);

            return 0;
        }

        private int? GetBrightness(HaState entity)
        {
            var brightness = HaStateExtensions.GetAttributeValueCaseInsensitive(entity, HomeAssistantAttributeKeys.Brightness);
            if (brightness is int intValue)
                return intValue;
            if (int.TryParse(brightness?.ToString(), out var parsed))
                return parsed;
            return null;
        }

        private string FormatName(string name)
            => System.Text.RegularExpressions.Regex.Replace(name, "(?:^|_)(.)", m => " " + m.Groups[1].Value.ToUpper()).Trim();
    }
}
