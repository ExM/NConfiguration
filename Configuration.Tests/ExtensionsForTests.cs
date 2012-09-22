using System.Xml;
using System.Xml.Linq;

namespace Configuration
{
	public static class ExtensionsForTests
	{
		public static IAppSettings ToXmlSettings(this string text)
		{
			return new XmlStringSettings(text);
		}
	}
}



