using System.Xml.Linq;

namespace Configuration
{
	/// <summary>
	/// store application settings in XML elements
	/// </summary>
	public interface IXmlSettings
	{
		XElement GetSection(string name);
	}
}
