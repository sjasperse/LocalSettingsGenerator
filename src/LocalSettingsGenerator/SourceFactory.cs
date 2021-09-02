using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LocalSettingsGenerator.SourceProviders;
using Microsoft.Extensions.DependencyInjection;

namespace LocalSettingsGenerator
{
    public static class SourceFactory
    {
        public static async Task<IDictionary<string, ISource>> CreateAllAsync(IEnumerable<IDictionary<string, object>> definitions, IServiceProvider serviceProvider)
        {
            var context = new SourceProviderContext(serviceProvider);
            var sourceProviders = serviceProvider.GetServices<ISourceProvider>()
                .ToDictionary(x => x.Type);

            var sources = new List<ISource>();
            foreach (var definition in definitions)
            {
                var source = await CreateAsync(definition, sourceProviders, context);

                sources.Add(source);
            }

            return sources.ToDictionary(x => x.Name);
        }

        static async Task<ISource> CreateAsync(IDictionary<string, object> definition, IDictionary<string, ISourceProvider> sourceProviders, SourceProviderContext context)
        {
            var providerNames = sourceProviders.Values.Select(x => x.Type).OrderBy(x => x);

            var name = definition.GetValueOrDefault("name", caseInspecificKey: true)?.ToString();
            if (string.IsNullOrEmpty(name)) throw new ApplicationException("All sources must all have a name");

            var type = definition.GetValueOrDefault("type", caseInspecificKey: true)?.ToString();
            if (string.IsNullOrEmpty(type)) throw new ApplicationException($"Source '{name}' did not have a type specified. Valid types: {string.Join(", ", providerNames)}");

            var provider = sourceProviders.GetValueOrDefault(type, caseInspecificKey: true);
            if (provider == null) throw new ApplicationException($"Source '{name}' had an invalid type. Valid types: {string.Join(", ", providerNames)}");

            return await provider.CreateAsync(definition, context);
        }
    }
}