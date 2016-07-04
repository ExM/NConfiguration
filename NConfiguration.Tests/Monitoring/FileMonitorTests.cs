using System;
using NUnit.Framework;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NConfiguration.Monitoring
{
	[TestFixture]
	public class FileMonitorTests
	{
		[TestCase(WatchMode.None, 1000)]
		[TestCase(WatchMode.Time, 1)]
		[TestCase(WatchMode.None, null)]
		[TestCase(WatchMode.Time, null)]
		public void WrongArguments(WatchMode mode, int? ms)
		{
			TimeSpan? ts = null;
			if (ms != null)
				ts = TimeSpan.FromMilliseconds(ms.Value);

			string file = Path.GetTempFileName();
			try
			{
				new FileMonitor(file, new byte[] { 1, 2, 3 }, mode, ts);
			}
			catch (ArgumentException)
			{
			}
		}

		[TestCase(WatchMode.System, null)]
		[TestCase(WatchMode.Auto, null)]
		[TestCase(WatchMode.Auto, 1000)]
		[TestCase(WatchMode.Time, 1000)]
		public void FileChange(WatchMode mode, int? ms)
		{
			TimeSpan? ts = null;
			if (ms != null)
				ts = TimeSpan.FromMilliseconds(ms.Value);

			string file = Path.GetTempFileName();
			File.WriteAllBytes(file, new byte[]{1, 2, 3});

			var m = new FileMonitor(file, new byte[] { 1, 2, 3 }, mode, ts);

			var wait = new ManualResetEvent(false);

			m.Changed += (a, e) =>
			{
				wait.Set();
			};

			var t = Task.Factory.StartNew(() =>
			{
				Thread.Sleep(1000);
				using (var fs = new FileStream(file, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
				{
					fs.Position = 2;
					fs.WriteByte(1);
					fs.Close();
				};
			}, TaskCreationOptions.LongRunning);

			Task.WaitAll(t);

			Assert.IsTrue(wait.WaitOne(10000), "10 sec elapsed");

			wait.Reset();

			m.Changed += (a, e) =>
			{
				wait.Set();
			};

			Assert.IsTrue(wait.WaitOne(10000), "10 sec elapsed");
		}
	}
}
