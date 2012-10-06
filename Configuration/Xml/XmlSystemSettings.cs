using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Configuration.Xml
{
	public class XmlSystemSettings : XmlSettings
	{
		public XmlSystemSettings(string sectionName)
			:base(LoadFromSystemConfig(sectionName))
		{
		}
		
		private static XDocument LoadFromSystemConfig(string sectionName)
		{
			try
			{
				var section = ConfigurationManager.GetSection(sectionName) as PlainXmlSection;
				if(section == null)
					throw new FormatException("section not found");

				return section.PlainXml;
			}
			catch(SystemException ex)
			{
				throw new ApplicationException(string.Format("Unable to system section `{0}'", sectionName), ex);
			}
		}
	}
}
