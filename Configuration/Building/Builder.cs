
using System;
using System.Configuration;

namespace Configuration.Building
{
	public class Builder
	{
		public Builder()
		{
		}
		
		public IAppSettings Settings { get; set;}
		
		public IXmlCryptoProviders XmlCryptoProviders { get; set;}
		
		public Builder XmlFile(string fileName)
		{
			if(Settings != null)
				throw new InvalidOperationException("settings already loaded");
			
			IXmlSettings xmlSettings = new XmlFileSettings(fileName);
			if(XmlCryptoProviders != null)
				xmlSettings = new XmlCryptoWrapper(xmlSettings, XmlCryptoProviders);
			
			Settings = new SystemXmlDeserializer(xmlSettings);
			
			return this;
		}
		
		public Builder ConfigSection(string sectionName)
		{
			if(Settings != null)
				throw new InvalidOperationException("settings already loaded");
			
			IXmlSettings xmlSettings = new XmlSystemSettings(sectionName);
			if(XmlCryptoProviders != null)
				xmlSettings = new XmlCryptoWrapper(xmlSettings, XmlCryptoProviders);
			
			Settings = new SystemXmlDeserializer(xmlSettings);
			
			return this;
		}
	}
}
