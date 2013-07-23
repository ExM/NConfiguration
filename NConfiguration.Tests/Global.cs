using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NConfiguration.GenericView;

namespace NConfiguration.Tests
{
	public static class Global
	{
		//public static readonly IXmlViewConverter XmlViewConverter = new XmlViewConverter();

		public static readonly IStringConverter PlainConverter = new StringConverter();

		public static readonly IGenericDeserializer GenericDeserializer = new GenericDeserializer();
	}
}
