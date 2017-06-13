using System;

namespace Bodrocode.LoggingAdvanced.Console.Times {
    public interface IDateTimeProvider
    {
        DateTime Now();
    }
}