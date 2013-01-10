using System;
using System.Collections.Generic;
using System.Globalization;

namespace Configuration.GenericView
{
	public partial class XmlViewConverter
	{
		private readonly CultureInfo _ci;
		private readonly Dictionary<Type, object> _map = new Dictionary<Type, object>();

		public XmlViewConverter()
			: this(CultureInfo.InvariantCulture)
		{
		}

		public XmlViewConverter(CultureInfo ci)
		{
			_ci = ci;
			InitMap();
		}
		
		public Byte[] ToByteArray(string text)
		{
			return System.Convert.FromBase64String(text);
		}

		public Char ToChar(string text)
		{
			if (text.Length != 1)
				throw new ArgumentOutOfRangeException("text", "must contain only one char");
			return text[0];
		}

		public UInt16 ToUInt16(string text)
		{
			return UInt16.Parse(text, _ci);
		}

		public Int16 ToInt16(string text)
		{
			return Int16.Parse(text, _ci);
		}

		public UInt32 ToUInt32(string text)
		{
			return UInt32.Parse(text, _ci);
		}

		public Int32 ToInt32(string text)
		{
			return Int32.Parse(text, _ci);
		}

		public UInt64 ToUInt64(string text)
		{
			return UInt64.Parse(text, _ci);
		}

		public Int64 ToInt64(string text)
		{
			return Int64.Parse(text, _ci);
		}

		public Single ToSingle(string text)
		{
			return Single.Parse(text, _ci);
		}

		public Double ToDouble(string text)
		{
			return Double.Parse(text, _ci);
		}

		public SByte ToSByte(string text)
		{
			return SByte.Parse(text, _ci);
		}

		public Byte ToByte(string text)
		{
			return Byte.Parse(text, _ci);
		}

		public TimeSpan ToTimeSpan(string text)
		{
			return TimeSpan.Parse(text, _ci);
		}

		public DateTime ToDateTime(string text)
		{
			return DateTime.Parse(text, _ci,
				DateTimeStyles.AdjustToUniversal |
				DateTimeStyles.AllowWhiteSpaces |
				DateTimeStyles.AssumeUniversal |
				DateTimeStyles.NoCurrentDateDefault);
		}

		public string ToString(string text)
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

		public bool ToBoolean(string text)
		{
			bool result;
			if (_booleanMap.TryGetValue(text, out result))
				return result;

			throw new FormatException(string.Format("can not convert '{0}' to a boolean type", text));
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

