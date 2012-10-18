using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using Configuration.Xml.Protected;

namespace Configuration.Xml
{
	/// <summary>
	/// settings loaded from a file
	/// </summary>
	public class XmlFileSettings : XmlSettings, IRelativePathOwner
	{
		private readonly string _directory;
		private readonly string _identity;
		/// <summary>
		/// settings loaded from a file
		/// </summary>
		/// <param name="fileName">file name</param>
		public XmlFileSettings(string fileName)
		{
			try
			{
				fileName = Path.GetFullPath(fileName);

				using(var s = System.IO.File.OpenRead(fileName))
					Root = XDocument.Load(s).Root;

				var idAttr = Root.Attribute("Identity");
				if (idAttr != null && !string.IsNullOrWhiteSpace(idAttr.Value))
					_identity = idAttr.Value;
				else
					_identity= fileName;
				
				_directory = Path.GetDirectoryName(fileName);
			}
			catch(SystemException ex)
			{
				throw new ApplicationException(string.Format("Unable to load file `{0}'", fileName), ex);
			}
		}
		
		public override string Identity
		{
			get
			{
				return _identity;
			}
		}

		public string RelativePath
		{
			get
			{
				return _directory;
			}
		}
	}
}

