using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;

namespace Configuration.Protected
{
	public class XmlCryptoProviders : IXmlCryptoProviders
	{
		private Dictionary<string, ProtectedConfigurationProvider> _map = new Dictionary<string, ProtectedConfigurationProvider>();

		public XmlCryptoProviders()
		{
		}

		public void SetProvider(string name, ProtectedConfigurationProvider provider)
		{
			if (_map.ContainsKey(name))
				_map[name] = provider;
			else
				_map.Add(name, provider);
		}

		public ProtectedConfigurationProvider GetProvider(string name)
		{
			ProtectedConfigurationProvider provider;
			if (_map.TryGetValue(name, out provider))
				return provider;
			else
				return null;
		}

		public static IEnumerable<ProviderSettings> ConfigProtectedDataProviders
		{
			get
			{
				var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
				if(config == null)
					yield break;

				var section = config.GetSection("configProtectedData") as ProtectedConfigurationSection;
				if(section == null)
					yield break;

				foreach (ProviderSettings settings in section.Providers)
					yield return settings;
			}
		}

		public static XmlCryptoProviders LoadFromConfigProtectedData()
		{
			XmlCryptoProviders result = new XmlCryptoProviders();

			foreach (var settings in ConfigProtectedDataProviders)
			{
				Type providerType = Type.GetType(settings.Type, true);

				var provider = Activator.CreateInstance(providerType) as ProtectedConfigurationProvider;
				provider.Initialize(settings.Name, settings.Parameters);

				result.SetProvider(settings.Name, provider);
			}

			return result;
		}
	}
}
