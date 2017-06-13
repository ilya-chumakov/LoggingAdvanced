namespace Bodrocode.LoggingAdvanced.Console.Times
{
    public interface ITimestampProvider
    {
        string GetTimestamp(TimestampPolicy policy);
    }
}