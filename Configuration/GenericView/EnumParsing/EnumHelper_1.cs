using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Configuration.GenericView
{
	public class EnumHelper<T> where T: struct
	{
		private static readonly Func<string, T> _parser;

		static EnumHelper()
		{
			var type = typeof(T);
			if(!type.IsEnum)
				throw new NotImplementedException("supported only Enum types");
			var underType = type.GetEnumUnderlyingType();
			
			Type parserType;
			if(underType == typeof(Byte))
				parserType = typeof(ByteEnumParser<T>);
			else if(underType == typeof(SByte))
				parserType = typeof(SByteEnumParser<T>);
			else if(underType == typeof(Int16))
				parserType = typeof(Int16EnumParser<T>);
			else if(underType == typeof(Int32))
				parserType = typeof(Int32EnumParser<T>);
			else if(underType == typeof(Int64))
				parserType = typeof(Int64EnumParser<T>);
			else if(underType == typeof(UInt16))
				parserType = typeof(UInt16EnumParser<T>);
			else if(underType == typeof(UInt32))
				parserType = typeof(UInt32EnumParser<T>);
			else if(underType == typeof(UInt64))
				parserType = typeof(UInt64EnumParser<T>);
			else
				throw new NotImplementedException("unexpected underlying type: " + underType.FullName);

			var parser = (IEnumParser<T>)Activator.CreateInstance(parserType);

			if (type.GetCustomAttributes(typeof(FlagsAttribute), true).Length != 0)
				_parser = parser.ParseFlags;
			else
				_parser = parser.ParseOne;
		}

		public static T Parse(string text)
		{
			return _parser(text);
		}
	}
}
