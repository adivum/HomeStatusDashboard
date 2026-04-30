using Core.Constants;

namespace Core.Utilities
{
    public static class ServiceActionExtensions
    {
        public static string ToServicePath(this ServiceDomain domain, ServiceAction action)
        {
            var domainName = domain.ToString().ToLowerInvariant();
            var actionName = ConvertActionToSnakeCase(action);
            return $"{domainName}/{actionName}";
        }

        private static string ConvertActionToSnakeCase(ServiceAction action)
        {
            return action switch
            {
                ServiceAction.TurnOn => "turn_on",
                ServiceAction.TurnOff => "turn_off",
                ServiceAction.Toggle => "toggle",
                ServiceAction.Open => "open",
                ServiceAction.Close => "close",
                ServiceAction.Stop => "stop",
                _ => throw new ArgumentOutOfRangeException(nameof(action), $"Unknown service action: {action}")
            };
        }
    }
}
