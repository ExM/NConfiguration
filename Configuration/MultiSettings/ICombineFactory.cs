using System;

namespace Configuration
{
	public interface ICombineFactory
	{
		Func<T, T, T> GetCombinator<T>() where T : class;
	}
}

