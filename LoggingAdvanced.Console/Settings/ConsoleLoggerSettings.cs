// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// ReSharper disable CheckNamespace
using System;
using System.Collections.Generic;
using LoggingAdvanced.Console.Timestamps;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace LoggingAdvanced.Console
{
    public class ConsoleLoggerSettings : IConsoleLoggerSettings
    {
        private TimestampPolicy _timestampPolicy;

        /// <summary>
        ///     Microsoft default implementation with poor message format.
        /// </summary>
        public static ConsoleLoggerSettings Default => new ConsoleLoggerSettings
        {
            IncludeScopes = false,
            IncludeLineBreak = true,
            IncludeTimestamp = false,
            IncludeZeroEventId = true,
            IncludeLogNamespace = true
        };

        /// <summary>
        ///     Improved formatting settings.
        /// </summary>
        public static ConsoleLoggerSettings Optimized => new ConsoleLoggerSettings
        {
            IncludeScopes = false,
            IncludeLineBreak = false,
            IncludeTimestamp = true,
            IncludeZeroEventId = false,
            IncludeLogNamespace = false,
            TimestampPolicy = new TimestampPolicy()
        };

        public IDictionary<string, LogLevel> Switches { get; set; } = new Dictionary<string, LogLevel>();
        public IChangeToken ChangeToken { get; set; }

        /// <inheritdoc cref="IReadonlyLoggerSettings" />
        public bool IncludeLineBreak { get; set; }
        /// <inheritdoc cref="IReadonlyLoggerSettings" />
        public bool IncludeScopes { get; set; }
        /// <inheritdoc cref="IReadonlyLoggerSettings" />
        public bool IncludeTimestamp { get; set; }
        /// <inheritdoc cref="IReadonlyLoggerSettings" />
        public bool IncludeZeroEventId { get; set; }
        /// <inheritdoc cref="IReadonlyLoggerSettings" />
        public bool IncludeLogNamespace { get; set; }

        public TimestampPolicy TimestampPolicy
        {
            get => _timestampPolicy;
            set => _timestampPolicy = value ?? throw new ArgumentNullException(nameof(TimestampPolicy));
        }

        public IConsoleLoggerSettings Reload()
        {
            return this;
        }

        public bool TryGetSwitch(string name, out LogLevel level)
        {
            return Switches.TryGetValue(name, out level);
        }
    }
}