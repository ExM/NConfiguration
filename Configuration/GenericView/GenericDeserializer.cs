using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Configuration.GenericView.Deserialization;

namespace Configuration.GenericView
{
	public class GenericDeserializer: IGenericDeserializer
	{
		private IGenericMapper _mapper = new GenericMapper();
		private Dictionary<Type, object> _funcMap = new Dictionary<Type, object>();

		public GenericDeserializer() : this(new GenericMapper())
		{
		}

		public GenericDeserializer(IGenericMapper mapper)
		{
			_mapper = mapper;
		}

		public T Deserialize<T>(ICfgNode cfgNode)
		{
			return GetFunction<T>()(cfgNode);
		}

		private Func<ICfgNode, T> GetFunction<T>()
		{
			//TODO: tread safe
			object func;
			if (!_funcMap.TryGetValue(typeof(T), out func))
			{
				func = _mapper.CreateFunction(typeof (T), this);
				_funcMap.Add(typeof (T), func);
			}

			return (Func<ICfgNode, T>)func;
		}
	}
}
