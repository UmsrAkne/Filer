using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Filer.Models.Settings
{
    public class ApplicationSetting
    {
        public static string AppSettingFileName => "applicationSettings.json";

        public List<Favorite> Favorites { get; set; } = new List<Favorite>();

        public List<string> LastVisitedDirectories { get; set; } = new List<string>();

        public static void WriteApplicationSetting(ApplicationSetting setting)
        {
            var jsonSerializeSetting = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented,
            };

            string data = JsonConvert.SerializeObject(setting, jsonSerializeSetting);

            using (StreamWriter sw = File.CreateText(@"applicationSettings.json"))
            {
                sw.Write(data);
            }
        }

        public static ApplicationSetting ReadApplicationSetting(string jsonFilePath)
        {
            using (var reader = new StreamReader(jsonFilePath))
            {
                return new JsonSerializer().Deserialize<ApplicationSetting>(new JsonTextReader(reader));
            }
        }
    }
}