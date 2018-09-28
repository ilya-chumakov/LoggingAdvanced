// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LoggingAdvanced.Console
{
    public static class ConsoleLoggerExtensions
    {
        ///// <summary>Adds a console logger named 'ConsoleAdvanced' to the factory.</summary>
        ///// <param name="builder">The <see cref="T:Microsoft.Extensions.Logging.ILoggingBuilder" /> to use.</param>
        //public static ILoggingBuilder AddConsoleAdvanced(this ILoggingBuilder builder)
        //{
        //    builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, AdvancedConsoleLoggerProvider>());
        //    return builder;
        //}

        ///// <summary>Adds a console logger named 'ConsoleAdvanced' to the factory.</summary>
        ///// <param name="builder">The <see cref="T:Microsoft.Extensions.Logging.ILoggingBuilder" /> to use.</param>
        //public static ILoggingBuilder AddConsoleAdvanced(this ILoggingBuilder builder, 
        //    IConsoleLoggerSettings settings)
        //{
        //    if (settings == null)
        //        throw new ArgumentNullException(nameof (settings));

        //    builder.AddConsoleAdvanced();
        //    builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton(provider => settings));
        //    return builder;
        //}        
        
        /// <summary>
        /// Adds a console logger that is enabled for <see cref="LogLevel"/>.Information or higher.
        /// </summary>
        public static ILoggerFactory AddConsoleAdvanced(this ILoggerFactory factory)
        {
            return factory.AddConsoleAdvanced(ConsoleLoggerSettings.Optimized);
        }

        /// <summary>
        /// Adds a console logger that is enabled for <see cref="LogLevel"/>.Information or higher.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="settings"></param>
        public static ILoggerFactory AddConsoleAdvanced(
            this ILoggerFactory factory,
            IConsoleLoggerSettings settings)
        {
            factory.AddConsoleAdvanced((n, l) => l >= LogLevel.Information, settings);
            return factory;
        }
        
        /// <summary>
        /// Adds a console logger that is enabled for <see cref="LogLevel"/>s of minLevel or higher.
        /// </summary>
        /// <param name="factory">The <see cref="ILoggerFactory"/> to use.</param>
        /// <param name="minLevel">The minimum <see cref="LogLevel"/> to be logged</param>
        public static ILoggerFactory AddConsoleAdvanced(
            this ILoggerFactory factory, 
            LogLevel minLevel)
        {
            factory.AddConsoleAdvanced(minLevel, ConsoleLoggerSettings.Optimized);
            return factory;
        }

        /// <summary>
        /// Adds a console logger that is enabled for <see cref="LogLevel"/>s of minLevel or higher.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="minLevel">The minimum <see cref="LogLevel"/> to be logged</param>
        /// <param name="settings"></param>
        public static ILoggerFactory AddConsoleAdvanced(
            this ILoggerFactory factory,
            LogLevel minLevel,
            IConsoleLoggerSettings settings)
        {
            factory.AddConsoleAdvanced((category, logLevel) => logLevel >= minLevel, settings);
            return factory;
        }

        /// <summary>
        /// Adds a console logger that is enabled as defined by the filter function.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="filter"></param>
        public static ILoggerFactory AddConsoleAdvanced(
            this ILoggerFactory factory,
            Func<string, LogLevel, bool> filter)
        {
            factory.AddConsoleAdvanced(filter, ConsoleLoggerSettings.Optimized);
            return factory;
        }

        /// <summary>
        /// Adds a console logger that is enabled as defined by the filter function.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="filter"></param>
        /// <param name="settings"></param>
        public static ILoggerFactory AddConsoleAdvanced(
            this ILoggerFactory factory,
            Func<string, LogLevel, bool> filter,
            IConsoleLoggerSettings settings)
        {
            factory.AddProvider(new AdvancedConsoleLoggerProvider(filter, settings));

            return factory;
        }

        public static ILoggerFactory AddConsoleAdvanced(
            this ILoggerFactory factory, 
            IConfiguration configuration)
        {
            var settings = new ConfigurationConsoleLoggerSettings(configuration);

            factory.AddProvider(new AdvancedConsoleLoggerProvider(settings));

            return factory;
        }
    }
}