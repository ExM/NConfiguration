using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Configuration
{
	public class TestBase
	{
		private Dictionary<string, IAppSettings> _nameXmlSettingsMap = new Dictionary<string, IAppSettings>();

		[TestFixtureSetUp]
		public void SetUp()
		{
			foreach(var name in new string[] { "ACfg", "ACfg_FA", "ACfg_FB", "Empty", "Config1" })
				_nameXmlSettingsMap.Add(name, new XmlFileSettings("test" + name + ".xml"));
		}

		public IAppSettings GetXmlSettings(string name)
		{
			return _nameXmlSettingsMap[name];
		}
	}
}
