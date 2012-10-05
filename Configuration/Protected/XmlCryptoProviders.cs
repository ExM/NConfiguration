using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using Configuration.ConfigSections;
using System.Xml;

namespace Configuration.Protected
{
	public class XmlCryptoProviders : IXmlCryptoProviders
	{
		private Dictionary<string, ProtectedConfigurationProvider> _map = new Dictionary<string, ProtectedConfigurationProvider>();

		public XmlCryptoProviders()
		{
		}

		public void Clear()
		{
			_map.Clear();
		}

		public void SetProvider(string name, ProtectedConfigurationProvider provider)
		{
			if (_map.ContainsKey(name))
				_map[name] = provider;
			else
				_map.Add(name, provider);
		}

		public void SetProvider(NameValueCollection parameters)
		{
			string name = parameters["name"];
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name", "missing parameter");
			parameters.Remove("name");
			string type = parameters["type"];
			if(string.IsNullOrWhiteSpace(type))
				throw new ArgumentNullException("type", "missing parameter");
			parameters.Remove("type");

			ResolveProvider(name, type, parameters);
		}

		private void ResolveProvider(string name, string type, NameValueCollection parameters)
		{
			Type providerType = Type.GetType(type, true);

			var provider = Activator.CreateInstance(providerType) as ProtectedConfigurationProvider;
			if(provider == null)
				throw new FormatException(string.Format("instance of type `{0}' can not be cast to ProtectedConfigurationProvider", providerType.FullName));

			provider.Initialize(name, parameters);

			SetProvider(name, provider);
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

		public void LoadFromAppSettings(IAppSettings settings)
		{
			var cfg = settings.Load<ConfigProtectedData>();
			
			foreach(XmlElement el in cfg.Providers.ChildNodes)
			{
				if(el.Name == "clear")
				{
					Clear();
					continue;
				}

				if(el.Name == "add")
				{
					SetProvider(el.Attributes.ToNameValueCollection());
					continue;
				}

				throw new InvalidOperationException(string.Format("unexpected element `{0}'", el.Name));
			}
		}

		public void LoadFromConfigProtectedData()
		{
			foreach (var settings in ConfigProtectedDataProviders)
				ResolveProvider(settings.Name, settings.Type, settings.Parameters);
		}
	}
}
