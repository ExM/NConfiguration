using System;
using NUnit.Framework;
namespace Configuration
{
	[TestFixture]
	public class MultiSettingsTests
	{
		[Test]
		public void ForwardReplace()
		{
			var s = new MultiSettings(CombineFactory.Forward);
			
			s.Add(new XmlFileSettings("testMA_e.xml"));
			s.Add(new XmlFileSettings("testMA_n.xml"));
			
			
			var cfg = s.TryLoad<ReplacedConfig>("ACfg");
			Assert.IsNotNull(cfg);
			Assert.IsNull(cfg.F);
		}
		
		[Test]
		public void BackwardReplace()
		{
			var s = new MultiSettings(CombineFactory.Backward);
			
			s.Add(new XmlFileSettings("testMA_e.xml"));
			s.Add(new XmlFileSettings("testMA_n.xml"));
			
			
			var cfg = s.TryLoad<ReplacedConfig>("ACfg");
			Assert.IsNotNull(cfg);
			Assert.AreEqual("A", cfg.F);
		}
	}
}

