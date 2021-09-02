using System.Collections.Generic;

namespace LocalSettingsGenerator.Models
{
    public class LocalSettings
    {
        public IEnumerable<IDictionary<string, object>> Sources { get; set; }
        public IEnumerable<LocalSettingsFile> Files { get; set; }
    }
}