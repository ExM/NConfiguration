using System;
using System.Linq;

namespace Configuration
{
	public class CombineFactory: ICombineFactory
	{
		private readonly bool _isForward;
		
		public CombineFactory(bool isForward)
		{
			_isForward = isForward;
		}
		
		public static ICombineFactory Forward
		{
			get { return new CombineFactory(true); }
		}
		
		public static ICombineFactory Backward
		{
			get { return new CombineFactory(false); }
		}
		
		public virtual Func<T, T, T> GetCombinator<T>() where T: class
		{
			bool isCombinable = typeof(T).GetInterfaces().Any(x =>
				x.IsGenericType &&
				x.GetGenericTypeDefinition() == typeof(ICombinable<>)
			);

			if(isCombinable)
			{
				var combinator = (ICombinable<T>)Activator.CreateInstance(typeof(T));
				return combinator.Combine;
			}
			else
			{
				var combType = _isForward ? typeof(ForwardCombinator<>) : typeof(BackwardCombinator<>);
				var combinator = (ICombinable<T>)Activator.CreateInstance(combType.MakeGenericType(typeof(T)));
				return combinator.Combine;
			}
		}
		
		private class BackwardCombinator<T> : ICombinable<T> where T : class
		{
			public T Combine(T prev, T next)
			{
				return prev ?? next;
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

