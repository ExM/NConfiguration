using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using Configuration.Xml;
using Configuration.GenericView;
using Configuration.Tests;
using System.Collections.Generic;
using Configuration.Ini.Parsing;

namespace Configuration.Ini
{
	public class IniStringSettings : IniSettings
	{
		private readonly List<Section> _sections;

		public IniStringSettings(string text)
			:base(Global.XmlViewConverter, Global.GenericDeserializer)
		{
			var context = new ParseContext();
			context.ParseSource(text);
			_sections = new List<Section>(context.Sections);
		}

		protected override IEnumerable<Section> Sections
		{
			get
			{
				return _sections;
			}
		}
	}
}

