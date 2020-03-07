using System;
using System.Configuration;
using System.Reflection;
using System.Xml.Linq;

namespace NConfiguration.Xml
{
	public sealed class XmlSystemSettings : XmlFileSettings
	{
		private readonly string _sectionName;

		public XmlSystemSettings(string configPath, string sectionName)
			: base(configPath)
		{
			_sectionName = sectionName;
		}

		/// <summary>
		/// XML root element that contains all the configuration section
		/// </summary>
		protected override XElement Root
		{
			get
			{
				var root = base.Root.Element(XName.Get(_sectionName));
				if (root == null)
					throw new FormatException($"section '{_sectionName}' not found ");

				return root;
			}
		}
	}
}
