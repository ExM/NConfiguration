using System.Collections.Generic;
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
			var cfg = new XmlSystemSettings("ExtConfigure").ToAppSettings().First<MyXmlConfig>();
			
			Assert.AreEqual("attr field text", cfg.AttrField);
			Assert.AreEqual("elem field text", cfg.ElemField);
		}

		[Test]
		public void IncludeByRelativePath()
		{
			var loader = new SettingsLoader();
			loader.XmlFileByExtension();
			var systemSettings = new XmlSystemSettings("ExtConfigure");
			var settings = loader.LoadSettings(systemSettings).ToAppSettings();

			var cfg = settings.Get<MyXmlConfig>("MyCfg2");

			Assert.AreEqual("2", cfg.AttrField);
			Assert.IsNull(cfg.ElemField);
		}

		[Test]
		public void IncludeByFixedPathOwner()
		{
			var localPath = "".ResolveTestPath();

			var systemSettings = new XmlSystemSettings("ExtConfigure");
			var fixupPathOwner = new FixupPathOwner(systemSettings.Items, localPath, systemSettings.Identity);

			var loader = new SettingsLoader();
			loader.XmlFileByExtension();
			var settings = loader.LoadSettings(fixupPathOwner).ToAppSettings();

			var cfg = settings.Get<MyXmlConfig>("MyCfg2");

			Assert.AreEqual("2", cfg.AttrField);
			Assert.IsNull(cfg.ElemField);
		}

		public class FixupPathOwner : DefaultConfigNodeProvider, IFilePathOwner, IIdentifiedSource
		{
			public FixupPathOwner(IEnumerable<KeyValuePair<string, ICfgNode>> items, string path, string identity) : base(items)
			{
				Path = path;
				Identity = identity;
			}

			public string Path { get; private set; }

			public string Identity { get; private set; }
		}
	}
}

