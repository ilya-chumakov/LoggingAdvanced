// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Bodrocode.LoggingAdvanced.Console.Test.Legacy.Console;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Bodrocode.LoggingAdvanced.Console.Test
{
    public partial class ConsoleLoggerTest
    {
        private const int WritesPerMsg = 2;
        private readonly string _paddingString;
        private const string _loggerName = "test";

        private static (AdvancedConsoleLogger logger, ConsoleSink sink) SetUp(
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
        public void Log_IncludeLineBreakIsFalse_NoLineBreak()
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

        private string GetMessage(ConsoleSink sink, int messageIndex)
        {
            return GetMessage(sink.Writes.GetRange(messageIndex * WritesPerMsg, WritesPerMsg));
        }

        private string GetMessage<TState>(
            string logLevelString, 
            int eventId, 
            TState state, 
            Exception exception, 
            ConsoleLoggerSettings settings)
        {
            var loglevelStringWithPadding = $"{logLevelString}: ";

            string message = loglevelStringWithPadding
                          + $"{_loggerName}[{eventId}]"
                          + (settings.IncludeLineBreak ? Environment.NewLine : "")
                          + _paddingString
                          + ReplaceMessageNewLinesWithPadding(state?.ToString())
                          + Environment.NewLine
                          + (exception != null
                              ? exception.ToString() + Environment.NewLine
                              : string.Empty);

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
