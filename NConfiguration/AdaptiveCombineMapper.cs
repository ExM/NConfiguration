using NConfiguration.Combination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NConfiguration
{
	public class AdaptiveCombineMapper : ICombineMapper
	{
		private ICombineMapper _innerMapper;

		public AdaptiveCombineMapper()
			: this(new CombineMapper())
		{
		}

		public AdaptiveCombineMapper(ICombineMapper innerMapper)
		{
			_innerMapper = innerMapper;
		}

		public object CreateFunction(Type targetType, IGenericCombiner combiner)
		{
			if (typeof(ICombinable).IsAssignableFrom(targetType))
			{
				var funcType = typeof(Func<,,>).MakeGenericType(targetType, targetType, targetType);
				return Delegate.CreateDelegate(funcType, CustomCombineMI.MakeGenericMethod(targetType));
			}

			return _innerMapper.CreateFunction(targetType, combiner);
		}

		internal static readonly MethodInfo CustomCombineMI = typeof(AdaptiveCombineMapper).GetMethod("CustomCombine", BindingFlags.Static | BindingFlags.NonPublic);

		internal static T CustomCombine<T>(T x, T y) where T : ICombinable
		{
			x.Combine(y);
			return x;
		}
	}
}
