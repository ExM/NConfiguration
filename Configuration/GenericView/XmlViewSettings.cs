using System;
using System.Collections.Generic;

namespace Configuration.GenericView
{
	public class XmlViewSettings
	{
		private Dictionary<Type, object> _map = new Dictionary<Type, object>();

		public XmlViewSettings()
		{
			_map.Add(typeof(string), (Func<string, string>)GetString);
			_map.Add(typeof(bool), (Func<string, bool>)GetBoolean);
			_map.Add(typeof(bool?), (Func<string, bool?>)GetNBoolean);
		}

		public static string GetString(string text)
		{
			return text;
		}

		private static Dictionary<string, bool> _booleanMap = new Dictionary<string, bool>(IgnoreCaseEqualityComparer.Instance)
		{
			{"true", true},
			{"yes", true},
			{"1", true},
			{"+", true},
			{"t", true},
			{"y", true},
			{"false", false},
			{"no", false},
			{"0", false},
			{"-", false},
			{"f", false},
			{"n", false}
		};

		public static bool GetBoolean(string text)
		{
			bool result;
			if (_booleanMap.TryGetValue(text, out result))
				return result;

			throw new FormatException(string.Format("can not convert '{0}' to a boolean type", text));
		}

		public static bool? GetNBoolean(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return GetBoolean(text);
		}

		public void SetConvert<T>(Func<string, T> conv)
		{
			_map[typeof(T)] = conv;
		}

		public T Convert<T>(string text)
		{
			object conv;
			if (!_map.TryGetValue(typeof(T), out conv))
				throw new ApplicationException(string.Format("unknown type: {0}", typeof(T).FullName));

			var func = (Func<string, T>)conv;

			return func(text);
		}
	}
}

