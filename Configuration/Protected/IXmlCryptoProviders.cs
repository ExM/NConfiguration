using System;
using System.Configuration;


namespace Configuration
{
	public interface IXmlCryptoProviders
	{
		ProtectedConfigurationProvider Get(string name);
		void Set(string name, ProtectedConfigurationProvider provider);
		void Clear();
		bool Remove(string name);
	}
}

