using System;

namespace NConfiguration.Combination
{
	public interface ICombineMapper
	{
		object CreateFunction(Type targetType, IGenericCombiner combiner);
	}
}