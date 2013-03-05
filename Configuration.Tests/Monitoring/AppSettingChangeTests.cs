using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Configuration.Xml;
using Configuration.Tests;
using Configuration.Examples;

namespace Configuration.Monitoring
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

			var xmlFileLoader = new XmlFileSettingsLoader(Global.GenericDeserializer, Global.PlainConverter);

			IAppSettings settings = xmlFileLoader.LoadFile(cfgFile);
			
			var wait = new ManualResetEvent(false);
			((IChangeable)settings).Changed += (a, e) => { wait.Set(); };

			var t = Task.Factory.StartNew(() =>
			{
				File.WriteAllText(cfgFile, _xmlCfgAutoModify);
			}, TaskCreationOptions.LongRunning);

			Task.WaitAll(t);

			Assert.IsTrue(wait.WaitOne(10000), "10 sec elapsed");

			settings = xmlFileLoader.LoadFile(cfgFile);
			Assert.That(settings.First<ExampleCombineConfig>("AdditionalConfig").F, Is.EqualTo("Modify"));
		}

	}
}
