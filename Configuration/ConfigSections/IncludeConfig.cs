using System;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Configuration.ConfigSections
{
	[XmlRoot("Include")]
	public class IncludeConfig
	{
		[XmlAttribute("Files")]
		public string FilePaths { get; set;}
		
		[XmlElement("File")]
		public List<IncludeFileConfig> FileConfigs { get; set;}
		
		[XmlAttribute("FinalSearch")]
		public bool FinalSearch { get; set;}
		
		[XmlIgnore]
		public IEnumerable<IncludeFileConfig> Files
		{
			get
			{
				if(FilePaths == null)
					return FileConfigs;
				
				return FilePaths.Split(';')
					.Where(p => !string.IsNullOrWhiteSpace(p))
					.Select(p => new IncludeFileConfig(){ Path = p}).Union(FileConfigs);
			}
		}
	}
}

