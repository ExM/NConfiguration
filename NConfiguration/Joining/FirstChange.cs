using System;
using System.Threading;

namespace NConfiguration.Joining
{
	public sealed class FirstChange : IChangeable
	{
		private readonly object _sync = new object();
		private bool _changed = false;
		private object _firstChangedSource = null;

		public void Observe(IChangeable changable)
		{
			if(changable != null)
				changable.Changed += onInnerChangableChanged;
		}

		private void onInnerChangableChanged(object sender, EventArgs e)
		{
			EventHandler copy;
			lock (_sync)
			{
				if (_changed)
					return;

				_changed = true;
				copy = _changedHandler;
				_changedHandler = null;
				_firstChangedSource = sender;
			}

			if (copy != null)
				copy(_firstChangedSource, EventArgs.Empty);
		}

		private EventHandler _changedHandler = null;

		/// <summary>
		/// Instance changed.
		/// </summary>
		public event EventHandler Changed
		{
			add
			{
				lock (_sync)
				{
					if (_changed)
						ThreadPool.QueueUserWorkItem(asyncChangedWork, value);
					else
						_changedHandler += value;
				}
			}
			remove
			{
				lock (_sync)
				{
					if (!_changed)
						_changedHandler -= value;
				}
			}
		}

		private void asyncChangedWork(object arg)
		{
			((EventHandler)arg)(_firstChangedSource, EventArgs.Empty);
		}
	}
}

