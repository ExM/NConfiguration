using System;
using NUnit.Framework;
using System.Collections.Specialized;
using System.Configuration;
using Configuration.Xml.Joining;

namespace Configuration
{
	[TestFixture]
	public class XmlFileSettingsTests
	{
		private IAppSettings _settings;
		
		[TestFixtureSetUp]
		public void SetUp()
		{
			_settings = SettingsLoader.FromXmlFile("testConfig1.xml").Settings;
		}
		
		[Test]
		public void ReadForDefaultName()
		{
			var cfg = _settings.Load<MyXmlConfig>();
			
			Assert.AreEqual("attr field text", cfg.AttrField);
			Assert.AreEqual("elem field text", cfg.ElemField);
		}
		
		[Test, ExpectedException(typeof(SectionNotFoundException))]
		public void SectionNotFound()
		{
			_settings.Load<MyXmlConfig>("MyCfg3");
		}
		
		[Test]
		public void ReadDefaultSection()
		{
			var cfg = _settings.TryLoad<MyXmlConfig>("MyCfg3", true);
			Assert.IsNotNull(cfg);
			Assert.AreEqual("default", cfg.AttrField);
		}
		
		[Test]
		public void ReadNullSection()
		{
			var cfg = _settings.TryLoad<MyXmlConfig>("MyCfg3", false);
			Assert.IsNull(cfg);
		}
		
		[Test]
		public void ReadForSpecifiedName()
		{
			var cfg = _settings.Load<MyXmlConfig>("MyCfg2");
			
			Assert.AreEqual("2", cfg.AttrField);
		}
	}
}

