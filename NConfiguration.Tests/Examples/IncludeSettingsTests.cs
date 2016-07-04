using System;
using NUnit.Framework;
using NConfiguration.Xml;
using NConfiguration.Joining;
using NConfiguration.Xml.Protected;
using NConfiguration.Json;

namespace NConfiguration.Examples
{
	[TestFixture]
	public class IncludeSettingsTests
	{
		[Test]
		public void Load()
		{
			var loader = new SettingsLoader();
			loader.XmlFileBySection();

			loader.Loaded += (s,e) => 
			{
				Console.WriteLine("Loaded: {0} ({1})", e.Settings.GetType(), e.Settings.Identity);
			};

			var settings = loader
				.LoadSettings(new XmlFileSettings("Examples/AppDirectory/main.config".ResolveTestPath()))
				.ToChangeableAppSettings();

			var addCfg = settings.TryGet<ExampleCombineConfig>("AdditionalConfig");

			Assert.IsNotNull(addCfg);
			Assert.AreEqual("InAppDirectory", addCfg.F);
		}

		[Test]
		public void LoadJson()
		{
			var loader = new SettingsLoader();
			loader.XmlFileBySection();
			loader.JsonFileBySection();
			loader.JsonFileByExtension();

			loader.Loaded += (s, e) =>
			{
				Console.WriteLine("Loaded: {0} ({1})", e.Settings.GetType(), e.Settings.Identity);
			};

			var settings = loader
				.LoadSettings(new XmlFileSettings("Examples/AppDirectory/mainJson.config".ResolveTestPath()))
				.ToChangeableAppSettings();

			var addCfg = settings.TryGet<ExampleCombineConfig>("AdditionalConfig");

			Assert.IsNotNull(addCfg);
			Assert.AreEqual("InAppDirectory_json", addCfg.F);
		}

		[Test]
		public void AutoCombineLoad()
		{
			var loader = new SettingsLoader();
			loader.XmlFileBySection();
			loader.Loaded += (s, e) =>
			{
				Console.WriteLine("Loaded: {0} ({1})", e.Settings.GetType(), e.Settings.Identity);
			};

			var settings = loader
				.LoadSettings(new XmlFileSettings("Examples/AppDirectory/autoMain.config".ResolveTestPath()))
				.ToChangeableAppSettings();

			var cfg = settings.TryGet<ChildAutoCombinableConnectionConfig>();

			Assert.IsNotNull(cfg);
			Assert.AreEqual("Server=localhost;Database=workDb;User ID=admin;Password=pass;Trusted_Connection=True;Connection Timeout=60", cfg.ConnectionString);
		}

		[Test]
		public void SecureLoad()
		{
			KeyManager.Create();

			var providerLoader = new ProviderLoader();

			var loader = new SettingsLoader();
			loader.XmlFileBySection();
			loader.Loaded += providerLoader.TryExtractConfigProtectedData;
			
			loader.Loaded += (s, e) =>
			{
				Console.WriteLine("Loaded: {0} ({1})", e.Settings.GetType(), e.Settings.Identity);
			};

			var settings = loader
				.LoadSettings(new XmlFileSettings("Examples/AppDirectory/secureMain.config".ResolveTestPath()))
				.ToChangeableAppSettings();

			var addCfg = settings.TryGet<ExampleCombineConfig>("AdditionalConfig");

			Assert.IsNotNull(addCfg);
			Assert.AreEqual("InUpDirectory", addCfg.F);

			Assert.AreEqual("Server=localhost;Database=workDb;User ID=admin;Password=pass;", settings.TryGet<ConnectionConfig>("MyExtConnection").ConnectionString);
			Assert.AreEqual("Server=localhost;Database=workDb;User ID=admin;Password=pass;", settings.TryGet<ConnectionConfig>("MySecuredConnection").ConnectionString);

			KeyManager.Delete();
		}

	}
}

