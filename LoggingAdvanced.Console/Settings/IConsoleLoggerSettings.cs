// ReSharper disable CheckNamespace
using LoggingAdvanced.Console.Timestamps;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace LoggingAdvanced.Console
{
    public interface IConsoleLoggerSettings : IReadonlyLoggerSettings
    {
        IChangeToken ChangeToken { get; }

        bool TryGetSwitch(string name, out LogLevel level);

        IConsoleLoggerSettings Reload();
    }

    public interface IReadonlyLoggerSettings
    {
        bool IncludeScopes { get; }
        
        /// <summary>
        /// Line break after log name and EventId. For example:
        ///     err: FooClass[42] \n foo message
        /// </summary>
        bool IncludeLineBreak { get; }

        /// <summary>
        /// Datetime before message level. For example:
        ///     [06/09/2017 14:00:45] err: FooClass[42] \n foo message
        /// </summary>
        bool IncludeTimestamp { get; }

        /// <summary>
        /// Should EventId=0 be included or not. For example:
        ///     FooClass[0] -> FooClass
        /// </summary>
        bool IncludeZeroEventId { get; }


        /// <summary>
        /// Should log source namespace be included or not. For example:
        ///     Root.Sub.FooClass -> FooClass
        /// </summary>
        bool IncludeLogNamespace { get; }

        TimestampPolicy TimestampPolicy { get; }
    }
}
