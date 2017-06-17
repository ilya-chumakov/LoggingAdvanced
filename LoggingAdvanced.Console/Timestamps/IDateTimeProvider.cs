using System;

namespace LoggingAdvanced.Console.Timestamps {
    public interface IDateTimeProvider
    {
        DateTime Now();
    }
}