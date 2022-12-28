using System.Diagnostics;
using Newtonsoft.Json;

namespace Filer.Models.Settings
{
    public class ApplicationSetting
    {
        public string TestValue { get; set; }

        public static void WriteApplicationSetting(ApplicationSetting setting)
        {
            string data = JsonConvert.SerializeObject(setting);
            Debug.WriteLine(data);
        }
    }
}