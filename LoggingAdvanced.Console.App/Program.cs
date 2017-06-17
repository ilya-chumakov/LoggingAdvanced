using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LoggingAdvanced.Console.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddLogging();

            var provider = services.BuildServiceProvider();

            var factory = provider.GetService<ILoggerFactory>();

            factory.AddConsoleAdvanced();

            var logger = factory.CreateLogger<Program>();

            while (true)
            {
                logger.LogCritical("foo");

                System.Console.ReadLine();
            }
        }
    }
}