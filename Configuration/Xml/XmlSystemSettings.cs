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
	public class XmlSystemSettings : XmlSettings, IRelativePathOwner
	{
		private readonly string _directory;

		public XmlSystemSettings(string sectionName)
		{
			var confFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
			confFile = Path.GetFullPath(confFile);
			_directory = Path.GetDirectoryName(confFile);

			try
			{
				var section = ConfigurationManager.GetSection(sectionName) as PlainXmlSection;
				if(section == null || section.PlainXml == null)
					throw new FormatException("section not found");

				Root = section.PlainXml.Root;
			}
			catch(SystemException ex)
			{
				throw new ApplicationException(string.Format("Unable to system section `{0}'", sectionName), ex);
			}
		}

		public string RelativePath
		{
			get
			{
				return _directory;
			}
		}
	}
}
