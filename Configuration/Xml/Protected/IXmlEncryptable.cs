using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Configuration.Xml.Protected
{
	public interface IXmlEncryptable
	{
		void SetProviderCollection(IProviderCollection collection);
	}
}
