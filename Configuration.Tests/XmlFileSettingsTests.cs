using System;
using NUnit.Framework;
using System.Collections.Specialized;
using System.Configuration;


namespace Configuration
{
	[TestFixture]
	public class XmlFileSettingsTests : TestBase
	{
		[Test]
		public void ReadForDefaultName()
		{
			var cfg = GetXmlSettings("Config1").Load<MyXmlConfig>();
			
			Assert.AreEqual("attr field text", cfg.AttrField);
			Assert.AreEqual("elem field text", cfg.ElemField);
		}
		
		[Test, ExpectedException(typeof(SectionNotFoundException))]
		public void SectionNotFound()
		{
			GetXmlSettings("Config1").Load<MyXmlConfig>("MyCfg3");
		}
		
		[Test]
		public void ReadDefaultSection()
		{
			var cfg = GetXmlSettings("Config1").TryLoad<MyXmlConfig>("MyCfg3", true);
			Assert.IsNotNull(cfg);
			Assert.AreEqual("default", cfg.AttrField);
		}
		
		[Test]
		public void ReadNullSection()
		{
			var cfg = GetXmlSettings("Config1").TryLoad<MyXmlConfig>("MyCfg3", false);
			Assert.IsNull(cfg);
		}
		
		[Test]
		public void ReadForSpecifiedName()
		{
			var cfg = GetXmlSettings("Config1").Load<MyXmlConfig>("MyCfg2");
			
			Assert.AreEqual("2", cfg.AttrField);
		}
	}
}

