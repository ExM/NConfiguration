using System;
using NUnit.Framework;
using Configuration.Xml.Joining;

namespace Configuration
{
	[TestFixture]
	public class XmlSystemSettingsTests
	{
		[Test]
		public void ReadForDefaultName()
		{
			var cfg = SettingsLoader.FromConfigSection("ExtConfigure").Settings.Load<MyXmlConfig>();
			
			Assert.AreEqual("attr field text", cfg.AttrField);
			Assert.AreEqual("elem field text", cfg.ElemField);
		}
	}
}

