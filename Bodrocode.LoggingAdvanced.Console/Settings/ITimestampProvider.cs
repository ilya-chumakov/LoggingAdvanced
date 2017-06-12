using System;

namespace Bodrocode.LoggingAdvanced.Console.Settings
{
    public interface ITimestampProvider
    {
        string GetTimestamp();
    }

    internal class TimestampProvider : ITimestampProvider
    {
        public string GetTimestamp() => $"[{DateTime.Now}]";
    }
}