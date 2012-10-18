using System;
using System.Xml;
using System.IO;
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
		private readonly string _fullFileName;
		/// <summary>
		/// settings loaded from a file
		/// </summary>
		/// <param name="fileName">file name</param>
		public XmlFileSettings(string fileName)
		{
			try
			{
				fileName = Path.GetFullPath(fileName);
				_fullFileName = fileName; //FIXME: normalize path
				_directory = Path.GetDirectoryName(fileName);

				using(var s = System.IO.File.OpenRead(fileName))
					Root = XDocument.Load(s).Root;
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
				return _fullFileName;
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

