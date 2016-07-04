using System;
using System.IO;
using System.Threading;
using NConfiguration.Serialization;

namespace NConfiguration.Monitoring
{
	public sealed class FileMonitor
	{
		internal static readonly string ConfigSectionName = "WatchFile";

		public static FileMonitor TryCreate(IConfigNodeProvider nodeProvider, string fileName, byte[] expectedContent)
		{
			foreach (var node in nodeProvider.ByName(ConfigSectionName))
			{
				var cfg = DefaultDeserializer.Instance.Deserialize<WatchFileConfig>(node);
				if (cfg.Mode == WatchMode.None)
					return null;

				return new FileMonitor(fileName, expectedContent, cfg.Mode, cfg.Delay);
			}

			return null;
		}

		private readonly string _fileName;
		private readonly byte[] _content;

		private object _sync = new object();
		private bool _changed = false;
		private FileSystemWatcher _watcher = null;

		private int _delay;
		private Timer _timer = null;

		public FileMonitor(string fileName, byte[] expectedContent, WatchMode mode, TimeSpan? delay)
		{
			_fileName = fileName;
			_content = expectedContent;

			lock (_sync)
			{
				if (mode == WatchMode.Time)
				{
					if (delay == null)
						throw new ArgumentNullException("delay time is null");

					setTimer(delay.Value);
				}
				else if (mode == WatchMode.Auto)
				{
					_watcher = tryCreateWatch(!delay.HasValue);
					if (_watcher == null)
						setTimer(delay.Value);
				}
				else if (mode == WatchMode.System)
				{
					_watcher = tryCreateWatch(true);
				}
				else
					throw new ArgumentOutOfRangeException("unexpected mode");
			}

			asyncReview();
		}

		private void setTimer(TimeSpan delay)
		{
			_delay = (int)delay.TotalMilliseconds;
			if (_delay < 100 || _delay > 24 * 60 * 60 * 1000)
				throw new ArgumentOutOfRangeException("delay must be greater than 100 ms and less than 1 day");

			_timer = new Timer(onReviewTime, null, _delay, -1);
		}

		private void waitToNextReview()
		{
			lock (_sync)
			{
				if (_changed)
					return;

				if (_timer != null)
				{
					_timer.Dispose();
					_timer = new Timer(onReviewTime, null, _delay, -1);
				}
			}
		}

		private void onReviewTime(object arg)
		{
			lock (_sync)
			{
				if (_changed)
					return;
			}

			asyncReview();
		}

		private FileSystemWatcher tryCreateWatch(bool throwException)
		{
			try
			{
				var watcher = new FileSystemWatcher();
				watcher.Path = Path.GetDirectoryName(_fileName);
				watcher.Filter = Path.GetFileName(_fileName);
				watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.Size | NotifyFilters.Security | NotifyFilters.Attributes;


				watcher.Created += watcherOnModify;
				watcher.Changed += watcherOnModify;
				watcher.Deleted += watcherOnModify;
				watcher.Renamed += watcherOnModify;
				watcher.Error += watcherError;

				watcher.EnableRaisingEvents = true;

				return watcher;
			}
			catch(Exception)
			{
				if (throwException)
					throw;
				else
					return null;
			}
		}

		private void watcherError(object sender, ErrorEventArgs e)
		{
			lock (_sync)
			{
				if (_changed)
					return;

				if (_watcher != sender)
					return;

				_watcher = tryCreateWatch(true);
			}

			asyncReview();
		}

		private void asyncReview()
		{
			AsyncComparer.Compare(_fileName, _content, rewiewResult);
		}

		private void rewiewResult(bool equal)
		{
			if(equal)
				waitToNextReview();
			else
				onChanged();
		}

		private void watcherOnModify(object sender, FileSystemEventArgs e)
		{
			onChanged();
		}

		private void onChanged()
		{
			EventHandler copy;
			lock (_sync)
			{
				if(_changed)
					return;

				_changed = true;
				copy = _changedHandler;
				_changedHandler = null;

				if (_watcher != null)
				{
					_watcher.Dispose();
					_watcher = null;
				}

				if (_timer != null)
				{
					_timer.Dispose();
					_timer = null;
				}
			}

			if (copy != null)
				copy(this, EventArgs.Empty);
		}

		private EventHandler _changedHandler = null;

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
			((EventHandler)arg)(this, EventArgs.Empty);
		}
	}
}
