using System;
using System.Linq;

namespace Configuration
{
	public class CombineFactory: ICombineFactory
	{
		public CombineFactory()
		{
		}
		
		public virtual Func<T, T, T> GetCombinator<T>() where T: class
		{
			bool isGenCombinable = typeof(T).GetInterfaces().Any(x => x == typeof(ICombinable<T>));

			if (isGenCombinable)
			{
				var combinator = (ICombinable<T>)Activator.CreateInstance(typeof(T));
				return combinator.Combine;
			}

			bool isCombinable = typeof(T).GetInterfaces().Any(x => x == typeof(ICombinable));

			if (isCombinable)
			{
				var type = typeof(CustomCombinator<>).MakeGenericType(typeof(T));
				var combinator = (BaseCustomCombinator<T>)Activator.CreateInstance(type);
				return combinator.Combine;
			}
			else
			{
				var type = typeof(ForwardCombinator<>).MakeGenericType(typeof(T));
				var combinator = (ICombinable<T>)Activator.CreateInstance(type);
				return combinator.Combine;
			}
		}

		private abstract class BaseCustomCombinator<T> : ICombinable<T> where T : class
		{
			public abstract T Combine(T prev, T next);
		}

		private class CustomCombinator<T> : BaseCustomCombinator<T> where T : class, ICombinable
		{
			public override T Combine(T prev, T next)
			{
				if (prev == null)
					return next;
				prev.Combine(next);
				return prev;
			}
		}
		
		private class ForwardCombinator<T> : ICombinable<T> where T : class
		{
			public T Combine(T prev, T next)
			{
				return next ?? prev;
			}
		}
	}
}

