using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.Rest;

namespace LocalSettingsGenerator.SourceProviders.Azure
{
    public class AzureAppServiceSettingsSourceProvider : ISourceProvider
    {
        private readonly AzureClient azureClient;

        public AzureAppServiceSettingsSourceProvider(AzureClient azureClient)
        {
            this.azureClient = azureClient;
        }

        public virtual string Type => "AzureAppServiceSettings";

        public async Task<ISource> CreateAsync(IDictionary<string, object> definition, SourceProviderContext context)
        {
            var name = definition.GetValueOrDefault("name", caseInspecificKey: true)?.ToString();
            var subscriptionId = definition.GetValueOrDefault("subscriptionId", caseInspecificKey: true)?.ToString();
            var resourceGroup = definition.GetValueOrDefault("resourceGroup", caseInspecificKey: true)?.ToString();
            var resource = definition.GetValueOrDefault("resource", caseInspecificKey: true)?.ToString();

            var values = await azureClient.GetAppServiceSettings(subscriptionId, resourceGroup, resource);

            return new DictionarySource(name, values);
        }
    }
}