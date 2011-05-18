using System;
using NUnit.Framework;
using System.Xml.Linq;
namespace Configuration
{
	[TestFixture]
	public class XDocumentTest
	{
		[Test]
		public void SaveSection()
		{
			XDocument doc = XDocument.Parse("<root />");
			
			MyXmlConfig cfg = new MyXmlConfig();
			cfg.AttrField = "text";
			
			doc.SaveElement(cfg, "CFG");
			
			Assert.AreEqual("text", doc.Root.Element(XNamespace.None + "CFG").Attribute("AttrField").Value);
		}
		
		[Test]
		public void ReplaceSection()
		{
			XDocument doc = XDocument.Parse("<root />");
			
			MyXmlConfig cfg = new MyXmlConfig();
			cfg.AttrField = "text";
			
			doc.SaveElement(cfg, "CFG");
			
			MyXmlConfig cfg2 = new MyXmlConfig();
			cfg2.AttrField = "text2";
			
			doc.SaveElement(cfg2, "CFG");
			
			Assert.AreEqual("text2", doc.Root.Element(XNamespace.None + "CFG").Attribute("AttrField").Value);
		}

	}
}

