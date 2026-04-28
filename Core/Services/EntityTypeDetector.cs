using Core.Constants;
using Core.Interfaces;
using Core.Models;
using Core.Utilities;

namespace Core.Services
{
    public class EntityTypeDetector : IEntityTypeDetector
    {
        private static readonly string[] SpecificDeviceClasses = { "temperature", "humidity", "pressure", "timestamp" };
        private static readonly string[] BinaryDeviceClasses =
        {
            "motion", "occupancy", "presence", "battery", "door", "window",
            "moisture", "smoke", "gas", "vibration", "tamper", "leak", "rain", "connectivity"
        };

        public EntityType Detect(HaState? entity)
        {
            if (entity == null)
                return EntityType.Generic;

            var domain = GetEntityDomain(entity.EntityId);
            var state = entity.State?.ToLower() ?? string.Empty;
            var deviceClass = HaStateExtensions.GetAttributeString(entity, HomeAssistantAttributeKeys.DeviceClass)?.ToLower() ?? string.Empty;
            var stateClass = HaStateExtensions.GetAttributeString(entity, HomeAssistantAttributeKeys.StateClass)?.ToLower() ?? string.Empty;
            var unitOfMeasurement = HaStateExtensions.GetAttributeString(entity, HomeAssistantAttributeKeys.UnitOfMeasurement)?.ToLower() ?? string.Empty;

            if (!string.IsNullOrEmpty(deviceClass))
                return DetectByDeviceClass(deviceClass, state);

            if (!string.IsNullOrEmpty(domain))
            {
                var typeByDomain = DetectByDomain(domain, stateClass, unitOfMeasurement, state);
                if (typeByDomain != EntityType.Generic)
                    return typeByDomain;
            }

            return DetectByState(stateClass, unitOfMeasurement, entity.State, state);
        }

        private EntityType DetectByDeviceClass(string deviceClass, string state)
        {
            if (IsSpecificDeviceClass(deviceClass))
                return deviceClass switch
                {
                    "temperature" => EntityType.Temperature,
                    "humidity" => EntityType.Humidity,
                    "pressure" => EntityType.Pressure,
                    "timestamp" => EntityType.Timestamp,
                    _ => EntityType.Generic
                };

            if (double.TryParse(state, out _))
                return EntityType.NumericSensor;

            if (IsBinaryDeviceClass(deviceClass))
                return EntityType.BinarySensor;

            return EntityType.Generic;
        }

        private EntityType DetectByDomain(string domain, string stateClass, string unitOfMeasurement, string state)
        {
            return domain switch
            {
                HomeAssistantDomains.Light => EntityType.Light,
                HomeAssistantDomains.Switch or HomeAssistantDomains.InputBoolean or HomeAssistantDomains.Automation => EntityType.Switch,
                HomeAssistantDomains.BinarySensor => EntityType.BinarySensor,
                HomeAssistantDomains.Sensor => DetectSensorType(stateClass, unitOfMeasurement, state),
                HomeAssistantDomains.Select or HomeAssistantDomains.InputSelect or HomeAssistantDomains.Climate or
                HomeAssistantDomains.Fan or HomeAssistantDomains.Camera or HomeAssistantDomains.MediaPlayer or
                HomeAssistantDomains.Weather or HomeAssistantDomains.InputNumber or HomeAssistantDomains.InputDatetime => EntityType.NumericSensor,
                _ => EntityType.Generic
            };
        }

        private EntityType DetectSensorType(string stateClass, string unitOfMeasurement, string state)
        {
            if (HomeAssistantStateClasses.NumericStateClasses.Contains(stateClass) || !string.IsNullOrEmpty(unitOfMeasurement))
                return EntityType.NumericSensor;

            if (double.TryParse(state, out _))
                return EntityType.NumericSensor;

            return EntityType.Generic;
        }

        private EntityType DetectByState(string stateClass, string unitOfMeasurement, string? originalState, string state)
        {
            if (HomeAssistantStateClasses.NumericStateClasses.Contains(stateClass))
                return EntityType.NumericSensor;

            if (!string.IsNullOrEmpty(unitOfMeasurement))
                return EntityType.NumericSensor;

            if (HomeAssistantStateValues.BinaryValues.Contains(state))
                return EntityType.BinarySensor;

            if (DateTime.TryParse(originalState, out _))
                return EntityType.Timestamp;

            if (double.TryParse(state, out _))
                return EntityType.NumericSensor;

            return EntityType.Generic;
        }

        private string GetEntityDomain(string entityId)
        {
            var parts = entityId.Split('.');
            return parts.Length > 0 ? parts[0] : string.Empty;
        }

        private bool IsSpecificDeviceClass(string deviceClass)
            => SpecificDeviceClasses.Contains(deviceClass);

        private bool IsBinaryDeviceClass(string deviceClass)
            => BinaryDeviceClasses.Contains(deviceClass);
    }
}
