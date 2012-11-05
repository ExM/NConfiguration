using System;
using Configuration.Xml.Joining;
using NUnit.Framework;
using System.Collections.Specialized;
using System.Configuration;
using Configuration.Xml;
using Configuration.Joining;
using Configuration.Examples;
using Configuration.Xml.Protected;

namespace Configuration
{
	[TestFixture]
	public class IncludeSettingsTests
	{
		[Test]
		public void Load()
		{
			var loader = new SettingsLoader();
			var includer = new IncludeInXmlLoader();

			includer.IncludeXmlElement += XmlFileSettingsLoader.ResolveXmlElement;
			loader.Loaded += includer.Include;
			loader.Loaded += (s,e) => 
			{
				Console.WriteLine("Loaded: {0} ({1})", e.Settings.GetType(), e.Settings.Identity);
			};
			loader.LoadXmlFile("Examples/AppDirectory/main.config");


			IAppSettings settings = loader.Settings;

			var addCfg = settings.TryLoad<ExampleCombineConfig>("AdditionalConfig");

			Assert.IsNotNull(addCfg);
			Assert.AreEqual("InAppDirectory", addCfg.F);
		}

		[Test]
		public void SecureLoad()
		{
			var providerLoader = new ProviderLoader();
			var loader = new SettingsLoader();
			var includer = new IncludeInXmlLoader();

			includer.IncludeXmlElement += XmlFileSettingsLoader.ResolveXmlElement;
			loader.Loaded += includer.Include;
			loader.Loaded += providerLoader.TryExtractConfigProtectedData;
			
			loader.Loaded += (s, e) =>
			{
				Console.WriteLine("Loaded: {0} ({1})", e.Settings.GetType(), e.Settings.Identity);
			};
			loader.LoadXmlFile("Examples/AppDirectory/secureMain.config");


			IAppSettings settings = loader.Settings;

			var addCfg = settings.TryLoad<ExampleCombineConfig>("AdditionalConfig");

			Assert.IsNotNull(addCfg);
			Assert.AreEqual("InUpDirectory", addCfg.F);

			var extConn = settings.TryLoad<ConnectionConfig>("MyExtConnection");

			Assert.AreEqual("Server=localhost;Database=workDb;User ID=admin;Password=pass;", settings.TryLoad<ConnectionConfig>("MyExtConnection").ConnectionString);
			Assert.AreEqual("Server=localhost;Database=workDb;User ID=admin;Password=pass;", settings.TryLoad<ConnectionConfig>("MySecuredConnection").ConnectionString);
		}

	}
}

