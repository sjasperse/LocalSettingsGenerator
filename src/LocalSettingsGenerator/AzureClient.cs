using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Identity;

namespace LocalSettingsGenerator
{
    public class AzureClient
    {
        private readonly Task<HttpClient> httpClientTask;

        public AzureClient(IHttpClientFactory httpClientFactory, Azure.Core.TokenCredential azCredential)
        {
            httpClientTask = azCredential
                .GetTokenAsync(new Azure.Core.TokenRequestContext(new [] { 
                    "https://management.azure.com/.default",
                }), default)
                .AsTask()
                .ContinueWith(x => x.Result.Token)
                .ContinueWith(tokenTask => {
                    var httpClient = httpClientFactory.CreateClient();
                    httpClient.BaseAddress = new Uri("https://management.azure.com/");
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", tokenTask.Result);

                    return httpClient;
                });
        }

        private async Task<TResult> PostAndReceive<TResult>(string url)
        {
            var httpClient = await httpClientTask;

            var response = await httpClient.PostAsync(url, new StringContent(""));

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<TResult>(content, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });

            return result;
        }

        private async Task<TResult> GetAndReceive<TResult>(string url)
        {
            var httpClient = await httpClientTask;

            var response = await httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<TResult>(content, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });

            return result;
        }

        public async Task<IDictionary<string, string>> GetAppServiceSettings(string subscriptionId, string resourceGroupName, string name)
        {
            var result = await PostAndReceive<AzureResponse>($"/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Web/sites/{name}/config/appsettings/list?api-version=2019-08-01");

            var properties = result.Properties.ToDictionary(x => x.Key, x => x.Value?.ToString());

            return properties;
        }

        private class AzureResponse
        {   
            public IDictionary<string, string> Tags { get; set; }
            public IDictionary<string, object> Properties { get; set; }
        }
    }
}