using System;

namespace Bodrocode.LoggingAdvanced.Console.Timestamps {
    public interface IDateTimeProvider
    {
        DateTime Now();
    }
}