using System;
using NUnit.Framework;

namespace NConfiguration.Serialization
{
	[TestFixture]
	public class ShortFormatTimeSpansTests
	{
		[Test]
		public void WhenXmlContainsShortTextTimes_ThenTheyShouldBeParsedCorrectly()
		{
			var root =
				@"<Root Time1='15d' Time2='2h' Time3='15:00:00' />".ToXmlView();

			var item = DefaultDeserializer.Instance.Deserialize<TimeConfig>(root);

			Assert.AreEqual(TimeSpan.FromDays(15), item.Time1);
			Assert.AreEqual(TimeSpan.FromHours(2), item.Time2);
			Assert.AreEqual(TimeSpan.FromHours(15), item.Time3);
		}
	}
}
