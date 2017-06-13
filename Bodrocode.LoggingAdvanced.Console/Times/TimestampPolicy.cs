namespace Bodrocode.LoggingAdvanced.Console.Times
{
    public class TimestampPolicy
    {
        public TimestampPolicy()
        {
            Format = null;
            TimeZone = "Local";
        }

        /// <summary>
        /// Custom output format https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Support for "Local" value and timezones from https://msdn.microsoft.com/en-us/library/gg154758.aspx
        /// </summary>
        public string TimeZone { get; set; }
    }
}