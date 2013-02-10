using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Collections.Concurrent;
using System.Xml.Linq;

namespace Configuration.GenericView
{
	public partial class PlainMapper : IPlainMapper
	{
		private CultureInfo _ci;

		public CultureInfo CurrentCulture
		{
			get
			{
				return _ci;
			}
		}

		public PlainMapper()
			:this(CultureInfo.InvariantCulture)
		{
		}

		public PlainMapper(CultureInfo ci)
		{
			if (ci == null)
				throw new ArgumentNullException("ci");

			_ci = ci;
		}

		public virtual object CreateFunction(Type src, Type dst)
		{
			if (src == dst)
				return CreateNativeConverter(src);

			if (src == typeof(string) && dst.IsEnum)
				return CreateStringToEnum(dst);

			if (src == typeof(string))
			{
				if(dst.IsEnum)
					return CreateStringToEnum(dst);
				else
					return CreateConverterFromString(dst);
			}

			return DefaultConverter(src, dst);
		}

		private object DefaultConverter(Type src, Type dst)
		{
			var mi = typeof(PlainMapper).GetMethod("DefaultConverter", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(src, dst);
			var funcType = typeof(Func<,>).MakeGenericType(src, dst);
			return Delegate.CreateDelegate(funcType, mi);
		}

		private static TDest DefaultConverter<TSrc, TDest>(TSrc val)
		{
			return (TDest)Convert.ChangeType(val, typeof(TDest));
		}

		private object CreateNativeConverter(Type src)
		{
			var mi = typeof(PlainMapper).GetMethod("NativeConverter", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(src);
			var funcType = typeof(Func<,>).MakeGenericType(src, src);
			return Delegate.CreateDelegate(funcType, mi);
		}

		private static T NativeConverter<T>(T val)
		{
			return val;
		}

		private object CreateStringToEnum(Type dst)
		{
			var mi = typeof(EnumHelper<>).MakeGenericType(dst).GetMethod("Parse", BindingFlags.Public | BindingFlags.Static);
			var funcType = typeof(Func<,>).MakeGenericType(typeof(string), dst);
			return Delegate.CreateDelegate(funcType, mi);
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
	}
}

