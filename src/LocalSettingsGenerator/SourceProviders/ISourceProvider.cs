using System.Collections.Generic;
using System.Threading.Tasks;
using LocalSettingsGenerator.Models;

namespace LocalSettingsGenerator.SourceProviders
{
    public interface ISourceProvider
    {
        string Type { get; }

        Task<ISource> CreateAsync(IDictionary<string, object> definition, SourceProviderContext context);
    }
}