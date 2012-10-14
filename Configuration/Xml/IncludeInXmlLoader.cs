using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using Configuration.Joining;

namespace Configuration.Xml
{
	public class IncludeInXmlLoader
	{
		public event EventHandler<IncludeXmlElementEventArgs> IncludeXmlElement;

		private IEnumerable<IAppSettings> OnIncludeXmlElement(SettingsLoader loader, IAppSettings baseSettings, XElement element)
		{
			var copy = IncludeXmlElement;
			if (copy == null)
				throw new ApplicationException(string.Format("unexpected XML element '{0}'", element.Name));

			var args = new IncludeXmlElementEventArgs(loader, baseSettings, element);
			copy(this, args);

			if(!args.Handled)
				throw new ApplicationException(string.Format("unexpected XML element '{0}'", element.Name));

			return args.AddedSettings;
		}

		public void Include(object sender, LoadedEventArgs args)
		{
			var loader = sender as SettingsLoader;
			var rootEl = args.Settings.TryLoad<XElement>("Include", false);
			if (rootEl == null)
				return;

			var results = new List<IAppSettings>();

			foreach (var childEl in rootEl.Elements())
				results.AddRange(OnIncludeXmlElement(loader, args.Settings, childEl));

			foreach (var item in results)
				loader.LoadSettings(item);
		}
	}
}

