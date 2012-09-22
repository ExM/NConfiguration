using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Configuration
{
	/// <summary>
	/// settings loaded from a file
	/// </summary>
	public class XmlFileSettings : XmlSettings
	{
		/// <summary>
		/// settings loaded from a file
		/// </summary>
		/// <param name="fileName">file name</param>
		public XmlFileSettings(string fileName)
			: base(LoadFromFile(fileName))
		{
		}
		
		private static XDocument LoadFromFile(string fileName)
		{
			try
			{
				using(var s = System.IO.File.OpenRead(fileName))
					return XDocument.Load(s);
			}
			catch(SystemException ex)
			{
				throw new ApplicationException(string.Format("Unable to load file `{0}'", fileName), ex);
			}
		}
	}
}

