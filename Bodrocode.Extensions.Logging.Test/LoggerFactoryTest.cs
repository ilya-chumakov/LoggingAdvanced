using System;
using Bodrocode.Extensions.Logging.Console;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Bodrocode.Extensions.Logging.Test
{
    public class LoggerFactoryTest
    {
        [Fact]
        public void CreateLogger_WhenCalled_LoggerIsNotNull()
        {
            var services = new ServiceCollection();

            services.AddLogging();

            var provider = services.BuildServiceProvider();

            var factory = provider.GetService<ILoggerFactory>();

            factory.AddConsoleAdvanced();

            var logger = factory.CreateLogger<LoggerFactoryTest>();

            Assert.NotNull(logger);
        }
    }
}
