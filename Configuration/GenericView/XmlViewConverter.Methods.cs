using System;
using System.Collections.Generic;
using System.Globalization;

namespace Configuration.GenericView
{
	public partial class XmlViewConverter
	{
		private void InitMap()
		{
			_map.Add(typeof(string), (Func<string, string>)ToString);
			_map.Add(typeof(byte[]), (Func<string, byte[]>)ToByteArray);
			_map.Add(typeof(Boolean), (Func<string, Boolean>)ToBoolean);
			_map.Add(typeof(Boolean?), (Func<string, Boolean?>)ToNBoolean);
			_map.Add(typeof(Byte), (Func<string, Byte>)ToByte);
			_map.Add(typeof(Byte?), (Func<string, Byte?>)ToNByte);
			_map.Add(typeof(SByte), (Func<string, SByte>)ToSByte);
			_map.Add(typeof(SByte?), (Func<string, SByte?>)ToNSByte);
			_map.Add(typeof(Char), (Func<string, Char>)ToChar);
			_map.Add(typeof(Char?), (Func<string, Char?>)ToNChar);
			_map.Add(typeof(Int16), (Func<string, Int16>)ToInt16);
			_map.Add(typeof(Int16?), (Func<string, Int16?>)ToNInt16);
			_map.Add(typeof(Int32), (Func<string, Int32>)ToInt32);
			_map.Add(typeof(Int32?), (Func<string, Int32?>)ToNInt32);
			_map.Add(typeof(Int64), (Func<string, Int64>)ToInt64);
			_map.Add(typeof(Int64?), (Func<string, Int64?>)ToNInt64);
			_map.Add(typeof(UInt16), (Func<string, UInt16>)ToUInt16);
			_map.Add(typeof(UInt16?), (Func<string, UInt16?>)ToNUInt16);
			_map.Add(typeof(UInt32), (Func<string, UInt32>)ToUInt32);
			_map.Add(typeof(UInt32?), (Func<string, UInt32?>)ToNUInt32);
			_map.Add(typeof(UInt64), (Func<string, UInt64>)ToUInt64);
			_map.Add(typeof(UInt64?), (Func<string, UInt64?>)ToNUInt64);
			_map.Add(typeof(Single), (Func<string, Single>)ToSingle);
			_map.Add(typeof(Single?), (Func<string, Single?>)ToNSingle);
			_map.Add(typeof(Double), (Func<string, Double>)ToDouble);
			_map.Add(typeof(Double?), (Func<string, Double?>)ToNDouble);
			_map.Add(typeof(TimeSpan), (Func<string, TimeSpan>)ToTimeSpan);
			_map.Add(typeof(TimeSpan?), (Func<string, TimeSpan?>)ToNTimeSpan);
			_map.Add(typeof(DateTime), (Func<string, DateTime>)ToDateTime);
			_map.Add(typeof(DateTime?), (Func<string, DateTime?>)ToNDateTime);
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

