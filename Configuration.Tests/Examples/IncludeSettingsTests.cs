using System;
using Configuration.Tests;
using NUnit.Framework;
using System.Collections.Specialized;
using System.Configuration;
using Configuration.Xml;
using Configuration.Joining;
using Configuration.Examples;
using Configuration.Xml.Protected;
using Configuration.GenericView;
using Configuration.Json;

namespace Configuration.Examples
{
	[TestFixture]
	public class IncludeSettingsTests
	{
		[Test]
		public void Load()
		{
			var loader = new SettingsLoader();
			var xmlFileLoader = new XmlFileSettingsLoader(Global.GenericDeserializer, Global.PlainConverter);

			loader.Including += xmlFileLoader.ResolveFile;
			loader.Loaded += (s,e) => 
			{
				Console.WriteLine("Loaded: {0} ({1})", e.Settings.GetType(), e.Settings.Identity);
			};

			loader.LoadSettings(xmlFileLoader.LoadFile("Examples/AppDirectory/main.config"));

			IAppSettings settings = loader.Settings;

			var addCfg = settings.TryCombine<ExampleCombineConfig>("AdditionalConfig");

			Assert.IsNotNull(addCfg);
			Assert.AreEqual("InAppDirectory", addCfg.F);
		}

		[Test]
		public void LoadJson()
		{
			var loader = new SettingsLoader();
			var xmlFileLoader = new XmlFileSettingsLoader(Global.GenericDeserializer, Global.PlainConverter);
			var jsonFileLoader = new JsonFileSettingsLoader(Global.GenericDeserializer, Global.PlainConverter);

			loader.Including += xmlFileLoader.ResolveFile;
			loader.Including += jsonFileLoader.ResolveFile;
			loader.Loaded += (s, e) =>
			{
				Console.WriteLine("Loaded: {0} ({1})", e.Settings.GetType(), e.Settings.Identity);
			};

			loader.LoadSettings(xmlFileLoader.LoadFile("Examples/AppDirectory/mainJson.config"));

			IAppSettings settings = loader.Settings;

			var addCfg = settings.TryCombine<ExampleCombineConfig>("AdditionalConfig");

			Assert.IsNotNull(addCfg);
			Assert.AreEqual("InAppDirectory_json", addCfg.F);
		}

		[Test]
		public void SecureLoad()
		{
			KeyManager.Create();

			var providerLoader = new ProviderLoader();
			var loader = new SettingsLoader();
			var xmlFileLoader = new XmlFileSettingsLoader(Global.GenericDeserializer, Global.PlainConverter);

			loader.Including += xmlFileLoader.ResolveFile;
			loader.Loaded += providerLoader.TryExtractConfigProtectedData;
			
			loader.Loaded += (s, e) =>
			{
				Console.WriteLine("Loaded: {0} ({1})", e.Settings.GetType(), e.Settings.Identity);
			};

			loader.LoadSettings(xmlFileLoader.LoadFile("Examples/AppDirectory/secureMain.config"));

			IAppSettings settings = loader.Settings;

			var addCfg = settings.TryCombine<ExampleCombineConfig>("AdditionalConfig");

			Assert.IsNotNull(addCfg);
			Assert.AreEqual("InUpDirectory", addCfg.F);

			Assert.AreEqual("Server=localhost;Database=workDb;User ID=admin;Password=pass;", settings.TryFirst<ConnectionConfig>("MyExtConnection").ConnectionString);
			Assert.AreEqual("Server=localhost;Database=workDb;User ID=admin;Password=pass;", settings.TryFirst<ConnectionConfig>("MySecuredConnection").ConnectionString);

			KeyManager.Delete();
		}

	}
}

