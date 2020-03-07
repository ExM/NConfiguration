using System.Configuration;
using System.Reflection;
using NConfiguration.Joining;
using NConfiguration.Xml;
using NUnit.Framework;

namespace NConfiguration
{
	[TestFixture]
	public class XmlSystemSettingsTests
	{
		[Test]
		public void ReadForDefaultName()
		{
			var path = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location).FilePath;
		
			var systemSettings = new XmlSystemSettings(path, "ExtConfigure");
			var appSettings = systemSettings.ToAppSettings();
			var cfg = appSettings.First<MyXmlConfig>();

			Assert.AreEqual("attr field text", cfg.AttrField);
			Assert.AreEqual("elem field text", cfg.ElemField);
		}

		[Test]
		public void IncludeByRelativePath()
		{
			var path = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location).FilePath;

			var loader = new SettingsLoader();
			loader.XmlFileByExtension();
			var systemSettings = new XmlSystemSettings(path, "ExtConfigure");
			var settings = loader.LoadSettings(systemSettings).Joined.ToAppSettings();

			var cfg = settings.Get<MyXmlConfig>("MyCfg2");

			Assert.AreEqual("2", cfg.AttrField);
			Assert.IsNull(cfg.ElemField);
		}
	}
}

