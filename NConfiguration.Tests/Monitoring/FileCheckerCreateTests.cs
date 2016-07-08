using System;
using NUnit.Framework;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NConfiguration.Monitoring
{
	[TestFixture]
	public class FileCheckerCreateTests
	{
		[Test]
		public void WrongTimeArguments_Null()
		{
			var fileInfo = ReadedFileInfo.Create(Path.GetTempFileName(), _ => _.CopyTo(Stream.Null));

			Assert.Throws<ArgumentNullException>(() => FileChecker.TryCreate(fileInfo, WatchMode.Time, null, CheckMode.All));
		}

		[Test]
		public void WrongTimeArguments_OutOfRange()
		{
			var fileInfo = ReadedFileInfo.Create(Path.GetTempFileName(), _ => _.CopyTo(Stream.Null));

			Assert.Throws<ArgumentOutOfRangeException>(() => FileChecker.TryCreate(fileInfo, WatchMode.Time, TimeSpan.FromMilliseconds(1), CheckMode.All));
		}

		[TestCase(1000)]
		[TestCase(1)]
		[TestCase(null)]
		public void None(int? ms)
		{
			TimeSpan? ts = null;
			if (ms != null)
				ts = TimeSpan.FromMilliseconds(ms.Value);

			string file = Path.GetTempFileName();
			var fileInfo = ReadedFileInfo.Create(file, _ => _.CopyTo(Stream.Null));

			Assert.IsNull(FileChecker.TryCreate(fileInfo, WatchMode.None, ts, CheckMode.All));
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

			var fileInfo = ReadedFileInfo.Create(file, _ => _.CopyTo(Stream.Null));

			var m = FileChecker.TryCreate(fileInfo, mode, ts, CheckMode.All);

			var wait = new ManualResetEvent(false);

			m.Changed += (a, e) =>
			{
				wait.Set();
			};

			var t = Task.Factory.StartNew(() =>
			{
				Thread.Sleep(1000);
				using (var fs = new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
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
