// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace LoggingAdvanced.Console
{
    public class AdvancedConsoleLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, AdvancedConsoleLogger> _loggers = new ConcurrentDictionary<string, AdvancedConsoleLogger>();

        private readonly Func<string, LogLevel, bool> _filter;
        private IConsoleLoggerSettings _settings;

        public AdvancedConsoleLoggerProvider(Func<string, LogLevel, bool> filter, IConsoleLoggerSettings settings)
        {
            //if (filter == null)
            //{
            //    throw new ArgumentNullException(nameof(filter));
            //}

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            _filter = filter;
            _settings = settings;

            if (_settings.ChangeToken != null)
            {
                _settings.ChangeToken.RegisterChangeCallback(OnConfigurationReload, null);
            }
        }

        public AdvancedConsoleLoggerProvider(IConsoleLoggerSettings settings)
            : this(null, settings)
        {
        }

        private void OnConfigurationReload(object state)
        {
            // The settings object needs to change here, because the old one is probably holding on
            // to an old change token.
            _settings = _settings.Reload();

            foreach (var logger in _loggers.Values)
            {
                logger.Filter = GetFilter(logger.FullName, _settings);
                logger.Settings = _settings;
            }

            // The token will change each time it reloads, so we need to register again.
            if (_settings?.ChangeToken != null)
            {
                _settings.ChangeToken.RegisterChangeCallback(OnConfigurationReload, null);
            }
        }

        public ILogger CreateLogger(string name)
        {
            return _loggers.GetOrAdd(name, CreateLoggerImplementation);
        }

        private AdvancedConsoleLogger CreateLoggerImplementation(string name)
        {
            return new AdvancedConsoleLogger(name, GetFilter(name, _settings), _settings);
        }

        private Func<string, LogLevel, bool> GetFilter(string name, IConsoleLoggerSettings settings)
        {
            if (settings != null)
            {
                foreach (var prefix in GetKeyPrefixes(name))
                {
                    LogLevel level;
                    if (settings.TryGetSwitch(prefix, out level))
                    {
                        return (n, l) => l >= level;
                    }
                }
            }

            if (_filter != null)
            {
                return _filter;
            }

            return (n, l) => false;
        }

        private IEnumerable<string> GetKeyPrefixes(string name)
        {
            while (!string.IsNullOrEmpty(name))
            {
                yield return name;
                var lastIndexOfDot = name.LastIndexOf('.');
                if (lastIndexOfDot == -1)
                {
                    yield return "Default";
                    break;
                }
                name = name.Substring(0, lastIndexOfDot);
            }
        }

        public void Dispose()
        {
        }
    }
}
