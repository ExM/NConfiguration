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

				using(var s = System.IO.File.OpenRead(fileName))
					Root = XDocument.Load(s).Root;

				_fullFileName = RealFilePath(fileName);
				_directory = Path.GetDirectoryName(_fullFileName);
			}
			catch(SystemException ex)
			{
				throw new ApplicationException(string.Format("Unable to load file `{0}'", fileName), ex);
			}
		}

		public static string RealFilePath(string userDefinedPath)
		{
			if (!Path.IsPathRooted(userDefinedPath))
				userDefinedPath = Path.GetFullPath(userDefinedPath);

			var path = Path.GetDirectoryName(userDefinedPath);
			var file = Path.GetFileName(userDefinedPath);

			var result = Directory.GetFiles(path, file, SearchOption.TopDirectoryOnly).FirstOrDefault();
			if (result == null)
				throw new FileNotFoundException("file not found", userDefinedPath);
			return result;
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

