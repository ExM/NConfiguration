using System;
using System.Xml.Linq;
using NConfiguration.Monitoring;

namespace NConfiguration.Xml
{
	/// <summary>
	/// settings loaded from a file
	/// </summary>
	public class XmlFileSettings : XmlSettings, IFilePathOwner, IIdentifiedSource, ILoadedFromFile
	{
		private readonly string _sectionName;

		public static XmlFileSettings Create(string fileName)
		{
			return new XmlFileSettings(fileName);
		}

		private readonly ReadedFileInfo _fileInfo;
		private XElement _root;

		/// <summary>
		/// settings loaded from a file
		/// </summary>
		/// <param name="fileName">file name</param>
		/// <param name="sectionName">if not null - read only selected section</param>
		public XmlFileSettings(string fileName, string sectionName = null)
		{
			_sectionName = sectionName;
			try
			{
				_fileInfo = ReadedFileInfo.Create(fileName,
					stream =>
					{
						_root = XDocument.Load(stream).Root;
						if (_root == null)
							throw new FormatException($"XML content not found ");
						
						if (_sectionName != null)
						{
							_root = _root.Element(XName.Get(_sectionName));
							if (_root == null)
								throw new FormatException($"section '{_sectionName}' not found ");
						}
					});
			}
			catch(SystemException ex)
			{
				throw new ApplicationException($"Unable to load file `{fileName}'", ex);
			}
		}

		/// <summary>
		/// XML root element that contains all the configuration section
		/// </summary>
		protected override XElement Root => _root;

		/// <summary>
		/// source identifier the application settings
		/// </summary>
		public virtual string Identity => this.GetIdentitySource(_fileInfo.FullName);

		/// <summary>
		/// Directory containing the configuration file
		/// </summary>
		public virtual string Path => System.IO.Path.GetDirectoryName(_fileInfo.FullName);

		public virtual ReadedFileInfo FileInfo => _fileInfo;
	}
}

