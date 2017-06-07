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
			Task.Run(() => checkLoop()).ThrowUnhandledException(MsgErrorWhileFileChecking);
		}

		private async Task checkLoop()
		{
			do
			{
				try
				{
					await Task.Delay(_delay, _cts.Token).ConfigureAwait(false);
				}
				catch (OperationCanceledException)
				{
					return;
				}
			} while (!await checkFile(_checkMode).ConfigureAwait(false));

			onChanged();
		}

		public override void Dispose()
		{
			_cts.Cancel();
			base.Dispose();
		}
	}
}
