using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using Configuration.ConfigSections;
using System.Xml;

namespace Configuration.Xml.Protected
{
	public class ProviderLoader
	{
		private IProviderCollection _providers;

		public ProviderLoader()
			: this(new ProviderCollection())
		{
		}

		public ProviderLoader(IProviderCollection providers)
		{
			_providers = providers;
		}

		public IProviderCollection Providers
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
			if (OnLoading(name, type, parameters))
				return;

			Type providerType = Type.GetType(type, true);

			var provider = Activator.CreateInstance(providerType) as ProtectedConfigurationProvider;
			if (provider == null)
				throw new FormatException(string.Format("instance of type `{0}' can not be cast to ProtectedConfigurationProvider", providerType.FullName));

			provider.Initialize(name, parameters);

			_providers.Set(name, provider);
		}

		public ProviderLoader SubscribeLoading(EventHandler<ProviderLoadingEventArgs> handler)
		{
			Loading += handler;
			return this;
		}

		public ProviderLoader SubscribeClearing(EventHandler<CancelableEventArgs> handler)
		{
			Clearing += handler;
			return this;
		}

		public event EventHandler<CancelableEventArgs> Clearing;
		
		private bool OnClearing()
		{
			var copy = Clearing;
			if (copy == null)
				return false;

			var args = new CancelableEventArgs() { Canceled = false };
			copy(this, args);
			return args.Canceled;
		}

		public event EventHandler<ProviderLoadingEventArgs> Loading;

		private bool OnLoading(string name, string type, NameValueCollection parameters)
		{
			var copy = Loading;
			if (copy == null)
				return false;

			var args = new ProviderLoadingEventArgs() { Canceled = false, Name = name, Type = type, Parameters = parameters };
			copy(this, args);
			return args.Canceled;
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

		public static ProviderLoader FromAppSettings(IAppSettings settings)
		{
			return new ProviderLoader().LoadAppSettings(settings);
		}

		public ProviderLoader LoadAppSettings(IAppSettings settings)
		{
			var cfg = settings.Load<ConfigProtectedData>();
			
			foreach(XmlElement el in cfg.Providers.ChildNodes)
			{
				if(el.Name == "clear")
				{
					if (OnClearing())
						continue;

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

		public static ProviderLoader FromConfigProtectedData()
		{
			return new ProviderLoader().LoadConfigProtectedData();
		}

		public ProviderLoader LoadConfigProtectedData()
		{
			foreach (var settings in ConfigProtectedDataProviders)
				Resolve(settings.Name, settings.Type, settings.Parameters);

			return this;
		}
	}
}
