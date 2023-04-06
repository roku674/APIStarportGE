
//Created by Alexander Fields 
using Optimization.Objects;
using System.IO;

namespace APILoggingTests
{
    internal class SettingsTests
    {
        static string configJson = Directory.GetCurrentDirectory() + "/config.json";

        public void CreateSettings()
        {
            string configContents = File.ReadAllText(configJson);
            Settings.BuildAndSetConfig(configJson);
        }
    }
}
