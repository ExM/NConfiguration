using System;
using NUnit.Framework;
namespace Configuration
{
	[TestFixture]
	public class ConfigFileTest
	{
		[Test]
		public void ReadForDefaultNameSection()
		{
			FileSettings file = new FileSettings("testConfig1.xml");
			MyXmlConfig cfg = file.Load<MyXmlConfig>(EmptyResult.Null);
			
			Assert.IsNotNull(cfg);
			Assert.AreEqual("attr field text", cfg.AttrField);
			Assert.AreEqual("elem field text", cfg.ElemField);
		}
		
		[Test]
		public void ReadForSpecifiedNameSection()
		{
			FileSettings file = new FileSettings("testConfig1.xml");
			MyXmlConfig cfg = file.Load<MyXmlConfig>(EmptyResult.Throw, "MyCfg2");
			
			Assert.AreEqual("2", cfg.AttrField);
		}
	}
}

