using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using LocalSettingsGenerator.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LocalSettingsGenerator.SourceProviders;

namespace LocalSettingsGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Contains("--debug")) WaitForDebugger().Wait();

            MainAsync().Wait();
        }

        static async Task MainAsync()
        {
            var localSettingsFilePath = Path.Join(Environment.CurrentDirectory, "local-settings.json");

            if (!File.Exists(localSettingsFilePath))
            {
                Console.Error.WriteLine($"Could not find {localSettingsFilePath}");
                Environment.Exit(1);
            }

            var localSettingsFileContents = File.ReadAllText(localSettingsFilePath);

            var localSettingsModel = JsonSerializer.Deserialize<LocalSettings>(localSettingsFileContents, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            });

            var services = new ServiceCollection();
            services.AddLogging(options => {
                options.AddSimpleConsole();
                options.SetMinimumLevel(LogLevel.Information);
            });
            services.AddHttpClient();
            services.AddSourceProviders();
            services.AddSingleton<AzureClient>();
            services.AddSingleton<FileProcessor>();
            services.AddSingleton< Azure.Core.TokenCredential, Azure.Identity.AzureCliCredential>();
            var serviceProvider = services.BuildServiceProvider();
            
            var sources = await SourceFactory.CreateAllAsync(localSettingsModel.Sources, serviceProvider);  

            var fileProcessor = serviceProvider.GetRequiredService<FileProcessor>();
            foreach (var file in localSettingsModel.Files)
            {
                await fileProcessor.Process(file, sources, Environment.CurrentDirectory);
            }    
        }

        static async Task WaitForDebugger()
        {
            Console.WriteLine($"Waiting for debugger on process {Process.GetCurrentProcess().Id}");
            while (!Debugger.IsAttached)
            {
                await Task.Delay(200);
            }

            Console.WriteLine("Debugger attached.");
        }
    }
}
