using System;
using Bodrocode.LoggingAdvanced.Console.Times;
using Moq;
using Xunit;

namespace Bodrocode.LoggingAdvanced.Console.Test
{
    public class TimestampProviderTest
    {
        [Theory]
        [InlineData(null, "UTC", "[01/02/2000 00:04:05]")]
        [InlineData(null, "Local", "[01/02/2000 03:04:05]")]
        [InlineData(null, "Ulaanbaatar Standard Time", "[01/02/2000 08:04:05]")]
        [InlineData("MM/dd/yyyy HH:mm:ss.fff", "Local", "[01/02/2000 03:04:05.006]")]
        [InlineData("yyyy.MM.dd HH:mm:ss", "Local", "[2000.01.02 03:04:05]")]
        public void GetTimestamp_Test(string format, string timezone, string expected)
        {
            var now = new DateTime(2000, 1, 2, 3, 4, 5, 6);

            var mock = new Mock<IDateTimeProvider>();
            mock.Setup(x => x.Now()).Returns(now);

            var provider = new TimestampProvider(mock.Object);

            var policy = new TimestampPolicy
            {
                Format = format,
                TimeZone = timezone
            };

            string datetime = provider.GetTimestamp2(policy);

            Assert.Equal(expected, datetime);
        }
    }
}