using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NConfiguration.Xml.Protected
{
	/// <summary>
	/// This configuration can decrypt XML section
	/// </summary>
	public interface IXmlEncryptable
	{
		/// <summary>
		/// Sets the collection providers to decrypt XML sections
		/// </summary>
		void SetProviderCollection(IProviderCollection collection);
	}
}
