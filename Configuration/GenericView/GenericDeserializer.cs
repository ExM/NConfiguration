using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Configuration.GenericView
{
	public class GenericDeserializer: IGenericDeserializer
	{
		private Dictionary<Type, object> _map = new Dictionary<Type, object>();

		public GenericDeserializer()
		{
		}

		public T Deserialize<T>(ICfgNode cfgNode)
		{
			return DeserializeFunction<T>()(cfgNode);
		}

		public Func<ICfgNode, T> DeserializeFunction<T>()
		{
			object func;
			if(_map.TryGetValue(typeof(T), out func))
				return (Func<ICfgNode, T>)func;

			var typedFunc = CreateFunction<T>();
			_map.Add(typeof(T), typedFunc);
			return typedFunc;
		}

		private Func<ICfgNode, T> CreateFunction<T>()
		{

			//TODO
			return null;
		}
	}
}
