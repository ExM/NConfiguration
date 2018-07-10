using System;
using System.Globalization;
using NConfiguration.Serialization.SimpleTypes.Parsing.Time;
using NUnit.Framework;

namespace NConfiguration.Serialization
{
	[TestFixture]
	public class TimeSpanParserTests
	{
		private TimeSpanParser _parser;
		private CultureInfo _ci;

		[SetUp]
		public void SetUp()
		{
			_ci = CultureInfo.InvariantCulture;
			_parser = new TimeSpanParser();
		}

		private static void CheckAreEquals(TimeSpan timeSpan, double days, double hours, double minutes, double seconds)
		{
			Assert.AreEqual(TimeSpan.FromDays(days)
				.Add(TimeSpan.FromHours(hours))
				.Add(TimeSpan.FromMinutes(minutes)
					.Add(TimeSpan.FromSeconds(seconds))), timeSpan);
		}

		[TestCase("13.123d", 13.123, 0, 0, 0)]
		[TestCase("13.123d6.33h53m13s", 13.123, 6.33, 53, 13)]
		[TestCase("-3d ", -3, 0, 0, 0)]
		[TestCase("3:15:03", 0, 3, 15, 03)]
		public void CorrectInputTests(string input, double days, double hours, double minutes, double seconds)
		{
			TimeSpan result = _parser.Parse(input, _ci);
			CheckAreEquals(result, days, hours, minutes, seconds);
		}

		[TestCase("533asdf")]
		[TestCase("2hack")]
		[TestCase("1,5h")]
		[TestCase("5h-5m")]
		[TestCase("5h10h")]
		[TestCase("52s6h13m5d")]
		public void WhenInputHasIncorrectFormat_ThenExceptionShouldBeThrown(string input)
		{
			Assert.Throws<FormatException>(() => _parser.Parse(input, _ci));
		}

		[TestCase("2D3H4M5S", 2, 3, 4, 5)]
		[TestCase("2D3h4M5s", 2, 3, 4, 5)]
		public void CaseInsensitivityTests(string input, double days, double hours, double minutes, double seconds)
		{
			TimeSpan result = _parser.Parse(input, _ci);
			CheckAreEquals(result, days, hours, minutes, seconds);
		}
	}
}