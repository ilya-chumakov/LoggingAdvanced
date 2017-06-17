using System;

namespace LoggingAdvanced.Console.Timestamps
{
    internal class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}