using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Configuration
{
	/// <summary>
	/// класс конфигурации приложения
	/// </summary>
	public class FileSettings : IAppSettings
	{
		private readonly string _filePath;
		private object _sync = new object();
		private XDocument _doc;

		public FileSettings(string fileName)
		{
			_filePath = Path.GetFullPath(fileName);
			using(var s = System.IO.File.OpenText(_filePath))
				_doc = XDocument.Load(s);
		}
		
		public string FullPath
		{
			get
			{
				return _filePath;
			}
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
			lock (_sync)
			{
				if(sectionName == null)
					sectionName = GetSectionName<T>();
				T result = _doc.LoadElement<T>(sectionName);
				if(result != null)
					return result;
				
				if(mode == EmptyResult.Default)
				{
					result = Activator.CreateInstance<T>();
					Save<T>(result, sectionName);
					return result;
				}
				
				if(mode == EmptyResult.Null)
					return null;

				throw new InvalidOperationException(string.Format("section `{0}' not found", sectionName));
			}
		}

		/// <summary>
		/// replace section
		/// </summary>
		public void Save<T>(T instance, string sectionName = null) where T : class
		{
			lock (_sync)
			{
				if (sectionName == null)
					sectionName = GetSectionName<T>();
				_doc.SaveElement<T>(instance, sectionName);
			}
		}

		/// <summary>
		/// сохранение файла конфигурации
		/// </summary>
		public void Save()
		{
			string content;
			lock (_sync)
				content = _doc.ToString();
			System.IO.File.WriteAllText(_filePath, content);
		}
	}
}

