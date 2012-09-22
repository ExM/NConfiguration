using System;
using NUnit.Framework;
namespace Configuration
{
	[TestFixture]
	public class MultiSettingsReplaceTests: MultiSettingsBase
	{
		[Test]
		public void ForwardReplace()
		{
			var s = new MultiSettings(CombineFactory.Forward);
			
			s.Add(GetXmlSettings("ACfg_FA"));
			s.Add(GetXmlSettings("ACfg"));
			
			
			var cfg = s.TryLoad<ReplacedConfig>("ACfg");
			Assert.IsNotNull(cfg);
			Assert.IsNull(cfg.F);
		}
		
		[Test]
		public void BackwardReplace()
		{
			var s = new MultiSettings(CombineFactory.Backward);

			s.Add(GetXmlSettings("ACfg_FA"));
			s.Add(GetXmlSettings("ACfg"));
			
			
			var cfg = s.TryLoad<ReplacedConfig>("ACfg");
			Assert.IsNotNull(cfg);
			Assert.AreEqual("A", cfg.F);
		}

		[TestCase(null, "ACfg_FA", "ACfg")]
		[TestCase("A", "ACfg", "ACfg_FA")]
		[TestCase(null, "Empty", "Empty", "ACfg")]
		[TestCase("A", "Empty", "Empty", "ACfg_FA")]
		[TestCase(null, "Empty", "ACfg", "Empty")]
		[TestCase(null, "Empty", "ACfg", "ACfg")]
		[TestCase("A", "Empty", "ACfg", "ACfg_FA")]
		[TestCase("A", "Empty", "ACfg_FA", "Empty")]
		[TestCase(null, "Empty", "ACfg_FA", "ACfg")]
		[TestCase("A", "Empty", "ACfg_FA", "ACfg_FA")]
		[TestCase(null, "ACfg", "Empty", "Empty")]
		[TestCase(null, "ACfg", "Empty", "ACfg")]
		[TestCase("A", "ACfg", "Empty", "ACfg_FA")]
		[TestCase(null, "ACfg", "ACfg", "Empty")]
		[TestCase(null, "ACfg", "ACfg", "ACfg")]
		[TestCase("A", "ACfg", "ACfg", "ACfg_FA")]
		[TestCase("A", "ACfg", "ACfg_FA", "Empty")]
		[TestCase(null, "ACfg", "ACfg_FA", "ACfg")]
		[TestCase("A", "ACfg", "ACfg_FA", "ACfg_FA")]
		[TestCase("A", "ACfg_FA", "Empty", "Empty")]
		[TestCase(null, "ACfg_FA", "Empty", "ACfg")]
		[TestCase("A", "ACfg_FA", "Empty", "ACfg_FA")]
		[TestCase(null, "ACfg_FA", "ACfg", "Empty")]
		[TestCase(null, "ACfg_FA", "ACfg", "ACfg")]
		[TestCase("A", "ACfg_FA", "ACfg", "ACfg_FA")]
		[TestCase("A", "ACfg_FA", "ACfg_FA", "Empty")]
		[TestCase(null, "ACfg_FA", "ACfg_FA", "ACfg")]
		[TestCase("A", "ACfg_FA", "ACfg_FA", "ACfg_FA")]
		public void Replace(string expected, params string[] confFiles)
		{
			var s = new MultiSettings(CombineFactory.Forward);

			foreach(var name in confFiles)
				s.Add(GetXmlSettings(name));

			var cfg = s.TryLoad<ReplacedConfig>("ACfg");
			Assert.IsNotNull(cfg);
			Assert.AreEqual(expected, cfg.F);
		}

		[Test]
		public void ReplaceNull()
		{
			var s = new MultiSettings(CombineFactory.Forward);

			for(int i =0; i<3 ; i++)
				s.Add(GetXmlSettings("Empty"));

			var cfg = s.TryLoad<ReplacedConfig>("ACfg");
			Assert.IsNull(cfg);
		}
	}
}

