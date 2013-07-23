using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Configuration.GenericView.Deserialization;
using System.Collections.Concurrent;

namespace Configuration.GenericView
{
	public class GenericDeserializer: IGenericDeserializer
	{
		private IGenericMapper _mapper = new GenericMapper();
		private ConcurrentDictionary<Type, object> _funcMap = new ConcurrentDictionary<Type, object>();

		public GenericDeserializer() : this(new GenericMapper())
		{
		}

		public GenericDeserializer(IGenericMapper mapper)
		{
			_mapper = mapper;
		}

		/// <summary>
		/// Set custom deserializer
		/// </summary>
		/// <typeparam name="T">required type</typeparam>
		/// <param name="conv">deserialize function</param>
		public void SetDeserializer<T>(Func<ICfgNode, T> conv)
		{
			_funcMap[typeof(T)] = conv;
		}

		public T Deserialize<T>(ICfgNode cfgNode)
		{
			return ((Func<ICfgNode, T>)GetFunction(typeof(T)))(cfgNode);
		}

		private object GetFunction(Type type)
		{
			return _funcMap.GetOrAdd(type, CreateFunction);
		}

		private object CreateFunction(Type type)
		{
			return _mapper.CreateFunction(type, this);
		}
	}
}
