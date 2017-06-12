using System;

namespace Bodrocode.LoggingAdvanced.Console
{
    public interface ITimestampProvider
    {
        string GetTimestamp();
    }

    public class TimestampProvider : ITimestampProvider
    {
        public string GetTimestamp() => $"[{DateTime.Now}]";
    }
}