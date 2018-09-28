using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace LoggingAdvanced.Console.Test
{
    public class LoggerBuilderTest
    {
        [Fact]
        public void CreateLogger_Default_LoggerIsNotNull()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder.AddConsoleAdvanced());            
            
            var logger = serviceCollection.BuildServiceProvider()
                .GetService(typeof(ILogger<LoggerBuilderTest>));

            Assert.NotNull(logger);
        }
    }
}
