using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LocalSettingsGenerator.Models;
using Microsoft.Extensions.Logging;

namespace LocalSettingsGenerator
{
    public class FileProcessor
    {
        private static readonly Regex replacePattern = new Regex(@"{{(?<Source>\w+):(?<Key>\w+)}}");
        private readonly ILogger logger;

        public FileProcessor(ILogger<FileProcessor> logger)
        {
            this.logger = logger;
        }

        public async Task Process(LocalSettingsFile fileDef, IDictionary<string, ISource> sources, string basePath)
        {
            var fileText = await File.ReadAllTextAsync(Path.Join(basePath, fileDef.Template));

            var match = (Match)null;
            while (HasMatch(fileText, replacePattern, out match))
            {
                var matchText = match.Value;
                var matchStart = match.Index;
                var matchEnd = matchStart + match.Length;

                var sourceName = match.Groups["Source"].Value;
                var key = match.Groups["Key"].Value;

                var source = sources.GetValueOrDefault(sourceName);
                if (source == null) throw new System.Exception($"Source '{sourceName}' in file '{fileDef.Template}' not found");

                var value = await source.GetValueAsync(key);

                logger.LogInformation("Replacing on line {line} '{old}' --> '{new}'", GetLineNumber(match.Index, fileText), matchText, value);

                fileText = ReplaceBetweenIndexes(fileText, matchStart, matchEnd, value);
            }
            
            await File.WriteAllTextAsync(Path.Join(basePath, fileDef.Target), fileText);
        }

        private bool HasMatch(string text, Regex pattern, out Match match)
        {
            match = pattern.Match(text);

            return match.Success;
        }

        private int GetLineNumber(int index, string text)
        {
            var substring = text.Substring(0, index);
            var lines = substring.Split('\n');

            return lines.Length;
        }

        private string ReplaceBetweenIndexes(string text, int startIndex, int endIndex, string newText)
        {
            var before = text.Substring(0, startIndex);
            var after = text.Substring(endIndex);

            return before + newText + after;
        }
    }
}