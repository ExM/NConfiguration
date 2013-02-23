using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Collections.Concurrent;
using System.Xml.Linq;

namespace Configuration.GenericView
{
	public partial class StringConverter : IStringConverter
	{
		private readonly IStringMapper _mapper;
		private readonly Func<Type, object> _creater;
		private readonly ConcurrentDictionary<Type, object> _funcMap = new ConcurrentDictionary<Type, object>();

		public StringConverter(IStringMapper mapper)
		{
			_mapper = mapper;
			_creater = CreateFunction;
		}

		public void SetConverter<T>(Func<string, T> conv)
		{
			_funcMap[typeof(T)] = conv;
		}

		public T Convert<T>(string val)
		{
			var conv = (Func<string, T>)_funcMap.GetOrAdd(typeof(T), _creater);
			return conv(val);
		}

		private object CreateFunction(Type type)
		{
			return _mapper.CreateFunction(type);
		}
	}
}

