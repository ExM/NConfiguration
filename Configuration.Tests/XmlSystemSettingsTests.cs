using System;
using NUnit.Framework;
namespace Configuration
{
	[TestFixture]
	public class XmlSystemSettingsTests : TestBase
	{
		[Test]
		public void ReadForDefaultName()
		{
			var cfg = new XmlSystemSettings("ExtConfigure").Load<MyXmlConfig>();
			
			Assert.AreEqual("attr field text", cfg.AttrField);
			Assert.AreEqual("elem field text", cfg.ElemField);
		}
	}
}

