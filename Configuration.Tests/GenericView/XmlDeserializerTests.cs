using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Xml;
using System.Xml.Serialization;
using NUnit.Framework;
using Configuration.GenericView.Deserialization;

namespace Configuration.GenericView
{
	[TestFixture]
	public class XmlDeserializerTests
	{
		public class BadType1
		{
			[XmlArray]
			public int[] Array { get; set; }
		}

		public class BadType2
		{
			[XmlAnyElement]
			public XmlElement AnyElement { get; set; }
		}

		[Test]
		public void ParseBadType()
		{
			var root = XmlView.Create(
@"<Root></Root>".ToXDocument());
			var d = new GenericDeserializer();

			d.Deserialize<BadType1>(root);
		}
	}
}
