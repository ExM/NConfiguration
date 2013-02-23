using System;
using System.Collections.Generic;
using System.Globalization;

namespace Configuration.GenericView
{
	public partial class StringMapper
	{
		private object CreateConverterFromString(Type type)
		{
			if(type == typeof(string))
				return (Func<string, string>)ToString;
			else if(type == typeof(byte[]))
				return (Func<string, byte[]>)ToByteArray;
			else if(type == typeof(Boolean))
				return (Func<string, Boolean>)ToBoolean;
			else if(type == typeof(Boolean?))
				return (Func<string, Boolean?>)ToNBoolean;
			else if(type == typeof(Byte))
				return (Func<string, Byte>)ToByte;
			else if(type == typeof(Byte?))
				return (Func<string, Byte?>)ToNByte;
			else if(type == typeof(SByte))
				return (Func<string, SByte>)ToSByte;
			else if(type == typeof(SByte?))
				return (Func<string, SByte?>)ToNSByte;
			else if(type == typeof(Char))
				return (Func<string, Char>)ToChar;
			else if(type == typeof(Char?))
				return (Func<string, Char?>)ToNChar;
			else if(type == typeof(Int16))
				return (Func<string, Int16>)ToInt16;
			else if(type == typeof(Int16?))
				return (Func<string, Int16?>)ToNInt16;
			else if(type == typeof(Int32))
				return (Func<string, Int32>)ToInt32;
			else if(type == typeof(Int32?))
				return (Func<string, Int32?>)ToNInt32;
			else if(type == typeof(Int64))
				return (Func<string, Int64>)ToInt64;
			else if(type == typeof(Int64?))
				return (Func<string, Int64?>)ToNInt64;
			else if(type == typeof(UInt16))
				return (Func<string, UInt16>)ToUInt16;
			else if(type == typeof(UInt16?))
				return (Func<string, UInt16?>)ToNUInt16;
			else if(type == typeof(UInt32))
				return (Func<string, UInt32>)ToUInt32;
			else if(type == typeof(UInt32?))
				return (Func<string, UInt32?>)ToNUInt32;
			else if(type == typeof(UInt64))
				return (Func<string, UInt64>)ToUInt64;
			else if(type == typeof(UInt64?))
				return (Func<string, UInt64?>)ToNUInt64;
			else if(type == typeof(Single))
				return (Func<string, Single>)ToSingle;
			else if(type == typeof(Single?))
				return (Func<string, Single?>)ToNSingle;
			else if(type == typeof(Double))
				return (Func<string, Double>)ToDouble;
			else if(type == typeof(Double?))
				return (Func<string, Double?>)ToNDouble;
			else if(type == typeof(TimeSpan))
				return (Func<string, TimeSpan>)ToTimeSpan;
			else if(type == typeof(TimeSpan?))
				return (Func<string, TimeSpan?>)ToNTimeSpan;
			else if(type == typeof(DateTime))
				return (Func<string, DateTime>)ToDateTime;
			else if(type == typeof(DateTime?))
				return (Func<string, DateTime?>)ToNDateTime;
			
			return DefaultConverter(type);
		}

		
		public Boolean? ToNBoolean(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return ToBoolean(text);
		}
		
		public Byte? ToNByte(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return ToByte(text);
		}
		
		public SByte? ToNSByte(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return ToSByte(text);
		}
		
		public Char? ToNChar(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return ToChar(text);
		}
		
		public Int16? ToNInt16(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return ToInt16(text);
		}
		
		public Int32? ToNInt32(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return ToInt32(text);
		}
		
		public Int64? ToNInt64(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return ToInt64(text);
		}
		
		public UInt16? ToNUInt16(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return ToUInt16(text);
		}
		
		public UInt32? ToNUInt32(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return ToUInt32(text);
		}
		
		public UInt64? ToNUInt64(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return ToUInt64(text);
		}
		
		public Single? ToNSingle(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return ToSingle(text);
		}
		
		public Double? ToNDouble(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return ToDouble(text);
		}
		
		public TimeSpan? ToNTimeSpan(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return ToTimeSpan(text);
		}
		
		public DateTime? ToNDateTime(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return ToDateTime(text);
		}
		
	}
}

