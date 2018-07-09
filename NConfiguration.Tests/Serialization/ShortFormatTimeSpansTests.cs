using System;
using NUnit.Framework;

namespace NConfiguration.Serialization
{
	[TestFixture]
	public class ShortFormatTimeSpansTests
	{
		public class TimeConfig
		{
			public TimeSpan Time1 { get; set; }

			public TimeSpan Time2 { get; set; }

			public TimeSpan Time3 { get; set; }
		}

		public static object[] CaseSets => new object[]
		{
			new object[] {"15d", "2h", "15:00:00", TimeSpan.FromDays(15), TimeSpan.FromHours(2), TimeSpan.FromHours(15)}
		};

		[TestCaseSource(nameof(CaseSets))]
		public void WhenXmlContainsShortTextTimes_ThenTheyShouldBeParsedCorrectly(
			string time1, string time2, string time3,
			TimeSpan expectedTime1, TimeSpan expectedTime2, TimeSpan expectedTime3)
		{
			var root =
				$@"<Root Time1='{time1}' Time2='{time2}' Time3='{time3}' />".ToXmlView();

			var item = DefaultDeserializer.Instance.Deserialize<TimeConfig>(root);

			Assert.AreEqual(expectedTime1, item.Time1);
			Assert.AreEqual(expectedTime2, item.Time2);
			Assert.AreEqual(expectedTime3, item.Time3);
		}
	}
}
