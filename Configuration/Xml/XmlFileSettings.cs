using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using Configuration.Xml.Protected;
using Configuration.GenericView;

namespace Configuration.Xml
{
	/// <summary>
	/// settings loaded from a file
	/// </summary>
	public class XmlFileSettings : XmlSettings, IFilePathOwner, IAppSettingSource
	{
		private readonly XElement _root;
		private readonly string _directory;
		private readonly string _identity;
		/// <summary>
		/// settings loaded from a file
		/// </summary>
		/// <param name="fileName">file name</param>
		/// <param name="converter"></param>
		/// <param name="deserializer">deserializer</param>
		public XmlFileSettings(string fileName, IXmlViewConverter converter, IGenericDeserializer deserializer)
			: base(converter, deserializer)
		{
			try
			{
				fileName = System.IO.Path.GetFullPath(fileName);

				using(var s = System.IO.File.OpenRead(fileName))
					_root = XDocument.Load(s).Root;

				var idAttr = Root.Attribute("Identity");
				if (idAttr != null && !string.IsNullOrWhiteSpace(idAttr.Value))
					_identity = idAttr.Value;
				else
					_identity= fileName;

				_directory = System.IO.Path.GetDirectoryName(fileName);
			}
			catch(SystemException ex)
			{
				throw new ApplicationException(string.Format("Unable to load file `{0}'", fileName), ex);
			}
		}

		protected override XElement Root
		{
			get
			{
				return _root;
			}
		}
		
		public string Identity
		{
			get
			{
				return _identity;
			}
		}

		public string Path
		{
			get
			{
				return _directory;
			}
		}
	}
}

