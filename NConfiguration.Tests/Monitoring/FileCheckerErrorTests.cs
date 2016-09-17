using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace NConfiguration.Monitoring
{
	[TestFixture]
	public class FileCheckerErrorTests
	{
		private static IEnumerable checkerCreators()
		{
			yield return createData(fi => new PeriodicFileChecker(fi, TimeSpan.FromMilliseconds(100), CheckMode.All), false, "PeriodicFileChecker");
			yield return createData(fi => new PeriodicFileChecker(fi, TimeSpan.FromMilliseconds(100), CheckMode.All), true, "PeriodicFileChecker");

			yield return createData(fi => new WatchedFileChecker(fi, TimeSpan.FromMilliseconds(1000), CheckMode.All), false, "WatchedFileChecker");
			yield return createData(fi => new WatchedFileChecker(fi, TimeSpan.FromMilliseconds(1000), CheckMode.All), true, "WatchedFileChecker");

			yield return createData(fi => new WatchedFileChecker(fi, TimeSpan.FromMilliseconds(1000), CheckMode.Hash), false, "WatchedFileChecker by hash");
			yield return createData(fi => new WatchedFileChecker(fi, TimeSpan.FromMilliseconds(1000), CheckMode.Hash), true, "WatchedFileChecker by hash");
		}

		private static TestCaseData createData(Func<ReadedFileInfo, IChangeable> checkerCreator, bool deleteFile, string name)
		{
			return new TestCaseData(checkerCreator, deleteFile).SetName(name + (deleteFile ? " (delete file)": ""));
		}

		[TestCaseSource("checkerCreators")]
		public void ExceptionInEventHandler(Func<ReadedFileInfo, IChangeable> checkerCreator, bool deleteFile)
		{
			string file = Path.GetTempFileName();
			File.WriteAllBytes(file, new byte[] { 1, 2, 3 });

			var fileInfo = ReadedFileInfo.Create(file, _ => _.CopyTo(Stream.Null));
			if(deleteFile)
				File.Delete(file);
			var m = checkerCreator(fileInfo);

			m.Changed += (a, e) =>
			{
				throw new Exception("test");
			};

			var wait = new ManualResetEvent(false);
			Exception unhandledException = null;
			UnhandledExceptionEventHandler handler = (sender, args) =>
			{
				unhandledException = (Exception)args.ExceptionObject;
				wait.Set();
			};

			AppDomain.CurrentDomain.UnhandledException += handler;
			try
			{
				if (!deleteFile)
					Task.Factory.StartNew(() =>
					{
						Thread.Sleep(150);
						using (var fs = new FileStream(file, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
						{
							fs.Position = 2;
							fs.WriteByte(1);
							fs.Close();
						};
					}, TaskCreationOptions.LongRunning);

				Assert.IsTrue(wait.WaitOne(10000), "10 sec elapsed");
			}
			finally
			{
				AppDomain.CurrentDomain.UnhandledException -= handler;
			}

			Console.WriteLine(unhandledException);

			Assert.That(unhandledException.Message, Is.EqualTo("Error while file checking."));
			Assert.That(unhandledException.InnerException, Is.Not.Null);
			Assert.That(unhandledException.InnerException.Message, Is.EqualTo("test"));
		}
	}
}
