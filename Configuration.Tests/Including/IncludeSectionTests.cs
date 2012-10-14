using System;
using NUnit.Framework;
using System.Linq;
using Configuration.Xml.ConfigSections;
using Configuration.Xml;
using System.Collections.Generic;
using Configuration.Joining;

namespace Configuration.Including
{
	[TestFixture]
	public class ConfigSectionTests
	{
		private static string _xmlFrm = @"<?xml version='1.0' encoding='utf-8' ?>
<Config>
	{0}
</Config>";

		[TestCase("<Include FinalSearch='true'/>", true)]
		[TestCase("<Include FinalSearch='false'/>", false)]
		[TestCase("<Include />", false)]
		[TestCase("", false)]
		public void FinalSearch(string elText, bool expected)
		{
			var settings = string.Format(_xmlFrm, elText).ToXmlSettings();
			
			var cfg = settings.TryLoad<IncludeConfig>(true);
			
			Assert.AreEqual(expected, cfg.FinalSearch);
		}
		
		
		[Test]
		public void GetFiles()
		{
			var settings = @"<?xml version='1.0' encoding='utf-8' ?>
<Config>
	<Include FinalSearch='true'>
		<XmlFile Path='file1'/>
		<XmlFile Path='file2' Search='Exact' Include='All' Required='false'/>
		<XmlFile Path='file3' Search='All' Include='First' Required='true'/>
	</Include>
</Config>".ToXmlSettings();

			var includeLoader = new IncludeInXmlLoader();

			var files = new List<IncludeFileConfig>();

			includeLoader.IncludeXmlElement += (s, e) =>
			{
				Assert.IsNull(e.Loader);
				Assert.AreEqual(e.BaseSettings, settings);
				files.Add(e.IncludeElement.Deserialize<IncludeFileConfig>());
				e.Handled = true;
			};

			includeLoader.Include(null, new LoadedEventArgs(settings));

			CollectionAssert.IsNotEmpty(files);
			
			Assert.AreEqual(3, files.Count);
			
			Assert.AreEqual("file1", files[0].Path);
			Assert.AreEqual(SearchMode.All, files[0].Search);
			Assert.AreEqual(IncludeMode.All, files[0].Include);
			Assert.IsFalse(files[0].Required);

			Assert.AreEqual("file2", files[1].Path);
			Assert.AreEqual(SearchMode.Exact, files[1].Search);
			Assert.AreEqual(IncludeMode.All, files[1].Include);
			Assert.IsFalse(files[1].Required);

			Assert.AreEqual("file3", files[2].Path);
			Assert.AreEqual(SearchMode.All, files[2].Search);
			Assert.AreEqual(IncludeMode.First, files[2].Include);
			Assert.IsTrue(files[2].Required);
		}
	}
}

