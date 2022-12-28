using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Filer.Models.Settings
{
    public class ApplicationSetting
    {
        public string TestValue { get; set; }

        public List<Favorite> Favorites { get; set; }

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
    }
}