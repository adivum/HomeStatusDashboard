namespace Core.Constants
{
    public static class HomeAssistantStateValues
    {
        public const string On = "on";
        public const string Off = "off";
        public const string True = "true";
        public const string False = "false";
        public const string Open = "open";
        public const string Closed = "closed";
        public const string Locked = "locked";
        public const string Unlocked = "unlocked";
        public const string Motion = "motion";
        public const string NoMotion = "no_motion";

        public static readonly string[] BinaryValues = { On, Off, True, False, Open, Closed, Locked, Unlocked, Motion, NoMotion };
    }
}
