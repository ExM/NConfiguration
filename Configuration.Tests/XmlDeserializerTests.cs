using System;
using NUnit.Framework;
using System.Collections.Specialized;
using System.Configuration;
using Configuration.Xml.Joining;
using Configuration.Xml;

namespace Configuration
{
	[TestFixture]
	public class XmlDeserializerTests
	{
		[Test]
		public void Caching()
		{
			var xs1 = XmlDeserializer<MyXmlConfig>.GetSerializer("test");
			var xs2 = XmlDeserializer<MyXmlConfig>.GetSerializer("test");
			
			Assert.IsTrue(object.ReferenceEquals(xs1, xs2));
		}
		
		[Test]
		public void ClearCache()
		{
			var xs1 = XmlDeserializer<MyXmlConfig>.GetSerializer("test");
			
			GC.Collect();
			GC.WaitForPendingFinalizers();

			var xs2 = XmlDeserializer<MyXmlConfig>.GetSerializer("test");

			Assert.IsFalse(object.ReferenceEquals(xs1, xs2));
		}
	}
}

