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
			var systemSettings = new XmlSystemSettings("ExtConfigure");
			var appSettings = systemSettings.ToAppSettings();
			var cfg = appSettings.First<MyXmlConfig>();

			Assert.AreEqual("attr field text", cfg.AttrField);
			Assert.AreEqual("elem field text", cfg.ElemField);
		}

		[Test]
		public void IncludeByRelativePath()
		{
			var loader = new SettingsLoader();
			loader.XmlFileByExtension();
			var systemSettings = new XmlSystemSettings("ExtConfigure");
			var settings = loader.LoadSettings(systemSettings).Joined.ToAppSettings();

			var cfg = settings.Get<MyXmlConfig>("MyCfg2");

			Assert.AreEqual("2", cfg.AttrField);
			Assert.IsNull(cfg.ElemField);
		}

		[Test]
		public void IncludeByFixedPathOwner()
		{
			var localPath = "".ResolveTestPath();
			var systemSettings = new XmlSystemSettings("ExtConfigure", localPath); // fix for R# test runner

			var loader = new SettingsLoader();
			loader.XmlFileByExtension();
			var settings = loader.LoadSettings(systemSettings).Joined.ToAppSettings();

			var cfg = settings.Get<MyXmlConfig>("MyCfg2");

			Assert.AreEqual("2", cfg.AttrField);
			Assert.IsNull(cfg.ElemField);
		}
	}
}

