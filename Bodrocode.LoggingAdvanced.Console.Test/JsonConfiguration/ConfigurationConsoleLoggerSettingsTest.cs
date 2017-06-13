using Microsoft.Extensions.Configuration;
using Xunit;

namespace Bodrocode.LoggingAdvanced.Console.Test.JsonConfiguration
{
    public class ConfigurationConsoleLoggerSettingsTest
    {
        [Fact]
        public void BooleanSetting_ReadFromFile_IsTrue()
        {
            var builder = new ConfigurationBuilder()
                    .AddJsonFile("JsonConfiguration\\appsettings-example.json", optional: false);
            var configuration = builder.Build();

            var settings = new ConfigurationConsoleLoggerSettings(
                configuration.GetSection("Logging"));

            Assert.True(settings.IncludeScopes);
            Assert.True(settings.IncludeLineBreak);
            Assert.True(settings.IncludeLogNamespace);
            Assert.True(settings.IncludeTimestamp);
            Assert.True(settings.IncludeZeroEventId);
        }
    }
}