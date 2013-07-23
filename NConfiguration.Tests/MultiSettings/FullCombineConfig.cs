using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace NConfiguration
{
	public class FullCombineConfig
	{
		[XmlAttribute]
		public string F = null;

		public static FullCombineConfig Combine(FullCombineConfig prev, FullCombineConfig next)
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

