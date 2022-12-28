using System.Collections.Generic;
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
            setting.Favorites = new List<Favorite>() { new Favorite() { Key = "a" }, new Favorite() { Key = "b" } };

            ApplicationSetting.WriteApplicationSetting(setting);
        }
    }
}