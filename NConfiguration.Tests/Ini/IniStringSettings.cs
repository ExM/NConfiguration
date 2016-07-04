using System.Collections.Generic;

namespace NConfiguration.Ini
{
	public class IniStringSettings : IniSettings
	{
		private readonly List<Section> _sections;

		public IniStringSettings(string text)
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

