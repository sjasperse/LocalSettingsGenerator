using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace LocalSettingsGenerator.SourceProviders.Azure
{
    public class AzureKeyVaultSecretsSourceProvider : ISourceProvider
    {
        private readonly TokenCredential azCredential;
        private readonly IHttpClientFactory httpClientFactory;

        public AzureKeyVaultSecretsSourceProvider(TokenCredential azCredential, IHttpClientFactory httpClientFactory)
        {
            this.azCredential = azCredential;
            this.httpClientFactory = httpClientFactory;
        }

        public string Type => "AzureKeyVaultSecrets";

        public async Task<ISource> CreateAsync(IDictionary<string, object> definition, SourceProviderContext context)
        {
            var name = definition.GetValueOrDefault("name", caseInspecificKey: true)?.ToString();
            var subscriptionId = definition.GetValueOrDefault("subscriptionId", caseInspecificKey: true)?.ToString();
            var resourceGroup = definition.GetValueOrDefault("resourceGroup", caseInspecificKey: true)?.ToString();
            var resource = definition.GetValueOrDefault("resource", caseInspecificKey: true)?.ToString();

            var kvUri = new Uri($"https://{resource}.vault.azure.net/");

            var secretClient = new SecretClient(kvUri, azCredential, new SecretClientOptions()
            {
                Transport = new HttpClientTransport(httpClientFactory.CreateClient())
            });

            await Task.CompletedTask;
            
            return new CallbackSource(name, async key => {
                var response = await secretClient.GetSecretAsync(key);
                return response.Value.Value;
            });
        }
    }
}