// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Bodrocode.LoggingAdvanced.Console.Timestamps;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Bodrocode.LoggingAdvanced.Console
{
    public class ConfigurationConsoleLoggerSettings : IConsoleLoggerSettings
    {
        private readonly IConfiguration _configuration;

        public ConfigurationConsoleLoggerSettings(IConfiguration configuration)
        {
            _configuration = configuration;
            ChangeToken = configuration.GetReloadToken();
        }

        public IChangeToken ChangeToken { get; private set; }

        public bool IncludeScopes => ReadBooleanProperty(nameof(IncludeScopes));
        public bool IncludeLineBreak => ReadBooleanProperty(nameof(IncludeLineBreak));
        public bool IncludeTimestamp => ReadBooleanProperty(nameof(IncludeTimestamp));
        public bool IncludeZeroEventId => ReadBooleanProperty(nameof(IncludeZeroEventId));
        public bool IncludeLogNamespace => ReadBooleanProperty(nameof(IncludeLogNamespace));
        public TimestampPolicy TimestampPolicy => ReadProperty<TimestampPolicy>(nameof(TimestampPolicy));

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

        private bool ReadBooleanProperty(string name)
        {
            bool flag;
            var value = _configuration[name];
            if (string.IsNullOrEmpty(value))
                return false;
            if (bool.TryParse(value, out flag))
            {
                return flag;
            }
            var message = $"Configuration value '{value}' for setting '{name}' is not supported.";
            throw new InvalidOperationException(message);
        }

        private TPayload ReadProperty<TPayload>(string name)
            where TPayload : class 
        {
            var value = _configuration.GetSection(name);

            TPayload payload = value.Get<TPayload>();

            return payload;
        }
    }
}