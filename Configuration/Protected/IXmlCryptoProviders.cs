using System;
using System.Configuration;


namespace Configuration
{
	public interface IXmlCryptoProviders
	{
		ProtectedConfigurationProvider GetProvider(string name);
	}
}

