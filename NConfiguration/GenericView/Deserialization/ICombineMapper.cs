using System;

namespace NConfiguration.GenericView.Deserialization
{
	public interface ICombineMapper
	{
		object CreateFunction(Type targetType, IGenericCombiner combiner);
	}
}