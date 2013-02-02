using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Configuration.Ini
{
	
	public class Section
	{
		public string Name { get; private set; }

		public List<KeyValuePair<string, string>> Pairs { get; private set; }

		public Section(string name)
		{
			Name = name;
			Pairs = new List<KeyValuePair<string, string>>();
		}
	}
}
