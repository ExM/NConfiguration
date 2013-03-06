using System;
using System.Linq;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Configuration.Monitoring;

namespace Configuration
{
	/// <summary>
	/// Application setting extensions.
	/// </summary>
	public static class AppSettingExtensions
	{

		internal static string GetIdentitySource(this IAppSettings settings, string defaultIdentity)
		{
			var result = settings.TryFirst<string>("Identity");
			return string.IsNullOrWhiteSpace(result) ? defaultIdentity : result;
		}

		internal static FileMonitor GetMonitoring(this IAppSettings settings, string fileName, byte[] expectedContent)
		{
			var cfg = settings.TryFirst<WatchFileConfig>();
			if (cfg == null)
				return null;
			if (cfg.Mode == WatchMode.None)
				return null;

			return new FileMonitor(fileName, expectedContent, cfg.Mode, cfg.Delay);
		}

		/// <summary>
		/// Gets the name of the section in XmlRootAttribute or DataContractAttribute
		/// </summary>
		/// <returns>The section name.</returns>
		/// <typeparam name='T'>type of configuration</typeparam>
		public static string GetSectionName<T>()
			where T : class
		{
			var dataAttr = typeof(T).GetCustomAttributes(typeof(DataContractAttribute), false)
				.Select(a => a as DataContractAttribute)
				.FirstOrDefault(a => a != null);

			if (dataAttr != null)
				return dataAttr.Name;

			var xmlAttr = typeof(T).GetCustomAttributes(typeof(XmlRootAttribute), false)
				.Select(a => a as XmlRootAttribute)
				.FirstOrDefault(a => a != null);

			if (xmlAttr != null)
				return xmlAttr.ElementName;

			throw new ArgumentException("XmlRoot or DataContract attributes not set for " + typeof(T).Name);
		}
		
		/// <summary>
		/// Load the first configuration object.
		/// Resolve the section name in XmlRootAttribute or DataContractAttribute
		/// </summary>
		/// <param name='settings'>instance of application settings</param>
		/// <typeparam name='T'>type of configuration</typeparam>
		public static T First<T>(this IAppSettings settings) where T : class
		{
			return settings.First<T>(GetSectionName<T>());
		}
		
		/// <summary>
		/// Load the first configuration object by name.
		/// </summary>
		/// <param name='settings'>instance of application settings</param>
		/// <param name='sectionName'>Section name.</param>
		/// <typeparam name='T'>type of configuration</typeparam>
		public static T First<T>(this IAppSettings settings, string sectionName) where T : class
		{
			var result = settings
				.LoadCollection<T>(sectionName)
				.FirstOrDefault();
			if(result == null)
				throw new SectionNotFoundException(sectionName, typeof(T));
			return result;
		}
		
		/// <summary>
		/// Trying to load the first configuration.
		/// Resolve the section name in XmlRootAttribute or DataContractAttribute
		/// </summary>
		/// <returns>
		/// Instance of configuration or null or default instance
		/// </returns>
		/// <param name='settings'>instance of application settings</param>
		/// <param name='createDefaultInstance'>Create default instance.</param>
		/// <typeparam name='T'>type of configuration</typeparam>
		public static T TryFirst<T>(this IAppSettings settings, bool createDefaultInstance) where T : class
		{
			var result = settings
				.LoadCollection<T>(GetSectionName<T>())
				.FirstOrDefault();
			if (result == null && createDefaultInstance)
				result = Activator.CreateInstance<T>();
			return result;
		}
		
		/// <summary>
		/// Trying to load the first configuration by name.
		/// </summary>
		/// <returns>
		/// Instance of configuration or null or default instance
		/// </returns>
		/// <param name='settings'>instance of application settings</param>
		/// <param name='sectionName'>Section name.</param>
		/// <param name='createDefaultInstance'>Create default instance.</param>
		/// <typeparam name='T'>type of configuration</typeparam>
		public static T TryFirst<T>(this IAppSettings settings, string sectionName, bool createDefaultInstance) where T : class
		{
			var result = settings
				.LoadCollection<T>(sectionName)
				.FirstOrDefault();
			if(result == null && createDefaultInstance)
				result = Activator.CreateInstance<T>();
			return result;
		}

		/// <summary>
		/// Trying to load the first configuration by name.
		/// </summary>
		/// <returns>
		/// Instance of configuration or null or default instance
		/// </returns>
		/// <param name='settings'>instance of application settings</param>
		/// <param name='sectionName'>Section name.</param>
		/// <typeparam name='T'>type of configuration</typeparam>
		public static T TryFirst<T>(this IAppSettings settings, string sectionName) where T : class
		{
			return settings
				.LoadCollection<T>(sectionName)
				.FirstOrDefault();
		}

		/// <summary>
		/// Trying to load the first configuration.
		/// Resolve the section name in XmlRootAttribute or DataContractAttribute
		/// </summary>
		/// <returns>
		/// Instance of configuration or null or default instance
		/// </returns>
		/// <param name='settings'>instance of application settings</param>
		/// <typeparam name='T'>type of configuration</typeparam>
		public static T TryFirst<T>(this IAppSettings settings) where T : class
		{
			return settings
				.LoadCollection<T>(GetSectionName<T>())
				.FirstOrDefault();
		}

		public static T TryCombine<T>(this IAppSettings settings) where T : class, ICombinable
		{
			return TryCombine<T>(settings, GetSectionName<T>());
		}

		public static T TryCombine<T>(this IAppSettings settings, string sectionName) where T : class, ICombinable
		{
			T sum = null;
			foreach(var cfg in settings.LoadCollection<T>(sectionName))
			{
				if(sum == null)
					sum = cfg;
				else
					sum.Combine(cfg);
			}

			return sum;
		}

		public static T Combine<T>(this IAppSettings settings) where T : class, ICombinable
		{
			return Combine<T>(settings, GetSectionName<T>());
		}

		public static T Combine<T>(this IAppSettings settings, string sectionName) where T : class, ICombinable
		{
			T sum = null;
			foreach (var cfg in settings.LoadCollection<T>(sectionName))
			{
				if (sum == null)
					sum = cfg;
				else
					sum.Combine(cfg);
			}

			if (sum == null)
				throw new SectionNotFoundException(sectionName, typeof(T));

			return sum;
		}

		public static T Combine<T>(this IAppSettings settings, string sectionName, Func<T, T, T> combine) where T : class
		{
			T sum = settings.LoadCollection<T>(sectionName).Aggregate(null, combine);
			if (sum == null)
				throw new SectionNotFoundException(sectionName, typeof(T));
			return sum;
		}

		public static T Combine<T>(this IAppSettings settings, Func<T, T, T> combine) where T : class
		{
			return Combine<T>(settings, GetSectionName<T>(), combine);
		}

		public static T TryCombine<T>(this IAppSettings settings, string sectionName, Func<T, T, T> combine) where T : class
		{
			return settings.LoadCollection<T>(sectionName).Aggregate(null, combine);
		}

		public static T TryCombine<T>(this IAppSettings settings, Func<T, T, T> combine) where T : class
		{
			return TryCombine<T>(settings, GetSectionName<T>(), combine);
		}
	}
}

