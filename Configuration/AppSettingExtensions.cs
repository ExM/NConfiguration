using System;
using System.Xml.Serialization;


namespace Configuration
{
	/// <summary>
	/// Application setting extensions.
	/// </summary>
	public static class AppSettingExtensions
	{
		/// <summary>
		/// Gets the name of the section in XmlRootAttribute.
		/// </summary>
		/// <returns>The section name.</returns>
		/// <typeparam name='T'>type of configuration</typeparam>
		public static string GetSectionName<T>()
			where T : class
		{
			object[] attrs = typeof(T).GetCustomAttributes(typeof(XmlRootAttribute), false);
			if(attrs.Length != 1)
			{
				throw new ArgumentException("XmlRoot attribute not set for " + typeof(T).Name);
			}
			XmlRootAttribute root = attrs[0] as XmlRootAttribute;
			return root.ElementName;
		}
		
		/// <summary>
		/// Load the load the configuration object.
		/// Resolve the section name in XmlRootAttribute
		/// </summary>
		/// <param name='settings'>instance of application settings</param>
		/// <typeparam name='T'>type of configuration</typeparam>
		public static T Load<T>(this IAppSettings settings) where T : class
		{
			return settings.Load<T>(GetSectionName<T>());
		}
		
		/// <summary>
		/// Load the load the configuration object.
		/// </summary>
		/// <param name='settings'>instance of application settings</param>
		/// <param name='sectionName'>Section name.</param>
		/// <typeparam name='T'>type of configuration</typeparam>
		public static T Load<T>(this IAppSettings settings, string sectionName) where T : class
		{
			var result = settings.TryLoad<T>(sectionName);
			if(result == null)
				throw new SectionNotFoundException(sectionName);
			return result;
		}
		
		/// <summary>
		/// Trying to load the configuration.
		/// Resolve the section name in XmlRootAttribute..
		/// </summary>
		/// <returns>
		/// Instance of configuration or null or default instance
		/// </returns>
		/// <param name='settings'>instance of application settings</param>
		/// <param name='createDefaultInstance'>Create default instance.</param>
		/// <typeparam name='T'>type of configuration</typeparam>
		public static T TryLoad<T>(this IAppSettings settings, bool createDefaultInstance) where T : class
		{
			return settings.TryLoad<T>(GetSectionName<T>(), createDefaultInstance);
		}
		
		/// <summary>
		/// Trying to load the configuration
		/// </summary>
		/// <returns>
		/// Instance of configuration or null or default instance
		/// </returns>
		/// <param name='settings'>instance of application settings</param>
		/// <param name='sectionName'>Section name.</param>
		/// <param name='createDefaultInstance'>Create default instance.</param>
		/// <typeparam name='T'>type of configuration</typeparam>
		public static T TryLoad<T>(this IAppSettings settings, string sectionName, bool createDefaultInstance) where T : class
		{
			var result = settings.TryLoad<T>(sectionName);
			if(result == null && createDefaultInstance)
				result = Activator.CreateInstance<T>();
			return result;
		}
	}
}

