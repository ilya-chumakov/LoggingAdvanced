using System;
using System.Globalization;

namespace LoggingAdvanced.Console.Timestamps
{
    internal class TimestampProvider : ITimestampProvider
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public TimestampProvider(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public string GetTimestamp(TimestampPolicy policy)
        {
            if (policy == null) throw new ArgumentNullException(nameof(policy));

            DateTime now = TimeZoneInfo.ConvertTime(_dateTimeProvider.Now(), GetTimeZone(policy.TimeZone));

            string str = now.ToString(policy.Format, CultureInfo.InvariantCulture);

            return $"[{str}]";
        }

        private TimeZoneInfo GetTimeZone(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            if (id.Equals("local", StringComparison.OrdinalIgnoreCase))
            {
                return TimeZoneInfo.Local;
            }
            return TimeZoneInfo.FindSystemTimeZoneById(id);
        }
    }
}