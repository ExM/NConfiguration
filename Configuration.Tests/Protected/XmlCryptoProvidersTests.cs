using System;
using System.Linq;
using System.Security.Cryptography;
using System.Xml;
using NUnit.Framework;
using System.Configuration;
using Configuration.Xml.Protected;
using System.Collections.Generic;

namespace Configuration
{
	[TestFixture]
	public class XmlCryptoProvidersTests
	{
		[Test]
		public void LoadProviderSettingsFromConfigProtectedData()
		{
			var providerSettings = ProviderLoader.ConfigProtectedDataProviders.ToArray();
			Assert.AreEqual(2, providerSettings.Length);
			Assert.AreEqual("RsaProvider", providerSettings[0].Name);
			Assert.AreEqual("DpapiProvider", providerSettings[1].Name);
		}

		[Test]
		public void LoadFromConfigProtectedData()
		{
			var providers = ProviderLoader.FromConfigProtectedData().Providers;
			
			Assert.IsNotNull(providers.Get("RsaProvider"));
			Assert.IsNotNull(providers.Get("DpapiProvider"));
		}


		[Test]
		public void ProviderFiltering()
		{
			var settings = @"<Config>
<configProtectedData>
	<providers>
		<add name='ClearingProvider' type='System.Configuration.RsaProtectedConfigurationProvider, System.Configuration, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a' keyContainerName='SampleKeys' useMachineContainer='true' />
		<clear/>
		<add name='RsaProvider' type='System.Configuration.RsaProtectedConfigurationProvider, System.Configuration, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a' keyContainerName='SampleKeys' useMachineContainer='true' />
		<add name='MissedProvider' type='System.Configuration.DpapiProtectedConfigurationProvider, System.Configuration, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a' />
		<add name='DpapiProvider' type='System.Configuration.DpapiProtectedConfigurationProvider, System.Configuration, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a' />
	</providers>
</configProtectedData>
</Config>".ToXmlSettings();

			var names = new List<string>();

			var providers = new ProviderLoader()
				.SubscribeLoading((s, e) =>
				{
					names.Add(e.Name);
					if (e.Name == "MissedProvider")
						e.Canceled = true;
				})
				.LoadAppSettings(settings)
				.Providers;

			CollectionAssert.AreEqual(new string[] { "ClearingProvider", "RsaProvider", "MissedProvider", "DpapiProvider" }, names);
			Assert.IsNull(providers.Get("MissedProvider"));
			Assert.IsNull(providers.Get("ClearingProvider"));
			Assert.IsNotNull(providers.Get("RsaProvider"));
			Assert.IsNotNull(providers.Get("DpapiProvider"));
		}

		void loader_ProviderLoading(object sender, ProviderLoadingEventArgs e)
		{
			throw new NotImplementedException();
		}
		
		[Test]
		public void LoadFromAppSettings()
		{
			var settings = @"<Config>
<configProtectedData defaultProvider='SampleProvider'>
	<providers>
		<clear/>
		<add name='RsaProvider'
			type='System.Configuration.RsaProtectedConfigurationProvider, System.Configuration, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
			keyContainerName='SampleKeys' useMachineContainer='true' />
		<add name='DpapiProvider' type='System.Configuration.DpapiProtectedConfigurationProvider, System.Configuration, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a' />
	</providers>
</configProtectedData>
</Config>".ToXmlSettings();

			var providers = ProviderLoader.FromAppSettings(settings).Providers;
			
			Assert.IsNotNull(providers.Get("RsaProvider"));
			Assert.IsNotNull(providers.Get("DpapiProvider"));
		}

		[Test]
		public void EncryptDecryptSection()
		{
			var settings = @"<Config>
<configProtectedData defaultProvider='SampleProvider'>
	<providers>
		<clear/>
		<add name='RsaTestProvider'
			type='System.Configuration.RsaProtectedConfigurationProvider, System.Configuration, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
			keyContainerName='MyTestKeys' useMachineContainer='true' />
	</providers>
</configProtectedData>
</Config>".ToXmlSettings();

			KeyManager.Create();

			var providers = ProviderLoader.FromAppSettings(settings).Providers;

			var provider = providers.Get("RsaTestProvider");

			var el = "<MyXmlCfg AttrField='SecureMessage'/>".ToXmlElement();
			
			var encryptedXml = provider.Encrypt(el).OuterXml;

			Assert.IsFalse(encryptedXml.Contains("SecureMessage"));
			Assert.IsTrue(encryptedXml.Contains("EncryptedData"));
			
			KeyManager.Delete();
			
			var cryptedsettings = string.Format(@"<Config><MyXmlCfg configProtectionProvider='RsaTestProvider'>{0}</MyXmlCfg></Config>", encryptedXml).ToXmlSettings(providers);

			Assert.Throws<ConfigurationErrorsException>(() => cryptedsettings.Load<MyXmlConfig>());

			KeyManager.Create();

			var cfg = cryptedsettings.Load<MyXmlConfig>();

			Assert.AreEqual("SecureMessage", cfg.AttrField);
			Assert.IsNull(cfg.ElemField);

			KeyManager.Delete();
		}
	}
}

