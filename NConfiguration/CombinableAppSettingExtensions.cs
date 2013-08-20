using System;
using System.Linq;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using NConfiguration.Monitoring;

namespace NConfiguration
{
	/// <summary>
	/// Combinable application setting extensions.
	/// </summary>
	public static class CombinableAppSettingExtensions
	{

		/// <summary>
		/// Combines a collection of settings in one instance.
		/// </summary>
		/// <typeparam name="T">type of instance of configuration</typeparam>
		/// <param name="settings">instance of application settings</param>
		public static T Get<T>(this ICombinableAppSettings settings)
		{
			return settings.Get<T>(AppSettingExtensions.GetSectionName<T>());
		}

		/// <summary>
		/// Trying to combines a collection of settings in one instance
		/// </summary>
		/// <typeparam name="T">type of instance of configuration</typeparam>
		/// <param name="settings">instance of application settings</param>
		/// <returns>Instance of configuration or null.</returns>
		public static T TryGet<T>(this ICombinableAppSettings settings)
		{
			return settings.TryGet<T>(AppSettingExtensions.GetSectionName<T>());
		}
	}
}

