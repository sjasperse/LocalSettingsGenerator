using Microsoft.Extensions.DependencyInjection;

namespace LocalSettingsGenerator.SourceProviders
{
    public static class SourceProviderExtensions
    {
        public static IServiceCollection AddSourceProviders(this IServiceCollection services)
        {
            services.AddSingleton<ISourceProvider, Azure.AzureAppServiceSettingsSourceProvider>();
            services.AddSingleton<ISourceProvider, Azure.AzureFunctionAppSettingsSourceProvider>();
            services.AddSingleton<ISourceProvider, Azure.AzureKeyVaultSecretsSourceProvider>();

            return services;
        }
    }
}