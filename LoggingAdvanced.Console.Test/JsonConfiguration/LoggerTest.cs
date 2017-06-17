using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace LoggingAdvanced.Console.Test.JsonConfiguration
{
    public class LoggerTest
    {
        [Fact]
        public void Timestamp_Log_NoException()
        {
            var logger = CreateLogger("appsettings-timestamp.json");

            logger.LogCritical("foo");
        }

        [Fact]
        public void Booleans_Log_NoException()
        {
            var logger = CreateLogger("appsettings-booleans.json");

            logger.LogCritical("foo");
        }

        [Fact]
        public void Switches_Log_NoException()
        {
            var logger = CreateLogger("appsettings-switches.json");

            logger.LogCritical("foo");
        }

        private static ILogger<ConfigurationTest> CreateLogger(string filename)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"JsonConfiguration\\{filename}", optional: false);
            var configuration = builder.Build();

            var services = new ServiceCollection();
            services.AddLogging();
            var provider = services.BuildServiceProvider();

            var factory = provider.GetService<ILoggerFactory>();

            factory
                .AddDebug()
                .AddConsoleAdvanced(configuration.GetSection("Logging"));

            var logger = factory.CreateLogger<ConfigurationTest>();

            return logger;
        }
    }
}