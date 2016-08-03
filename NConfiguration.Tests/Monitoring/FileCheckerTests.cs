using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NConfiguration.Monitoring
{
	[TestFixture("Periodic", true)]
	[TestFixture("Watched", true)]
	[TestFixture("Periodic", false)]
	[TestFixture("Watched", false)]
	public class FileCheckerTests
	{
		private readonly string _monitorType;
		private readonly bool _fromConfig;

		public FileCheckerTests(string monitorType, bool fromConfig)
		{
			_monitorType = monitorType;
			_fromConfig = fromConfig;
		}

		private string _xmlFormat = @"<?xml version='1.0' encoding='utf-8' ?>
<configuration>
	<WatchFile Mode='{0}' Check='{1}' Delay='0:0:0.100'/>
</configuration>";

		public IChangeable createChecker(ReadedFileInfo fileInfo, CheckMode checkMode)
		{
			if (_fromConfig)
			{
				string modeText;
				if (_monitorType == "Periodic")
					modeText = "Time";
				else if(_monitorType == "Watched")
					modeText = "System";
				else
					throw new NotImplementedException();

				string checkText = "None";
				if(checkMode.HasFlag(CheckMode.Hash))
					checkText = "Hash";
				if(checkMode.HasFlag(CheckMode.Attr))
				{
					if(checkText == "None")
						checkText = "Attr";
					else
						checkText += ",Attr";
				}

				var settings = string.Format(_xmlFormat, modeText, checkText).ToXmlSettings();

				return FileChecker.TryCreate(settings, fileInfo);
			}
			else
			{
				if (_monitorType == "Periodic")
					return new PeriodicFileChecker(fileInfo, TimeSpan.FromMilliseconds(100), checkMode);

				if (_monitorType == "Watched")
					return new WatchedFileChecker(fileInfo, TimeSpan.FromMilliseconds(100), checkMode);
			}

			throw new NotImplementedException();
		}

		[TestCase(CheckMode.All, true)]
		[TestCase(CheckMode.Hash, true)]
		[TestCase(CheckMode.Attr, true)]
		[TestCase(CheckMode.None, false)]
		public void ChangeByte(CheckMode checkMode, bool expectedEvent)
		{
			string file = Path.GetTempFileName();
			File.WriteAllBytes(file, new byte[] { 1, 2, 3 });

			var fileInfo = ReadedFileInfo.Create(file, _ => _.CopyTo(Stream.Null));
			var m = createChecker(fileInfo, checkMode);
			var wait = new ManualResetEvent(false);
			m.Changed += (a, e) =>
			{
				wait.Set();
			};

			Task.Factory.StartNew(() =>
			{
				Thread.Sleep(400);
				using (var fs = new FileStream(file, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
				{
					fs.Position = 2;
					fs.WriteByte(1);
					fs.Close();
				};
			}, TaskCreationOptions.LongRunning);

			Assert.AreEqual(expectedEvent, wait.WaitOne(700), "700 sec elapsed");
		}

		[TestCase(CheckMode.All)]
		[TestCase(CheckMode.Hash)]
		[TestCase(CheckMode.Attr)]
		[TestCase(CheckMode.None)]
		public void ChangeLength(CheckMode checkMode)
		{
			string file = Path.GetTempFileName();
			File.WriteAllBytes(file, new byte[] { 1, 2, 3 });

			var fileInfo = ReadedFileInfo.Create(file, _ => _.CopyTo(Stream.Null));
			var m = createChecker(fileInfo, checkMode);
			var wait = new ManualResetEvent(false);
			m.Changed += (a, e) =>
			{
				wait.Set();
			};

			Task.Factory.StartNew(() =>
			{
				Thread.Sleep(400);
				using (var fs = new FileStream(file, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
				{
					fs.Position = 3;
					fs.WriteByte(1);
					fs.Close();
				};
			}, TaskCreationOptions.LongRunning);

			Assert.IsTrue(wait.WaitOne(700), "700 sec elapsed");

			wait.Reset();

			m.Changed += (a, e) =>
			{
				wait.Set();
			};

			Assert.IsTrue(wait.WaitOne(100), "100 sec elapsed");
		}

		[TestCase(CheckMode.All, true)]
		[TestCase(CheckMode.Hash, false)]
		[TestCase(CheckMode.Attr, true)]
		[TestCase(CheckMode.None, false)]
		public void ChangeWriteTime(CheckMode checkMode, bool expectedEvent)
		{
			string file = Path.GetTempFileName();
			File.WriteAllBytes(file, new byte[] { 1, 2, 3 });

			var fileInfo = ReadedFileInfo.Create(file, _ => _.CopyTo(Stream.Null));
			var m = createChecker(fileInfo, checkMode);
			var wait = new ManualResetEvent(false);
			m.Changed += (a, e) =>
			{
				wait.Set();
			};

			Task.Factory.StartNew(() =>
			{
				Thread.Sleep(200);
				using (var fs = new FileStream(file, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
				{
					fs.Position = 1;
					fs.WriteByte(2);
					fs.Close();
				};
			}, TaskCreationOptions.LongRunning);

			Assert.AreEqual(expectedEvent, wait.WaitOne(500));
		}
	}
}
