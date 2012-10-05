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
	public class XmlCryptoProvidersLoader
	{
		private IXmlCryptoProviders _providers;

		public XmlCryptoProvidersLoader()
			:this(new XmlCryptoProviders())
		{
		}

		public XmlCryptoProvidersLoader(IXmlCryptoProviders providers)
		{
			_providers = providers;
		}

		public IXmlCryptoProviders Providers
		{
			get { return _providers; }
		}

		public void Append(NameValueCollection parameters)
		{
			string name = parameters["name"];
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name", "missing parameter");
			parameters.Remove("name");
			string type = parameters["type"];
			if(string.IsNullOrWhiteSpace(type))
				throw new ArgumentNullException("type", "missing parameter");
			parameters.Remove("type");

			Resolve(name, type, parameters);
		}

		private void Resolve(string name, string type, NameValueCollection parameters)
		{
			Type providerType = Type.GetType(type, true);

			var provider = Activator.CreateInstance(providerType) as ProtectedConfigurationProvider;
			if(provider == null)
				throw new FormatException(string.Format("instance of type `{0}' can not be cast to ProtectedConfigurationProvider", providerType.FullName));

			provider.Initialize(name, parameters);

			_providers.Set(name, provider);
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

		public static XmlCryptoProvidersLoader FromAppSettings(IAppSettings settings)
		{
			return new XmlCryptoProvidersLoader().LoadAppSettings(settings);
		}

		public XmlCryptoProvidersLoader LoadAppSettings(IAppSettings settings)
		{
			var cfg = settings.Load<ConfigProtectedData>();
			
			foreach(XmlElement el in cfg.Providers.ChildNodes)
			{
				if(el.Name == "clear")
				{
					_providers.Clear();
					continue;
				}

				if(el.Name == "add")
				{
					Append(el.Attributes.ToNameValueCollection());
					continue;
				}

				throw new InvalidOperationException(string.Format("unexpected element `{0}'", el.Name));
			}

			return this;
		}

		public static XmlCryptoProvidersLoader FromConfigProtectedData()
		{
			return new XmlCryptoProvidersLoader().LoadConfigProtectedData();
		}

		public XmlCryptoProvidersLoader LoadConfigProtectedData()
		{
			foreach (var settings in ConfigProtectedDataProviders)
				Resolve(settings.Name, settings.Type, settings.Parameters);

			return this;
		}
	}
}
