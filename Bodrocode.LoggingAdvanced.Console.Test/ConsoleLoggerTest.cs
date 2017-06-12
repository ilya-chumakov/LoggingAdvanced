// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Bodrocode.LoggingAdvanced.Console.Settings;
using Bodrocode.LoggingAdvanced.Console.Test.Legacy.Console;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Bodrocode.LoggingAdvanced.Console.Test
{
    public partial class ConsoleLoggerTest
    {
        private int _writesPerMsgDefault = 2;
        private string _loggerName = "test";
        private string _paddingString;

        private (AdvancedConsoleLogger logger, ConsoleSink sink) SetUp(
            Func<string, LogLevel, bool> filter, 
            ConsoleLoggerSettings settings)
        {
            // Arrange
            var sink = new ConsoleSink();
            var console = new TestConsole(sink);
            
            var logger = new AdvancedConsoleLogger(_loggerName, filter, settings);
            logger.Console = console;

            return (logger, sink);
        }

        public ConsoleLoggerTest()
        {
            var loglevelStringWithPadding = "INFO: ";
            _paddingString = new string(' ', loglevelStringWithPadding.Length);
        }

        [Fact]
        public void IncludeLineBreak_IsFalse_NoLineBreak()
        {
            // Arrange
            var settings = ConsoleLoggerSettings.Default;
            settings.IncludeLineBreak = false;
            var tuple = SetUp(null, settings);
            var logger = tuple.logger;
            var sink = tuple.sink;
            var exception = new InvalidOperationException("Invalid value");

            // Act
            logger.LogCritical(eventId: 0, exception: null, message: null);
            logger.LogCritical(eventId: 0, message: null);
            logger.LogCritical(eventId: 0, message: "foo");
            logger.LogCritical(eventId: 0, message: null, exception: exception);

            // Assert
            Assert.Equal(8, sink.Writes.Count);

            Assert.Equal(GetMessage("crit", 0, "[null]", null, settings), GetMessage(sink, 0));
            Assert.Equal(GetMessage("crit", 0, "[null]", null, settings), GetMessage(sink, 1));
            Assert.Equal(GetMessage("crit", 0, "foo", null, settings), GetMessage(sink, 2));
            Assert.Equal(GetMessage("crit", 0, "[null]", exception, settings), GetMessage(sink, 3));
        }

        [Fact]
        public void IncludeTimestamp_IsTrue_HasTimestamp()
        {
            // Arrange
            var settings = ConsoleLoggerSettings.Default;
            settings.IncludeTimestamp = true;
            var tuple = SetUp(null, settings);
            var logger = tuple.logger;
            var sink = tuple.sink;

            var time = new Mock<ITimestampProvider>();
            time.Setup(x => x.GetTimestamp()).Returns("[dt]");
            logger.TimestampProvider = time.Object;

            // Act
            logger.LogCritical(eventId: 0, message: "foo");

            Assert.Equal(GetMessage("[dt] crit", 0, "foo", null, settings), GetMessage(sink, 0, 3));
        }

        [Fact]
        public void IncludeZeroEventId_IsFalse_NoEventId()
        {
            // Arrange
            var settings = ConsoleLoggerSettings.Default;
            settings.IncludeZeroEventId = false;
            var tuple = SetUp(null, settings);
            var logger = tuple.logger;
            var sink = tuple.sink;

            // Act
            logger.LogCritical(eventId: 0, message: "foo");

            Assert.Equal(GetMessage("crit", 0, "foo", null, settings), GetMessage(sink, 0, 2));
        }

        [Fact]
        public void IncludeLogNamespace_IsFalse_NoNamespace()
        {
            _loggerName = "Root.Sub.BarLogger";
            // Arrange
            var settings = ConsoleLoggerSettings.Default;
            settings.IncludeLogNamespace = false;
            var tuple = SetUp(null, settings);
            var logger = tuple.logger;
            var sink = tuple.sink;

            // Act
            logger.LogCritical(eventId: 0, message: "foo");

            Assert.Equal(GetMessage("crit", 0, "foo", null, settings, "BarLogger"), 
                GetMessage(sink, 0, 2));
        }


        private string GetMessage(ConsoleSink sink, int messageIndex, int? writesCount = null)
        {
            int count = writesCount ?? _writesPerMsgDefault;

            return GetMessage(sink.Writes.GetRange(messageIndex * count, count));
        }

        private string GetMessage(
            string logLevelString, 
            int eventId, 
            string text, 
            Exception exception, 
            ConsoleLoggerSettings settings,
            string actualLoggerName = null)
        {
            var loglevelStringWithPadding = $"{logLevelString}: ";

            string eventIdStr = !settings.IncludeZeroEventId && eventId == 0 
                ? ""
                : $"[{eventId}]";

            string exStr = exception != null
                ? exception + Environment.NewLine
                : string.Empty;

            string loggerName = settings.IncludeLogNamespace 
                ? _loggerName 
                : actualLoggerName;

            string message = loglevelStringWithPadding
                          + loggerName
                          + eventIdStr
                          + (settings.IncludeLineBreak ? Environment.NewLine : "")
                          + _paddingString
                          + ReplaceMessageNewLinesWithPadding(text?.ToString())
                          + Environment.NewLine
                          + exStr;

            return message;
        }

        private string ReplaceMessageNewLinesWithPadding(string message)
        {
            return message.Replace(Environment.NewLine, Environment.NewLine + _paddingString);
        }

        private string GetMessage(List<ConsoleContext> contexts)
        {
            return string.Join("", contexts.Select(c => c.Message));
        }

        private string CreateHeader(int eventId = 0)
        {
            return $": {_loggerName}[{eventId}]{Environment.NewLine}";
        }
    }
}
