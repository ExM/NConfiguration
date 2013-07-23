using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace NConfiguration
{
	public class MultiSettingsBase
	{
		private Dictionary<string, IAppSettings> _nameXmlSettingsMap = new Dictionary<string, IAppSettings>();

		[TestFixtureSetUp]
		public void SetUp()
		{
			_nameXmlSettingsMap.Add("ACfg", @"<Config><ACfg /></Config>".ToXmlSettings());
			_nameXmlSettingsMap.Add("ACfg_FA", @"<Config><ACfg F='A' /></Config>".ToXmlSettings());
			_nameXmlSettingsMap.Add("ACfg_FB", @"<Config><ACfg F='B' /></Config>".ToXmlSettings());
			_nameXmlSettingsMap.Add("Empty", @"<Config></Config>".ToXmlSettings());
		}

		public IAppSettings GetXmlSettings(string name)
		{
			return _nameXmlSettingsMap[name];
		}
	}
}
