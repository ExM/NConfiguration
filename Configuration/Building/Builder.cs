
using System;
using System.Configuration;
using Configuration.Xml.Protected;

namespace Configuration.Building
{
	public class Builder
	{
		public Builder()
		{
		}
		
		public IAppSettings Settings { get; set;}
		
		public IProviderCollection XmlCryptoProviders { get; set;}
		
		public Builder XmlFile(string fileName)
		{
			if(Settings != null)
				throw new InvalidOperationException("settings already loaded");
			
			IXmlSettings xmlSettings = new XmlFileSettings(fileName);
			if(XmlCryptoProviders != null)
				xmlSettings = new Wrapper(xmlSettings, XmlCryptoProviders);
			
			Settings = new SystemXmlDeserializer(xmlSettings);
			
			return this;
		}
		
		public Builder ConfigSection(string sectionName)
		{
			if(Settings != null)
				throw new InvalidOperationException("settings already loaded");
			
			IXmlSettings xmlSettings = new XmlSystemSettings(sectionName);
			if(XmlCryptoProviders != null)
				xmlSettings = new Wrapper(xmlSettings, XmlCryptoProviders);
			
			Settings = new SystemXmlDeserializer(xmlSettings);
			
			return this;
		}
	}
}
