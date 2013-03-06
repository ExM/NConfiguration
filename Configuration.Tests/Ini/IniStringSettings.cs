using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using Configuration.Xml;
using Configuration.GenericView;
using Configuration.Tests;
using System.Collections.Generic;

namespace Configuration.Ini
{
	public class IniStringSettings : IniSettings
	{
		private readonly List<Section> _sections;

		public IniStringSettings(string text)
			:base(Global.PlainConverter, Global.GenericDeserializer)
		{
			_sections = Section.Parse(text);
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

