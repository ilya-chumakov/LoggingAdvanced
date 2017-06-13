using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Bodrocode.LoggingAdvanced.Console.Test
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
        public void CreateLogger_WithParams_LoggerIsNotNull()
        {
            _factory.AddConsoleAdvanced(new ConsoleLoggerSettings());

            var logger = _factory.CreateLogger<LoggerFactoryTest>();

            Assert.NotNull(logger);
        }

        [Fact]
        public void Log_WhenCalled_NoException()
        {
            _factory.AddConsoleAdvanced(new ConsoleLoggerSettings());

            var logger = _factory.CreateLogger<LoggerFactoryTest>();

            logger.LogCritical("foo");
        }
    }
}
