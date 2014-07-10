using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NConfiguration.GenericView.Deserialization;
using System.Collections.Concurrent;

namespace NConfiguration.Combination
{
	public class GenericCombiner: IGenericCombiner
	{
		private ICombineMapper _mapper;
		private readonly Func<Type, object> _creater;
		private ConcurrentDictionary<Type, object> _funcMap = new ConcurrentDictionary<Type, object>();

		public GenericCombiner(ICombineMapper mapper)
		{
			if (mapper == null)
				throw new ArgumentNullException("mapper");
			_mapper = mapper;
			_creater = CreateFunction;
		}

		/// <summary>
		/// Set custom combiner
		/// </summary>
		/// <typeparam name="T">required type</typeparam>
		/// <param name="combiner">combine function</param>
		public void SetCombiner<T>(Func<T, T, T> combiner)
		{
			_funcMap[typeof(T)] = combiner;
		}

		public T Combine<T>(T x, T y)
		{
			return ((Func<T, T, T>)GetFunction(typeof(T)))(x, y);
		}

		private object GetFunction(Type type)
		{
			return _funcMap.GetOrAdd(type, _creater);
		}

		private object CreateFunction(Type type)
		{
			return _mapper.CreateFunction(type, this);
		}
	}
}
