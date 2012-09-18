using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Configuration
{
	public interface ICombinable<T>
	{
		T Combine(T prev, T next);
	}
}
