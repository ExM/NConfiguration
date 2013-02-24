using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Configuration.GenericView
{
	internal class NameComparer: IEqualityComparer<string>
	{
		public static readonly NameComparer Instance = new NameComparer();

		bool IEqualityComparer<string>.Equals(string x, string y)
		{
			return string.Equals(x, y, StringComparison.InvariantCultureIgnoreCase);
		}

		int IEqualityComparer<string>.GetHashCode(string obj)
		{
			return obj.ToLowerInvariant().GetHashCode();
		}

		public static bool Equals(string x, string y)
		{
			return string.Equals(x, y, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
