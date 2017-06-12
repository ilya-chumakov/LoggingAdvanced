// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Bodrocode.LoggingAdvanced.Console.Test.Fakes
{
    internal class MockConsoleLoggerSettings : IConsoleLoggerSettings
    {
        public CancellationTokenSource Cancel { get; set; }

        public IChangeToken ChangeToken => new CancellationChangeToken(Cancel.Token);

        public IDictionary<string, LogLevel> Switches { get; } = new Dictionary<string, LogLevel>();

        public bool IncludeScopes { get; set; }
        public bool IncludeLineBreak { get; }
        public bool IncludeTimestamp { get; }
        public bool IncludeZeroEventId { get; }
        public bool IncludeLogNamespace { get; }

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
