using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalSettingsGenerator
{
    public interface ISource
    {
        string Name { get; }

        Task<string> GetValueAsync(string name);
    }

    public class DictionarySource : ISource
    {
        private readonly IDictionary<string, string> values;

        public DictionarySource(string name, IDictionary<string, string> values)
        {
            Name = name;
            this.values = values;
        }

        public string Name { get; }

        public Task<string> GetValueAsync(string name)
        {
            return Task.FromResult(values.GetValueOrDefault(name));
        }
    }

    public class CallbackSource : ISource
    {
        private readonly Func<string, Task<string>> getValueAsyncCallback;

        public CallbackSource(string name, Func<string, Task<string>> getValueAsyncCallback)
        {
            this.Name = name;
            this.getValueAsyncCallback = getValueAsyncCallback;
        }

        public string Name { get; }

        public async Task<string> GetValueAsync(string name)
        {
            return await getValueAsyncCallback(name);
        }
    }
}