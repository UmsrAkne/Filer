using Filer.Models.Settings;
using NUnit.Framework;

namespace Tests.Models.Settings
{
    public class ApplicationSettingTest
    {
        [Test]
        public void WriteApplicationSettingTest()
        {
            var setting = new ApplicationSetting();
            setting.TestValue = "testの値";
            
            ApplicationSetting.WriteApplicationSetting(setting);
        }
    }
}