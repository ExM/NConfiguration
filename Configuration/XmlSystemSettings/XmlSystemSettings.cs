using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Configuration
{
	public class XmlSystemSettings : IAppSettings
	{
		private XDocument _doc = null;

		public XmlSystemSettings(string sectionName)
		{
			var section = ConfigurationManager.GetSection(sectionName) as PlainXmlSection;
			if(section == null)
				throw new InvalidOperationException(string.Format("section `{0}' not found", sectionName));

			_doc = section.PlainXml;
		}

		/// <summary>
		/// Trying to load the configuration.
		/// </summary>
		/// <returns>
		/// Instance of the configuration, or null if no section name
		/// </returns>
		/// <param name='sectionName'>instance of application settings</param>
		/// <typeparam name='T'>type of configuration</typeparam>
		public T TryLoad<T>(string sectionName) where T : class
		{
			if(sectionName == null)
				throw new ArgumentNullException("sectionName");

			var xs = new XmlSerializer(typeof(T), new XmlRootAttribute(sectionName));
			var el = _doc.Root.Element(XNamespace.None + sectionName);
			if(el == null)
				return null;

			using(XmlReader xr = el.CreateReader())
				return (T)xs.Deserialize(xr);
		}
	}
}
