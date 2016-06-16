using System;
using System.Collections.Generic;

namespace NConfiguration
{
	public class ChangeableConfigNodeProvider : DefaultConfigNodeProvider, IChangeable
	{
		private readonly IChangeable _monitor;

		public ChangeableConfigNodeProvider(IEnumerable<KeyValuePair<string, ICfgNode>> items, IChangeable monitor)
			: base(items)
		{
			_monitor = monitor;
		}

		public event EventHandler Changed
		{
			add { _monitor.Changed += value; }
			remove { _monitor.Changed -= value; }
		}
	}
}
