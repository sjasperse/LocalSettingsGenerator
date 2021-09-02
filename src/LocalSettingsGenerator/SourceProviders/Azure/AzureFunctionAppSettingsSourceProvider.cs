using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.Rest;

namespace LocalSettingsGenerator.SourceProviders.Azure
{
    public class AzureFunctionAppSettingsSourceProvider : AzureAppServiceSettingsSourceProvider
    {
        public AzureFunctionAppSettingsSourceProvider(AzureClient azureClient)
            : base(azureClient)
        {
        }

        public override string Type => "AzureFunctionAppSettings";
    }
}