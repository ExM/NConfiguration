using System;
using NConfiguration.Serialization.SimpleTypes.Parsing.Time;
using NUnit.Framework;

namespace NConfiguration.Serialization
{
    [TestFixture]
    public class ShortFormatTimeSpanParserTests
    {
        private readonly ShortFormatTimeSpanParser _parser;

        public ShortFormatTimeSpanParserTests()
        {
            _parser = new ShortFormatTimeSpanParser();
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
        public void CorrectInputTests(string input, double days, double hours, double minutes, double seconds)
        {
            TimeSpan result;
            var parsed = _parser.TryParse(input, out result);
            Assert.AreEqual(true, parsed);
            CheckAreEquals(result, days, hours, minutes, seconds);
        }

        [Test]
        public void WhenInputHasIncorrectFormat_ThenFalseShouldBeReturned()
        {
            TimeSpan result;
            var parsed = _parser.TryParse("533", out result);
            Assert.AreEqual(false, parsed);
        }

        [TestCase("52s6h13m5d", 5, 6, 13, 52)]
        public void OrderIndependencyTests(string input, double days, double hours, double minutes, double seconds)
        {
            TimeSpan result;
            var parsed = _parser.TryParse(input, out result);
            Assert.AreEqual(true, parsed);
            CheckAreEquals(result, days, hours, minutes, seconds);
        }

        [TestCase("2D3H4M5S", 2, 3, 4, 5)]
        [TestCase("2D3h4M5s", 2, 3, 4, 5)]
        public void CaseInsensitivityTests(string input, double days, double hours, double minutes, double seconds)
        {
            TimeSpan result;
            var parsed = _parser.TryParse(input, out result);
            Assert.AreEqual(true, parsed);
            CheckAreEquals(result, days, hours, minutes, seconds);
        }
    }
}
