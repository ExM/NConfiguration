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
			var workflows = new[]
			{
				new [] { "create", "subscribe", "delete" },
				new [] { "create", "delete", "subscribe" },
				new [] { "delete", "create", "subscribe" },
				new [] { "create", "subscribe", "modify" },
				new [] { "create", "modify", "subscribe" },
				new [] { "modify", "create", "subscribe" },

				new [] { "create", "delete", "delay", "subscribe" },
				new [] { "create", "subscribe", "delay", "modify" },
				new [] { "create", "modify", "delay", "subscribe" },
				new [] { "create", "subscribe", "delay", "delete" },
			};

			foreach (var mode in new[] { CheckMode.All, CheckMode.Hash, CheckMode.Attr })
				foreach (var workflow in workflows)
				{
					foreach (var checkerName in new[] { "PeriodicFileChecker", "WatchedFileChecker" })
					foreach (var delay in new[] { TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(1)})
						yield return createData(checkerName, delay, mode, workflow);

					yield return createData("WatchedFileChecker", TimeSpan.FromSeconds(30), mode, workflow);
				}
		}

		private static IChangeable checkerCreatorByName(string name, ReadedFileInfo fi, TimeSpan delay, CheckMode mode)
		{
			switch (name)
			{
				case "PeriodicFileChecker":
					return new PeriodicFileChecker(fi, delay, mode);
				case "WatchedFileChecker":
					return new WatchedFileChecker(fi, delay, mode);
				default:
					throw new NotSupportedException();
			}
		}

		private static TestCaseData createData(string checkerName, TimeSpan delay, CheckMode mode, params string[] workflow)
		{
			Func<ReadedFileInfo, IChangeable> checkerCreator = fi => checkerCreatorByName(checkerName, fi, delay, mode);
			var name = string.Format("{0} Delay: {1} Mode: {2} Workflow: {3}",
				checkerName, delay, mode, string.Join("-", workflow));
			return new TestCaseData(checkerCreator, workflow).SetName(name);
		}

		[TestCaseSource(nameof(checkerCreators))]
		public void ExceptionInEventHandler(Func<ReadedFileInfo, IChangeable> checkerCreator, string[] workflow)
		{
			string file = Path.GetTempFileName();
			File.WriteAllBytes(file, new byte[] { 1, 2, 3 });
			var fileInfo = ReadedFileInfo.Create(file, _ => _.CopyTo(Stream.Null));

			IChangeable monitor = null;

			var wait = new ManualResetEvent(false);
			Exception unhandledException = null;
			UnhandledExceptionEventHandler handler = (sender, args) =>
			{
				unhandledException = (Exception)args.ExceptionObject;
				wait.Set();
				
				// prevent the runtime from terminating
				Thread.CurrentThread.IsBackground = true;
				if(args.IsTerminating)
					Thread.Sleep(-1);
			};

			AppDomain.CurrentDomain.UnhandledException += handler;
			try
			{
				foreach(var op in workflow)
					switch (op)
					{
						case "create":
							monitor = checkerCreator(fileInfo);
							break;

						case "subscribe":
							monitor.Changed += (a, e) =>
							{
								throw new Exception("test");
							};
							break;

						case "delete":
							SafeDelete(file);
							break;

						case "modify":
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
							break;

						case "delay":
							Thread.Sleep(75);
							break;

						default:
							throw new NotSupportedException();
					}

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

		private static void SafeDelete(string file)
		{
			var attempt = 10;
			while (attempt-- > 0)
			{
				try
				{
					File.Delete(file);
					return;
				}
				catch (IOException)
				{
					Thread.Sleep(100);
				}
			}

			File.Delete(file);
		}
	}
}
