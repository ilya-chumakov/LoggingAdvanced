using System;
using Bodrocode.LoggingAdvanced.Console.Timestamps;
using Moq;
using Xunit;

namespace Bodrocode.LoggingAdvanced.Console.Test
{
    public class TimestampProviderTest
    {
        private static TimestampProvider CreateIsolatedTimestampProvider(DateTime now)
        {
            var mock = new Mock<IDateTimeProvider>();
            mock.Setup(x => x.Now()).Returns(now);

            return new TimestampProvider(mock.Object);
        }

        [Theory]
        [InlineData(null, "UTC", "[01/02/2000 03:04:05]")]
        [InlineData(null, "Ulaanbaatar Standard Time", "[01/02/2000 11:04:05]")]
        [InlineData("MM/dd/yyyy HH:mm:ss.fff", "UTC", "[01/02/2000 03:04:05.006]")]
        [InlineData("yyyy.MM.dd HH:mm:ss", "UTC", "[2000.01.02 03:04:05]")]
        public void GetTimestamp_InlineData_Test(string format, string timezone, string expected)
        {
            var now = new DateTime(2000, 1, 2, 3, 4, 5, 6, DateTimeKind.Utc);

            var provider = CreateIsolatedTimestampProvider(now);

            var policy = new TimestampPolicy
            {
                Format = format,
                TimeZone = timezone
            };

            string actual = provider.GetTimestamp(policy);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetTimestamp_Local_Test()
        {
            var now = new DateTime(2000, 1, 2, 3, 4, 5, 6, DateTimeKind.Local);

            var provider = CreateIsolatedTimestampProvider(now);

            var policy = new TimestampPolicy
            {
                TimeZone = "Local"
            };

            string actual = provider.GetTimestamp(policy);
            string expected = $"[{now.ToString(policy.Format)}]";

            Assert.Equal(expected, actual);
        }
    }
}