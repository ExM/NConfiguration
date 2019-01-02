using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using NConfiguration;
using NConfiguration.Joining;
using NConfiguration.Monitoring;
using NConfiguration.Xml;
using NLog;

namespace Demo
{
	class Program
	{
		static void Main(string[] args)
		{
			var path = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location).FilePath;
			
			var loader = new SettingsLoader();
			loader.XmlFileByExtension().FindingSettings += onFindingSettings;
			loader.XmlFileBySection().FindingSettings += onFindingSettings;
			loader.Loaded += (s, e) => _log.Info("Loaded: {0} ({1})", e.Settings.GetType(), e.Settings.Identity);
			
			var systemSettings = new XmlFileSettings(path, "ExtConfigure");

			systemSettings.ToAppSettings();
			
			var loaded = loader.LoadSettings(systemSettings);
			monitoringFirstFileChange(loaded.Sources);

			var appSettings = loaded.Joined.ToAppSettings();
			
			var cfg = appSettings.First<MyXmlConfig>();

			Console.WriteLine("AttrField: {0}", cfg.AttrField);
			Console.WriteLine("ElemField: {0}", cfg.ElemField);
			Console.WriteLine("Press any key...");
			Console.ReadKey();
		}
		
		private static void onFindingSettings(object sender, FindingSettingsArgs args)
		{
			var sourceIdentity = args.Source as IIdentifiedSource;
			_log.Debug("Find settings '{0}' in directory: '{1}' (customer: {2})",
				args.IncludeFile.Path,
				args.SearchPath,
				sourceIdentity == null ? "unknown" : sourceIdentity.Identity);
		}
		
		private static void monitoringFirstFileChange(IEnumerable<IIdentifiedSource> sources)
		{
			var fileCheckers = FileChecker.TryCreate(sources).ToArray();
			_log.Info("Setup monitoring for {0} files...", fileCheckers.Length);
			var fCh = new FirstChange(fileCheckers);
			fCh.Changed += (s, e) =>
			{
				foreach (var fileChecker in fileCheckers)
					fileChecker.Dispose();

				_log.Info("Config files is changed.");
			};
		}
		
		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}