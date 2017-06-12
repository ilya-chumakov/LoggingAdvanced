using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Bodrocode.LoggingAdvanced.Console
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
        /// Line break after log name and EventId: FooClass[42] \n foo message
        /// </summary>
        bool IncludeLineBreak { get; }
    }
}
