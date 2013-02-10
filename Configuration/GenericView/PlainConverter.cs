using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Collections.Concurrent;
using System.Xml.Linq;

namespace Configuration.GenericView
{
	public partial class PlainConverter : IPlainConverter
	{
		private readonly IPlainMapper _mapper;
		private readonly Func<Key, object> _creater;
		private readonly ConcurrentDictionary<Key, object> _funcMap = new ConcurrentDictionary<Key, object>();

		public PlainConverter(IPlainMapper mapper)
		{
			_mapper = mapper;
			_creater = CreateFunction;
		}

		public void SetConverter<TSrc, TDst>(Func<TSrc, TDst> conv)
		{
			_funcMap[new Key(typeof(TSrc), typeof(TDst))] = conv;
		}

		public TDst Convert<TSrc, TDst>(TSrc val)
		{
			return ((Func<TSrc, TDst>)GetFunction(typeof(TSrc), typeof(TDst)))(val);
		}

		private object GetFunction(Type src, Type dst)
		{
			return _funcMap.GetOrAdd(new Key(src, dst), CreateFunction);
		}

		private object CreateFunction(Key key)
		{
			return _mapper.CreateFunction(key.Source, key.Destination);
		}

		private struct Key
		{
			public Type Source;
			public Type Destination;

			public Key(Type src, Type dst)
			{
				Source = src;
				Destination = dst;
			}
		}

		private class Comparer : IEqualityComparer<Key>
		{
			public bool Equals(Key x, Key y)
			{
				return x.Source == y.Source && x.Destination == y.Destination;
			}

			public int GetHashCode(Key obj)
			{
				return obj.Source.GetHashCode() ^ obj.Destination.GetHashCode();
			}
		}
	}
}

