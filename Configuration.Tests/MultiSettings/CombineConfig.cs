using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Configuration
{
	public class CombineConfig : ICombinable<CombineConfig>
	{
		[XmlAttribute]
		public string F = null;

		public CombineConfig Combine(CombineConfig prev, CombineConfig next)
		{
			if(prev == null)
				return next;

			if(next == null)
				return prev;
			
			prev.F = next.F ?? prev.F;

			return prev;
		}
	}
}

