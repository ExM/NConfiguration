using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Configuration
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
		
		protected T Deserialize<T>(XElement element) where T : class
		{
			if(element == null)
				return null;
			
			var rootAttr = new XmlRootAttribute();
			rootAttr.ElementName = element.Name.LocalName;
			rootAttr.Namespace = element.Name.NamespaceName;
			
			var xs = new XmlSerializer(typeof(T), rootAttr);

		
			using(XmlReader xr = element.CreateReader())
				return (T)xs.Deserialize(xr);
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
			return Deserialize<T>(_settings.GetSection(sectionName));
		}
	}
}

