using NUnit.Framework;
using NConfiguration.ExampleTypes;

namespace NConfiguration
{
	[TestFixture]
	public class CombinableAppSettingsTests
	{
		[Test]
		public void ImplementInterfaceCombiner()
		{
			var settings = @"<Config>
<Combinable Field1='val1' />
<Combinable Field1='val2' />
<Combinable Field1='val3' />
</Config>".ToXmlSettings().ToAppSettings();

			var connCfg = settings.Get<CustomCombinableConfig>("Combinable");

			Assert.That(connCfg.Field1, Is.EqualTo("val1val2val3"));
		}

		[Test]
		public void DefaultCombiner()
		{
			var settings = @"<Config>
<Combinable Field1='val1' />
<Combinable Field1='val2' />
<Combinable Field1='val3' />
</Config>".ToXmlSettings().ToAppSettings();


			var connCfg = settings.Get<CombinableConfig>("Combinable");

			Assert.That(connCfg.Field1, Is.EqualTo("val3"));
		}
	}
}

