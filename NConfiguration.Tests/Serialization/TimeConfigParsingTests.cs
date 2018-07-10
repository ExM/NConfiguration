using System;
using NUnit.Framework;

namespace NConfiguration.Serialization
{
	[TestFixture]
	public class TimeConfigParsingTests
	{
		public class TimeConfig
		{
			public TimeSpan Time { get; set; }
		}

		public static object[] CaseSets => new object[]
		{
			new object[] {"15d", TimeSpan.FromDays(15) },
			new object[] {"2h", TimeSpan.FromHours(2) },
			new object[] {"2h30m", new TimeSpan(0, 2, 30, 0) },
			new object[] {"3:40:50", new TimeSpan(0, 3, 40, 50) },
		};

		[TestCaseSource(nameof(CaseSets))]
		public void WhenXmlContainsShortTextTimes_ThenTheyShouldBeParsedCorrectly(string time, TimeSpan expectedTime)
		{
			var root = $@"<Root Time='{time}' />".ToXmlView();

			var item = DefaultDeserializer.Instance.Deserialize<TimeConfig>(root);
			Assert.AreEqual(expectedTime, item.Time);
		}
	}
}
