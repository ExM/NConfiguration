using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NConfiguration.Xml;
using NConfiguration.Examples;
using NConfiguration.Joining;

namespace NConfiguration.Monitoring
{
	[TestFixture]
	public class AppSettingChangebleTests
	{
		private string _xmlCfgAutoOrigin = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
	<WatchFile Mode=""Auto"" />
	<AdditionalConfig F=""Origin"" />
</configuration>";

		private string _xmlCfgAutoModify = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
	<WatchFile Mode=""Auto"" />
	<AdditionalConfig F=""Modify"" />
</configuration>";

		[Test]
		public void AutoChange()
		{
			string cfgFile = Path.GetTempFileName();
			File.WriteAllText(cfgFile, _xmlCfgAutoOrigin);

			var fileSettings = new XmlFileSettings(cfgFile);
			
			var wait = new ManualResetEvent(false);
			FileChecker.TryCreate(fileSettings).Changed += (a, e) =>
			{
				wait.Set();
			};

			var t = Task.Factory.StartNew(() =>
			{
				File.WriteAllText(cfgFile, _xmlCfgAutoModify);
			}, TaskCreationOptions.LongRunning);

			Task.WaitAll(t);

			Assert.IsTrue(wait.WaitOne(10000), "10 sec elapsed");

			var appSettings = new XmlFileSettings(cfgFile).ToAppSettings();
			Assert.That(appSettings.First<ExampleCombineConfig>("AdditionalConfig").F, Is.EqualTo("Modify"));
		}

		private string _xmlCfgMain = @"<?xml version='1.0' encoding='utf-8' ?>
<configuration>
	<WatchFile Mode='Auto' />
	<AdditionalConfig F='InMain'/>
	<IncludeXmlFile Path='{0}' Search='Exact' Include='First' Required='true'/>
</configuration>";

		[Test]
		public void IncludedChange()
		{
			string cfgMainFile = Path.GetTempFileName();
			string cfgAdditionalFile = Path.GetTempFileName();

			File.WriteAllText(cfgAdditionalFile, _xmlCfgAutoOrigin);
			File.WriteAllText(cfgMainFile, string.Format(_xmlCfgMain, cfgAdditionalFile));

			var loader = new SettingsLoader();
			loader.XmlFileBySection();
			var loadResult = loader.LoadSettings(new XmlFileSettings(cfgMainFile));

			var wait = new ManualResetEvent(false);
			var fileCheckers = FileChecker.TryCreate(loadResult.Sources).ToArray();
			new FirstChange(fileCheckers).Changed += (s, e) =>
			{
				wait.Set();
				foreach (var fileChecker in fileCheckers)
					fileChecker.Dispose();
			};

			var t = Task.Factory.StartNew(() =>
			{
				File.WriteAllText(cfgAdditionalFile, _xmlCfgAutoModify);
			}, TaskCreationOptions.LongRunning);

			Task.WaitAll(t);

			Assert.IsTrue(wait.WaitOne(10000), "10 sec elapsed");
		}
	}
}
