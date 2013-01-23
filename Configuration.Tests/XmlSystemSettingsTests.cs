using Configuration.Xml;
using NUnit.Framework;
using Configuration.GenericView;
using Configuration.Tests;

namespace Configuration
{
	[TestFixture]
	public class XmlSystemSettingsTests
	{
		[Test]
		public void ReadForDefaultName()
		{
			var cfg = new XmlSystemSettings("ExtConfigure", Global.XmlViewConverter, Global.GenericDeserializer).First<MyXmlConfig>();
			
			Assert.AreEqual("attr field text", cfg.AttrField);
			Assert.AreEqual("elem field text", cfg.ElemField);
		}
	}
}

