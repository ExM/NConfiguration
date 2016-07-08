using System.IO;
using System.Threading;

namespace NConfiguration.Monitoring
{
	public class WatchedFileChecker: FileChecker
	{
		private readonly ReadedFileInfo _fileInfo;
		private readonly CheckMode _checkMode;

		public WatchedFileChecker(ReadedFileInfo fileInfo, CheckMode checkMode)
			:base(fileInfo, checkMode)
		{
			_fileInfo = fileInfo;
			_checkMode = checkMode;
			_watcher = createWatch();
			checkLoop();
		}

		private AutoResetEvent _are = new AutoResetEvent(true);

		private async void checkLoop()
		{
			do
			{
				await _are.AsTask().ConfigureAwait(false);
				lock(_sync)
					if (_disposed)
						return;
			} while (!await checkFile().ConfigureAwait(false));

			onChanged();
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
					NotifyFilters.Security | NotifyFilters.Attributes
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
			lock (_sync)
			{
				if (_watcher != sender)
					return;

				stopWatch();
				_watcher = createWatch();
			}

			_are.Set();
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

			_watcher.EnableRaisingEvents = false;
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
