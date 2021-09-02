using System;
using System.Collections.Generic;

namespace LocalSettingsGenerator.SourceProviders
{
    public class SourceProviderContext
    {
        public SourceProviderContext(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public Dictionary<string, object> Items { get; } = new Dictionary<string, object>();

        public IServiceProvider ServiceProvider { get; }
    }
}