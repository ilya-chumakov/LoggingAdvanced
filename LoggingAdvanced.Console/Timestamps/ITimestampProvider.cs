namespace LoggingAdvanced.Console.Timestamps
{
    public interface ITimestampProvider
    {
        string GetTimestamp(TimestampPolicy policy);
    }
}