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
	public class XmlSystemSettings : XmlSettings, IFilePathOwner
	{
		private readonly string _sectionName;
		private readonly string _directory;

		public XmlSystemSettings(string sectionName)
		{
			_sectionName = sectionName;
			var confFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
			confFile = System.IO.Path.GetFullPath(confFile);
			_directory = System.IO.Path.GetDirectoryName(confFile);

			try
			{
				var section = ConfigurationManager.GetSection(_sectionName) as PlainXmlSection;
				if(section == null || section.PlainXml == null)
					throw new FormatException("section not found");

				Root = section.PlainXml.Root;
			}
			catch(SystemException ex)
			{
				throw new ApplicationException(string.Format("Unable to system section `{0}'", sectionName), ex);
			}
		}

		public override string Identity
		{
			get
			{
				return _sectionName;
			}
		}

		public string Path
		{
			get
			{
				return _directory;
			}
		}
	}
}
