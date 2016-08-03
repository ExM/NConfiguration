using System;
using System.IO;
using System.Threading;

namespace NConfiguration.Monitoring
{
	public class WatchedFileChecker: FileChecker
	{
		private readonly ReadedFileInfo _fileInfo;
		private readonly CheckMode _checkMode;
		private readonly TimeSpan _delay;

		public WatchedFileChecker(ReadedFileInfo fileInfo, TimeSpan? delay, CheckMode checkMode)
			:base(fileInfo)
		{
			_delay = delay.GetValueOrDefault(TimeSpan.FromSeconds(10));

			if (_delay <= TimeSpan.FromMilliseconds(1))
				throw new ArgumentOutOfRangeException("delay should be greater of 1 ms");

			_fileInfo = fileInfo;
			_checkMode = checkMode;
			_watcher = createWatch();
			checkLoop();
		}

		private AutoResetEvent _are = new AutoResetEvent(false);

		private async void checkLoop()
		{
			await _are.AsTask(_delay).ConfigureAwait(false);
			if (await checkFile(_checkMode).ConfigureAwait(false))
			{
				onChanged();
				return;
			}

			while (true)
			{
				var timeout = await _are.AsTask(_delay).ConfigureAwait(false);

				lock (_sync)
					if (_disposed)
						return;

				if (await checkFile(timeout ? CheckMode.None : _checkMode).ConfigureAwait(false))
				{
					onChanged();
					return;
				}
			}
		}

		private FileSystemWatcher createWatch()
		{
			var watcher = new FileSystemWatcher
			{
				IncludeSubdirectories = false,
				Path = Path.GetDirectoryName(_fileInfo.FullName),
				Filter = Path.GetFileName(_fileInfo.FullName),
				NotifyFilter =
					NotifyFilters.Size | NotifyFilters.DirectoryName | NotifyFilters.LastWrite | NotifyFilters.CreationTime |
					NotifyFilters.Security | NotifyFilters.Attributes | NotifyFilters.FileName
			};

			watcher.Created += watcherOnModify;
			watcher.Changed += watcherOnModify;
			watcher.Deleted += watcherOnModify;
			watcher.Renamed += watcherOnModify;
			watcher.Error += watcherError;

			watcher.EnableRaisingEvents = true;
			return watcher;
		}

		private void watcherError(object sender, ErrorEventArgs e)
		{
			bool watchCreated;
			lock (_sync)
			{
				((FileSystemWatcher)sender).Dispose();

				if (_watcher != sender)
					return;

				try
				{
					_watcher = createWatch();
					watchCreated = true;
				}
				catch (Exception)
				{
					watchCreated = false;
				}
			}

			if (watchCreated)
				_are.Set();
			else
				base.onChanged();
		}

		private void watcherOnModify(object sender, FileSystemEventArgs e)
		{
			if (_checkMode.HasFlag(CheckMode.Attr))
				onChanged();
			else
				_are.Set();
		}

		protected override void onChanged()
		{
			lock (_sync)
				stopWatch();

			base.onChanged();
		}

		private void stopWatch()
		{
			if (_watcher == null)
				return;

			_watcher.Dispose();
			_watcher = null;
		}

		public override void Dispose()
		{
			lock (_sync)
			{
				if (_disposed)
					return;

				stopWatch();
				_disposed = true;
			}
			_are.Set();
			base.Dispose();
		}

		private readonly object _sync = new object();
		private bool _disposed = false;
		private FileSystemWatcher _watcher;
	}
}
