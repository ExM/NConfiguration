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
	public class FileSettings : IAppSettings
	{
		private XDocument _doc;

		/// <summary>
		/// settings loaded from a file
		/// </summary>
		/// <param name="fileName">file name</param>
		public FileSettings(string fileName)
		{
			using(var s = System.IO.File.OpenText(fileName))
				_doc = XDocument.Load(s);
		}
		
		public static string GetSectionName<T>()
			where T : class
		{
			object[] attrs = typeof(T).GetCustomAttributes(typeof(XmlRootAttribute), false);
			if(attrs.Length != 1)
			{
				throw new ArgumentException("XmlRoot attribute not set for " + typeof(T).Name);
			}
			XmlRootAttribute root = attrs[0] as XmlRootAttribute;
			return root.ElementName;
		}

		/// <summary>
		/// load section
		/// </summary>
		public T Load<T>(EmptyResult mode, string sectionName = null) where T : class
		{
			if(sectionName == null)
				sectionName = GetSectionName<T>();
			T result = _doc.LoadElement<T>(sectionName);
			if(result != null)
				return result;
				
			if(mode == EmptyResult.Default)
				return Activator.CreateInstance<T>();
				
			if(mode == EmptyResult.Null)
				return null;

			throw new InvalidOperationException(string.Format("section `{0}' not found", sectionName));
		}
	}
}

