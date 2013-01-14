using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Configuration.GenericView
{
	public class EnumHelper<T> where T: struct
	{
		private static readonly Dictionary<string, T> _nameMap = new Dictionary<string, T>();
		private static readonly Dictionary<ulong, T> _numMap = new Dictionary<ulong, T>();

		static EnumHelper()
		{
			foreach (T item in Enum.GetValues(typeof(T)).Cast<T>())
			{
				T exist;
				string strkey = item.ToString().ToLowerInvariant();
				if (!_nameMap.TryGetValue(strkey, out exist))
					_nameMap.Add(strkey, item);

				ulong numkey = (ulong)(ValueType)item;
				if (!_numMap.TryGetValue(numkey, out exist))
					_numMap.Add(numkey, item);
			}
		}

		public static T Parse(string text)
		{
			T exist;
			if (_nameMap.TryGetValue(text, out exist))
				return exist;

			ulong num;
			if(ulong.TryParse(text, NumberStyles.Integer | NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num))
			{
				if (_numMap.TryGetValue(num, out exist))
					return exist;
			}

			throw new InvalidCastException(string.Format("enum {0} not contain value '{1}'", typeof(T).FullName, text));
		}
	}
}
