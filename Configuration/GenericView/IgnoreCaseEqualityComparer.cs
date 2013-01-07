using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Configuration.GenericView
{
	internal class IgnoreCaseEqualityComparer: IEqualityComparer<string>
	{
		public static readonly IgnoreCaseEqualityComparer Instance = new IgnoreCaseEqualityComparer();

		public bool Equals(string x, string y)
		{
			return string.Equals(x, y, StringComparison.InvariantCultureIgnoreCase);
		}

		public int GetHashCode(string obj)
		{
			return obj.ToLowerInvariant().GetHashCode();
		}
	}
}
