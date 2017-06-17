// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// ReSharper disable CheckNamespace
using System;
using LoggingAdvanced.Console.Timestamps;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace LoggingAdvanced.Console
{
    public class ConfigurationConsoleLoggerSettings : IConsoleLoggerSettings
    {
        private readonly IConfiguration _configuration;
        private readonly ConsoleLoggerSettings _default;

        public ConfigurationConsoleLoggerSettings(IConfiguration configuration)
        {
            _configuration = configuration;
            ChangeToken = configuration.GetReloadToken();
            _default = ConsoleLoggerSettings.Optimized;
        }

        public IChangeToken ChangeToken { get; private set; }

        public bool IncludeScopes => ReadBooleanProperty(nameof(IncludeScopes), _default.IncludeScopes);
        public bool IncludeLineBreak => ReadBooleanProperty(nameof(IncludeLineBreak), _default.IncludeLineBreak);
        public bool IncludeTimestamp => ReadBooleanProperty(nameof(IncludeTimestamp), _default.IncludeTimestamp);
        public bool IncludeZeroEventId => ReadBooleanProperty(nameof(IncludeZeroEventId), _default.IncludeZeroEventId);
        public bool IncludeLogNamespace => ReadBooleanProperty(nameof(IncludeLogNamespace), _default.IncludeLogNamespace);
        public TimestampPolicy TimestampPolicy => ReadProperty<TimestampPolicy>(nameof(TimestampPolicy), _default.TimestampPolicy);

        public IConsoleLoggerSettings Reload()
        {
            ChangeToken = null;
            return new ConfigurationConsoleLoggerSettings(_configuration);
        }

        public bool TryGetSwitch(string name, out LogLevel level)
        {
            var switches = _configuration.GetSection("LogLevel");
            if (switches == null)
            {
                level = LogLevel.None;
                return false;
            }

            var value = switches[name];
            if (string.IsNullOrEmpty(value))
            {
                level = LogLevel.None;
                return false;
            }
            if (Enum.TryParse(value, out level))
                return true;
            var message = $"Configuration value '{value}' for category '{name}' is not supported.";
            throw new InvalidOperationException(message);
        }

        private bool ReadBooleanProperty(string name, bool defaultValue)
        {
            bool flag;
            var value = _configuration[name];

            if (string.IsNullOrEmpty(value))
                return defaultValue;

            if (bool.TryParse(value, out flag))
            {
                return flag;
            }
            var message = $"Configuration value '{value}' for setting '{name}' is not supported.";
            throw new InvalidOperationException(message);
        }

        private TPayload ReadProperty<TPayload>(string name, TPayload defaultPayload)
            where TPayload : class 
        {
            var value = _configuration.GetSection(name);

            TPayload payload = value.Get<TPayload>();

            return payload ?? defaultPayload;
        }
    }
}