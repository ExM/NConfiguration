using System;
using System.IO;
using System.Threading;
using NUnit.Framework;

namespace NConfiguration.Monitoring
{
	[TestFixture]
	public class WatchedFileCheckerTests
	{
		private string _xmlCfg = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
	<WatchFile Mode=""Auto"" />
	<AdditionalConfig F=""Origin"" />
</configuration>";

		[TestCase]
		public void MoveDirectory()
		{
			var tempPath = Path.Combine(Path.GetTempPath(), "NConfiguration.Tests");
			var dirName = Guid.NewGuid().ToString();
			var watchedFile = Path.Combine(tempPath, dirName, "test.xml");

			string dirPath = Path.Combine(tempPath, dirName);
			string movedDirPath = dirPath + "_moved";
			Directory.CreateDirectory(dirPath);

			File.WriteAllText(watchedFile, _xmlCfg);

			var fileInfo = ReadedFileInfo.Create(watchedFile, _ => _.CopyTo(Stream.Null));
			var fileChecker = FileChecker.TryCreate(fileInfo, WatchMode.System, TimeSpan.FromMilliseconds(100), CheckMode.All);

			var wait = new ManualResetEvent(false);
			fileChecker.Changed += (a, e) =>
			{
				wait.Set();
			};

#if NET40
			moveDirectory(dirPath, movedDirPath, attempts: 5);
#else
			Directory.Move(dirPath, movedDirPath);
#endif
			Directory.CreateDirectory(dirPath); //no lock directory

			Assert.IsTrue(wait.WaitOne(5000), "no event");
		}

#if NET40
		private static void moveDirectory(string source, string target, int attempts)
		{
			for (int i = 0; i < attempts; i++)
			{
				try
				{
					Directory.Move(source, target);
					break;
				}
				catch (IOException) when (i < attempts - 1)
				{
					Thread.Sleep(200);
				}
			}
		}
#endif

		[TestCase]
		public void DeleteDirectory()
		{
			var tempPath = Path.Combine(Path.GetTempPath(), "NConfiguration.Tests");
			var dirName = Guid.NewGuid().ToString();
			var watchedFile = Path.Combine(tempPath, dirName, "test.xml");

			Directory.CreateDirectory(Path.Combine(tempPath, dirName));

			File.WriteAllText(watchedFile, _xmlCfg);

			var fileInfo = ReadedFileInfo.Create(watchedFile, _ => _.CopyTo(Stream.Null));
			var fileChecker = FileChecker.TryCreate(fileInfo, WatchMode.System, TimeSpan.FromMilliseconds(100), CheckMode.All);

			Thread.Sleep(50);

			var wait = new ManualResetEvent(false);
			fileChecker.Changed += (a, e) =>
			{
				wait.Set();
			};

			Directory.Delete(Path.Combine(tempPath, dirName), true);
			Directory.CreateDirectory(Path.Combine(tempPath, dirName)); //no lock directory

			Assert.IsTrue(wait.WaitOne(10000), "no event");
		}
	}
}
