using System;
using NUnit.Framework;
using Configuration.ConfigSections;
using System.Linq;

namespace Configuration.Including
{
	[TestFixture]
	public class ConfigSectionTests
	{
		[Test]
		public void Empty()
		{
			var settings = @"<?xml version='1.0' encoding='utf-8' ?>
<Config>
	<Include />
</Config>".ToXmlSettings();
			
			var cfg = settings.Load<IncludeConfig>();
			
			Assert.IsNull(cfg.FilePaths);
			CollectionAssert.IsEmpty(cfg.FileConfigs);
			Assert.IsFalse(cfg.FinalSearch);
			CollectionAssert.IsEmpty(cfg.Files);
		}
		
		[Test]
		public void GetFiles()
		{
			var settings = @"<?xml version='1.0' encoding='utf-8' ?>
<Config>
	<Include Files='file1;file2;;file3;' FinalSearch='true'>
		<File Path='file4'/>
		<File Path='file5' Search='Exact' Include='All' Required='false'/>
		<File Path='file6' Search='All' Include='First' Required='true'/>
	</Include>
</Config>".ToXmlSettings();
			
			var cfg = settings.Load<IncludeConfig>();
			
			Assert.IsNotEmpty(cfg.FilePaths);
			CollectionAssert.IsNotEmpty(cfg.FileConfigs);
			Assert.IsTrue(cfg.FinalSearch);
			
			var files = cfg.Files.ToArray();
			Assert.AreEqual(6, files.Length);
			
			Assert.AreEqual("file1", files[0].Path);
			Assert.AreEqual(SearchMode.All, files[0].Search);
			Assert.AreEqual(IncludeMode.All, files[0].Include);
			Assert.IsFalse(files[0].Required);

			Assert.AreEqual("file4", files[3].Path);
			Assert.AreEqual(SearchMode.All, files[3].Search);
			Assert.AreEqual(IncludeMode.All, files[3].Include);
			Assert.IsFalse(files[3].Required);

			Assert.AreEqual("file6", files[5].Path);
			Assert.AreEqual(SearchMode.All, files[5].Search);
			Assert.AreEqual(IncludeMode.First, files[5].Include);
			Assert.IsTrue(files[5].Required);
		}
	}
}

