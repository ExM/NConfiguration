﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NConfiguration.Monitoring
{
	public class WatchedFileChecker : FileChecker
	{
		private readonly ReadedFileInfo _fileInfo;
		private readonly CheckMode _checkMode;
		private readonly TimeSpan _delay;

		public WatchedFileChecker(ReadedFileInfo fileInfo, TimeSpan? delay, CheckMode checkMode)
			: base(fileInfo)
		{
			_delay = delay.GetValueOrDefault(TimeSpan.FromSeconds(5 * 60));

			if (_delay <= TimeSpan.FromMilliseconds(1))
				throw new ArgumentOutOfRangeException("delay should be greater of 1 ms");

			_fileInfo = fileInfo;
			_checkMode = checkMode;
			_watcher = createWatch();
			Task.Run(() => checkLoop()).ThrowUnhandledException(MsgErrorWhileFileChecking);
		}

		private AutoResetEvent _are = new AutoResetEvent(false);

		private async Task checkLoop()
		{
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
			try
			{
				((FileSystemWatcher)sender).Dispose();

				FileSystemWatcher copy;

				lock (_sync)
				{
					if (_disposed)
						return;

					if (_watcher != sender)
						return;

					if (_watcher == null)
						return;

					copy = _watcher;
					_watcher = createWatch();
				}

				copy.EnableRaisingEvents = false;
				copy.Dispose();

				_are.Set();
			}
			catch (Exception ex)
			{
				throw new Exception(MsgErrorWhileFileChecking, ex);
			}
		}

		private void watcherOnModify(object sender, FileSystemEventArgs e)
		{
			try
			{
				if (_checkMode.HasFlag(CheckMode.Attr))
					onChanged();
				else
					_are.Set();
			}
			catch (Exception ex)
			{
				throw new Exception(MsgErrorWhileFileChecking, ex);
			}
		}

		protected override void onChanged()
		{
			FileSystemWatcher copy;

			lock (_sync)
			{
				if (_disposed)
					return;

				copy = _watcher;
				_watcher = null;
			}

			if (copy != null)
			{
				copy.EnableRaisingEvents = false;
				copy.Dispose();
			}

			base.onChanged();
		}

		public override void Dispose()
		{
			FileSystemWatcher copy;

			lock (_sync)
			{
				if (_disposed)
					return;

				copy = _watcher;
				_watcher = null;
				_disposed = true;
			}

			if (copy != null)
			{
				copy.EnableRaisingEvents = false;
				copy.Dispose();
			}

			_are.Set();
			base.Dispose();
		}

		private readonly object _sync = new object();
		private bool _disposed = false;
		private FileSystemWatcher _watcher;
	}
}
