using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using Configuration.Xml.Protected;
using Configuration.GenericView;
using System.Collections.Generic;
using Configuration.Ini.Parsing;

namespace Configuration.Ini
{
	public class IniFileSettings : IniSettings, IFilePathOwner, IAppSettingSource
	{
		private readonly List<Section> _sections;

		public IniFileSettings(string fileName, IStringConverter converter, IGenericDeserializer deserializer)
			: base(converter, deserializer)
		{
			try
			{
				fileName = System.IO.Path.GetFullPath(fileName);
				var text = System.IO.File.ReadAllText(fileName);
				
				var context = new ParseContext();
				context.ParseSource(text);
				_sections = new List<Section>(context.Sections);

				Identity = this.GetIdentitySource(fileName);
				Path = System.IO.Path.GetDirectoryName(fileName);
			}
			catch(SystemException ex)
			{
				throw new ApplicationException(string.Format("Unable to load file `{0}'", fileName), ex);
			}
		}

		protected override IEnumerable<Section> Sections
		{
			get
			{
				return _sections;
			}
		}
		
		public string Identity {get; private set;}

		public string Path {get; private set;}
	}
}

