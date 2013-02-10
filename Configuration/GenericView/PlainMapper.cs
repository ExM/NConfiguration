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
		public virtual object CreateFunction(Type src, Type dst)
		{
			if (src == dst)
				return CreateNativeConverter(src);

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
	}
}

