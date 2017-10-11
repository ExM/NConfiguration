using System;
using System.Threading;
using System.Threading.Tasks;

namespace NConfiguration.Monitoring
{
	public class PeriodicFileChecker : FileChecker
	{
		private readonly TimeSpan _delay;
		private readonly CheckMode _checkMode;
		private readonly CancellationTokenSource _cts;

		public PeriodicFileChecker(ReadedFileInfo fileInfo, TimeSpan delay, CheckMode checkMode)
			: base(fileInfo)
		{
			if (delay <= TimeSpan.FromMilliseconds(1))
				throw new ArgumentOutOfRangeException("delay should be greater of 1 ms");

			_delay = delay;
			_checkMode = checkMode;
			_cts = new CancellationTokenSource();

#if NET40
			Task.Factory.StartNew(checkLoop, TaskCreationOptions.LongRunning).ThrowUnhandledException(MsgErrorWhileFileChecking);
#else
			Task.Run(() => checkLoop()).ThrowUnhandledException(MsgErrorWhileFileChecking);
#endif
		}

#if NET40
		private void checkLoop()
#else
		private async Task checkLoop()
#endif
		{
			do
			{
				try
				{
#if NET40
					Thread.Sleep(_delay);
#else
					await Task.Delay(_delay, _cts.Token).ConfigureAwait(false);
#endif
				}
				catch (OperationCanceledException)
				{
					return;
				}
#if NET40
				catch (ThreadAbortException)
				{
					return;
				}
			} while (!checkFile(_checkMode));
#else
			} while (!await checkFile(_checkMode).ConfigureAwait(false));
#endif

			onChanged();
		}

		public override void Dispose()
		{
			_cts.Cancel();
			base.Dispose();
		}
	}
}
