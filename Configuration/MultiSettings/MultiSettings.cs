using System;
using System.Linq;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Configuration
{
	public class MultiSettings : IAppSettings
	{
		private LinkedList<IAppSettings> _list = new LinkedList<IAppSettings>();
		
		public void Add(IAppSettings setting)
		{
			_list.AddLast(setting);
		}
		
		public IEnumerable<T> LoadCollection<T>(string sectionName)
		{
			if (sectionName == null)
				throw new ArgumentNullException("sectionName");

			return _list.SelectMany(item => item.LoadCollection<T>(sectionName));
		}
	}
}

