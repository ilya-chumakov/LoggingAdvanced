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
    }
}
