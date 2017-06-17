
using Microsoft.Extensions.Configuration;
using Xunit;

namespace LoggingAdvanced.Console.Test.JsonConfiguration
{
    public class ConfigurationTest
    {
        private static IConfigurationRoot CreateConfiguration(string filename)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"JsonConfiguration\\{filename}", false);
            var configuration = builder.Build();
            return configuration;
        }

        [Fact]
        public void BooleanSetting_ReadFromFile_IsTrue()
        {
            var configuration = CreateConfiguration("appsettings-booleans.json");

            var settings = new ConfigurationConsoleLoggerSettings(
                configuration.GetSection("Logging"));

            Assert.True(settings.IncludeScopes);
            Assert.True(settings.IncludeLineBreak);
            Assert.True(settings.IncludeLogNamespace);
            Assert.True(settings.IncludeTimestamp);
            Assert.True(settings.IncludeZeroEventId);
        }

        [Fact]
        public void Timestamp_ReadFromFile_IsTrue()
        {
            var configuration = CreateConfiguration("appsettings-timestamp.json");

            var settings = new ConfigurationConsoleLoggerSettings(
                configuration.GetSection("Logging"));

            Assert.NotNull(settings.TimestampPolicy);
            Assert.Equal("UTC", settings.TimestampPolicy.TimeZone);
            Assert.Equal("foo", settings.TimestampPolicy.Format);
        }
    }
}