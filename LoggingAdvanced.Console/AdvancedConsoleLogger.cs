// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;
using System.Text;
using LoggingAdvanced.Console.Internal;
using LoggingAdvanced.Console.Timestamps;
using Microsoft.Extensions.Logging;

namespace LoggingAdvanced.Console
{
    public class AdvancedConsoleLogger : ILogger
    {
        // Writing to console is not an atomic operation in the current implementation and since multiple logger
        // instances are created with a different name. Also since Console is global, using a static lock is fine.
        private static readonly object _lock = new object();

        private static readonly string _loglevelPadding = ": ";
        private static readonly string _messagePadding;
        private static readonly string _newLineWithMessagePadding;

        [ThreadStatic] private static StringBuilder _logBuilder;

        // ConsoleColor does not have a value to specify the 'Default' color
        private readonly ConsoleColor? DefaultConsoleColor = null;

        private IConsole _console;
        private Func<string, LogLevel, bool> _filter;
        private ITimestampProvider _timestampProvider;

        static AdvancedConsoleLogger()
        {
            var logLevelString = GetLogLevelString(LogLevel.Information);
            _messagePadding = new string(' ', logLevelString.Length + _loglevelPadding.Length);
            _newLineWithMessagePadding = Environment.NewLine + _messagePadding;
        }

        public AdvancedConsoleLogger(string name, Func<string, LogLevel, bool> filter,
            IReadonlyLoggerSettings settings)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            FullName = name;
            //class name without namespace
            ShortName = StripNamespace(name);

            Filter = filter ?? ((category, logLevel) => true);
            Settings = settings;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Console = new WindowsLogConsole();
            else
                Console = new AnsiLogConsole(new AnsiSystemConsole());

            TimestampProvider = new TimestampProvider(new DateTimeProvider());
        }

        private string StripNamespace(string name)
        {
            int lastIndexOf = name.LastIndexOf(".", StringComparison.Ordinal);

            return lastIndexOf > 0 ? name.Substring(lastIndexOf + 1) : name;
        }

        public IConsole Console
        {
            get => _console;
            set => _console = value ?? throw new ArgumentNullException(nameof(value));
        }

        public Func<string, LogLevel, bool> Filter
        {
            get => _filter;
            set => _filter = value ?? throw new ArgumentNullException(nameof(value));
        }

        public ITimestampProvider TimestampProvider
        {
            get => _timestampProvider;
            set => _timestampProvider = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string FullName { get; }
        public string ShortName { get; }

        public IReadonlyLoggerSettings Settings { get; set; }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));

            var message = formatter(state, exception);

            if (!string.IsNullOrEmpty(message) || exception != null)
            {
                string logName = Settings.IncludeLogNamespace ? FullName : ShortName;

                WriteMessage(logLevel, logName, eventId.Id, message, exception);
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return Filter(FullName, logLevel);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            return ConsoleLogScope.Push(FullName, state);
        }

        public virtual void WriteMessage(LogLevel logLevel, string logName, int eventId, string message,
            Exception exception)
        {
            var logBuilder = _logBuilder;
            _logBuilder = null;

            if (logBuilder == null)
                logBuilder = new StringBuilder();

            var logLevelColors = default(ConsoleColors);
            var logLevelString = string.Empty;

            if (!string.IsNullOrEmpty(message))
            {
                logLevelColors = GetLogLevelConsoleColors(logLevel);
                logLevelString = GetLogLevelString(logLevel);
                // category and event id
                logBuilder.Append(_loglevelPadding);
                logBuilder.Append(logName);

                if (!Settings.IncludeZeroEventId && eventId == 0)
                {
                }
                else
                {
                    logBuilder.Append("[");
                    logBuilder.Append(eventId);
                    logBuilder.Append("]");
                }

                if (Settings.IncludeLineBreak) logBuilder.AppendLine();

                // scope information
                if (Settings.IncludeScopes)
                    GetScopeInformation(logBuilder);

                // message
                logBuilder.Append(_messagePadding);
                var len = logBuilder.Length;
                logBuilder.AppendLine(message);
                logBuilder.Replace(Environment.NewLine, _newLineWithMessagePadding, len, message.Length);
            }

            // Example:
            // System.InvalidOperationException
            //    at Namespace.Class.Function() in File:line X
            if (exception != null)
                logBuilder.AppendLine(exception.ToString());

            if (logBuilder.Length > 0)
            {
                var logMessage = logBuilder.ToString();
                lock (_lock)
                {
                    if (Settings.IncludeTimestamp)
                    {
                        string time = _timestampProvider.GetTimestamp(Settings.TimestampPolicy);

                        Console.Write(time + " ", DefaultConsoleColor, DefaultConsoleColor);
                    }

                    if (!string.IsNullOrEmpty(logLevelString))
                    {
                        Console.Write(
                            logLevelString,
                            logLevelColors.Background,
                            logLevelColors.Foreground);
                    }

                    // use default colors from here on
                    Console.Write(logMessage, DefaultConsoleColor, DefaultConsoleColor);

                    // In case of AnsiLogConsole, the messages are not yet written to the console,
                    // this would flush them instead.
                    Console.Flush();
                }
            }

            logBuilder.Clear();
            if (logBuilder.Capacity > 1024)
                logBuilder.Capacity = 1024;
            _logBuilder = logBuilder;
        }

        private static string GetLogLevelString(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return "trce";
                case LogLevel.Debug:
                    return "dbug";
                case LogLevel.Information:
                    return "info";
                case LogLevel.Warning:
                    return "warn";
                case LogLevel.Error:
                    return "fail";
                case LogLevel.Critical:
                    return "crit";
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }

        private ConsoleColors GetLogLevelConsoleColors(LogLevel logLevel)
        {
            // We must explicitly set the background color if we are setting the foreground color,
            // since just setting one can look bad on the users console.
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return new ConsoleColors(ConsoleColor.White, ConsoleColor.Red);
                case LogLevel.Error:
                    return new ConsoleColors(ConsoleColor.Black, ConsoleColor.Red);
                case LogLevel.Warning:
                    return new ConsoleColors(ConsoleColor.Yellow, ConsoleColor.Black);
                case LogLevel.Information:
                    return new ConsoleColors(ConsoleColor.DarkGreen, ConsoleColor.Black);
                case LogLevel.Debug:
                    return new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black);
                case LogLevel.Trace:
                    return new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black);
                default:
                    return new ConsoleColors(DefaultConsoleColor, DefaultConsoleColor);
            }
        }

        private void GetScopeInformation(StringBuilder builder)
        {
            var current = ConsoleLogScope.Current;
            string scopeLog = string.Empty;
            var length = builder.Length;

            while (current != null)
            {
                if (length == builder.Length)
                    scopeLog = $"=> {current}";
                else
                    scopeLog = $"=> {current} ";

                builder.Insert(length, scopeLog);
                current = current.Parent;
            }
            if (builder.Length > length)
            {
                builder.Insert(length, _messagePadding);
                builder.AppendLine();
            }
        }

        private struct ConsoleColors
        {
            public ConsoleColors(ConsoleColor? foreground, ConsoleColor? background)
            {
                Foreground = foreground;
                Background = background;
            }

            public ConsoleColor? Foreground { get; }

            public ConsoleColor? Background { get; }
        }

        private class AnsiSystemConsole : IAnsiSystemConsole
        {
            public void Write(string message)
            {
                System.Console.Write(message);
            }

            public void WriteLine(string message)
            {
                System.Console.WriteLine(message);
            }
        }
    }
}