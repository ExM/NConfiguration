using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Configuration.Xml
{
	public class SystemXmlDeserializer : IAppSettings
	{
		private IXmlSettings _settings;

		public SystemXmlDeserializer(IXmlSettings settings)
		{
			if(settings == null)
				throw new ArgumentNullException("settings");
			_settings = settings;
		}
		
		/// <summary>
		/// Trying to load the configuration.
		/// </summary>
		/// <returns>
		/// Instance of the configuration, or null if no section name
		/// </returns>
		/// <param name='sectionName'>instance of application settings</param>
		/// <typeparam name='T'>type of configuration</typeparam>
		public T TryLoad<T>(string sectionName) where T : class
		{
			var section = _settings.GetSection(sectionName);
			if(section == null)
				return null;

			if (typeof(T) == typeof(XElement))
				return (T)(object)XElement.Parse(section.ToString());

			return section.Deserialize<T>();
		}
	}
}

