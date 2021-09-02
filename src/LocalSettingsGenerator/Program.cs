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
using Microsoft.Extensions.Configuration;

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
            var output = new Output();

            var config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local-settings.json")
                .Build();

            var localSettingsModel = config.Get<LocalSettings>();

            var services = new ServiceCollection();
            services.AddLogging(options => {
                options.AddSimpleConsole();

                options.AddConfiguration(config.GetSection("logging"));
                    
            });
            services.Configure<LoggerFilterOptions>(options => {
                if (options.Rules.Any()) return;

                options.AddFilter(logLevel => logLevel >= LogLevel.Warning);
            });
            services.AddHttpClient();
            services.AddSourceProviders();
            services.AddSingleton<AzureClient>();
            services.AddSingleton<FileProcessor>();
            services.AddSingleton<Azure.Core.TokenCredential>(new Azure.Identity.DefaultAzureCredential(false));
            var serviceProvider = services.BuildServiceProvider();

            output.WriteLine("Initializing sources...");
            output.Indent();
            var sources = await SourceFactory.CreateAllAsync(localSettingsModel.Sources, serviceProvider, output);  
            output.Deindent();

            output.WriteLine("Generating Files...");
            using (output.Indent())
            {
                var fileProcessor = serviceProvider.GetRequiredService<FileProcessor>();
                foreach (var file in localSettingsModel.Files)
                {
                    output.WriteLine($"{file.Template}  -->  {file.Target}");
                    using (output.Indent())
                    {
                        await fileProcessor.Process(file, sources, Environment.CurrentDirectory, output);
                    }
                }    
            }
            output.WriteLine("Finished");
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
