using System;

namespace Bodrocode.LoggingAdvanced.Console.Timestamps
{
    internal class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}