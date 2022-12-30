using System.Collections.Generic;
using System.IO;
using Filer.Models.Settings;
using NUnit.Framework;

namespace Tests.Models.Settings
{
    public class ApplicationSettingTest
    {
        [TearDown]
        public void CleanUp()
        {
            File.Delete("applicationSettings.json");
        }

        [Test]
        public void WriteApplicationSettingTest()
        {
            var setting = new ApplicationSetting();
            setting.TestValue = "testの値";
            setting.Favorites = new List<Favorite>() { new Favorite() { Key = "a" }, new Favorite() { Key = "b" } };

            ApplicationSetting.WriteApplicationSetting(setting);
        }

        [Test]
        public void ReadApplicationSettingTest()
        {
            var setting = new ApplicationSetting();
            setting.Favorites.Add(new Favorite() { Key = "a" });
            setting.Favorites.Add(new Favorite() { Key = "b" });

            ApplicationSetting.WriteApplicationSetting(setting);
            var applicationSetting = ApplicationSetting.ReadApplicationSetting("applicationSettings.json");

            Assert.AreEqual(2, applicationSetting.Favorites.Count);
            Assert.AreEqual("a", applicationSetting.Favorites[0].Key);
            Assert.AreEqual("b", applicationSetting.Favorites[1].Key);
        }
    }
}