using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace LoggingAdvanced.Console.Test
{
    public class LoggerFactoryTest
    {
        private readonly ILoggerFactory _factory;

        public LoggerFactoryTest()
        {
            var services = new ServiceCollection();

            services.AddLogging();

            var provider = services.BuildServiceProvider();

            _factory = provider.GetService<ILoggerFactory>();
        }

        [Fact]
        public void CreateLogger_Default_LoggerIsNotNull()
        {
            _factory.AddConsoleAdvanced();

            var logger = _factory.CreateLogger<LoggerFactoryTest>();

            Assert.NotNull(logger);
        }

        [Fact]
        public void Log_Default_NoException()
        {
            _factory.AddConsoleAdvanced();

            var logger = _factory.CreateLogger<LoggerFactoryTest>();

            logger.LogCritical("foo");
        }

        [Fact]
        public void CreateLogger_NewSettingsObject_LoggerIsNotNull()
        {
            _factory.AddConsoleAdvanced(new ConsoleLoggerSettings());

            var logger = _factory.CreateLogger<LoggerFactoryTest>();

            Assert.NotNull(logger);
        }

        [Fact]
        public void Log_NewSettingsObject_NoException()
        {
            _factory.AddConsoleAdvanced(new ConsoleLoggerSettings());

            var logger = _factory.CreateLogger<LoggerFactoryTest>();

            logger.LogCritical("foo");
        }
    }
}
