using System;

namespace Bodrocode.LoggingAdvanced.Console.Times
{
    internal class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}