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
		private readonly string _directory;
		private readonly string _identity;

		public IniFileSettings(string fileName, IXmlViewConverter converter, IGenericDeserializer deserializer)
			: base(converter, deserializer)
		{
			try
			{
				fileName = System.IO.Path.GetFullPath(fileName);
				var text = System.IO.File.ReadAllText(fileName);
				
				var context = new ParseContext();
				context.ParseSource(text);
				_sections = new List<Section>(context.Sections);

				_identity = GetCustomIdentity();
				if (_identity == null)
					_identity = fileName;

				_directory = System.IO.Path.GetDirectoryName(fileName);
			}
			catch(SystemException ex)
			{
				throw new ApplicationException(string.Format("Unable to load file `{0}'", fileName), ex);
			}
		}

		private string GetCustomIdentity()
		{
			if (_sections.Count == 0)
				return null;

			if (_sections[0].Name != string.Empty)
				return null;

			return _sections[0].Pairs.Where(p => p.Key == "Identity").Select(p => p.Value).FirstOrDefault();
		}

		protected override IEnumerable<Section> Sections
		{
			get
			{
				return _sections;
			}
		}
		
		public string Identity
		{
			get
			{
				return _identity;
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

