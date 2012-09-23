using System;
using System.Linq;
using NUnit.Framework;
using Configuration.Protected;
using System.Configuration;
namespace Configuration
{
	[TestFixture]
	public class XmlCryptoProvidersTests
	{
		[Test]
		public void LoadProviderSettingsFromConfigProtectedData()
		{
			var providerSettings = XmlCryptoProviders.ConfigProtectedDataProviders.ToArray();
			Assert.AreEqual(2, providerSettings.Length);
			Assert.AreEqual("RsaProvider", providerSettings[0].Name);
			Assert.AreEqual("DpapiProvider", providerSettings[1].Name);
		}

		[Test]
		public void LoadFromConfigProtectedData()
		{
			var providers = XmlCryptoProviders.LoadFromConfigProtectedData();
			
			Assert.IsNotNull(providers.GetProvider("RsaProvider"));
			Assert.IsNotNull(providers.GetProvider("DpapiProvider"));
		}
	}
}

