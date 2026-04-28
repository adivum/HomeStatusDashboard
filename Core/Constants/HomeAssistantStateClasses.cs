namespace Core.Constants
{
    public static class HomeAssistantStateClasses
    {
        public const string Measurement = "measurement";
        public const string Total = "total";
        public const string TotalIncreasing = "total_increasing";

        public static readonly string[] NumericStateClasses = { Measurement, Total, TotalIncreasing };
    }
}
