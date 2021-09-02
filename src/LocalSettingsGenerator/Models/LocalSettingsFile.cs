using System.Collections.Generic;

namespace LocalSettingsGenerator.Models
{
    public class LocalSettingsFile
    {
        public string Name { get; set; }
        public string Template { get; set; }
        public string Target { get; set; }
    }
}