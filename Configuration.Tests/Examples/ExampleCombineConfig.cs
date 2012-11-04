using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Configuration.Examples
{
	public class ExampleCombineConfig : ICombinable
	{
		[XmlAttribute]
		public string F = null;

		public void Combine(object other)
		{
			Combine(other as ExampleCombineConfig);
		}

		public void Combine(ExampleCombineConfig other)
		{
			if (other == null)
				return;

			F = other.F ?? F;
		}
	}
}

