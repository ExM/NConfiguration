using System;
using System.Globalization;
using NConfiguration.Serialization.SimpleTypes.Parsing;
using NUnit.Framework;

namespace NConfiguration.Serialization
{
    [TestFixture]
    public class TimeSpanParserTests
    {
        private TimeSpanParser _parser;
        private CultureInfo _ci;

        static readonly object[] TimeSpanTestCases =
        {
            new object[] { "-3d", TimeSpan.FromDays(-3)},
            new object[] { "3.4d", TimeSpan.FromDays(3.4)},
            new object[] { "3h30m", TimeSpan.FromHours(3.5) },
            new object[] { "24h5s", TimeSpan.FromHours(24).Add(TimeSpan.FromSeconds(5))},
            new object[] { "-72.5d8s", TimeSpan.FromDays(-72.5).Add(TimeSpan.FromSeconds(-8))},
            new object[] { "-72.5d30.4h500m8s", TimeSpan.FromDays(-72.5)
                .Add(TimeSpan.FromHours(-30.4))
                .Add(TimeSpan.FromMinutes(-500))
                .Add(TimeSpan.FromSeconds(-8))},
        };

        private static readonly object[] TimeSpanCaseInsensitiveTestCases =
        {
            new object[] { "2D3H4M5S", TimeSpan.FromDays(2)
                .Add(TimeSpan.FromHours(3))
                .Add(TimeSpan.FromMinutes(4))
                .Add(TimeSpan.FromSeconds(5))
            },
            new object[] { "2D3h4M5s", TimeSpan.FromDays(2)
                .Add(TimeSpan.FromHours(3))
                .Add(TimeSpan.FromMinutes(4))
                .Add(TimeSpan.FromSeconds(5))
            }
        };

        [SetUp]
        public void SetUp()
        {
            _ci = CultureInfo.InvariantCulture;
            _parser = new TimeSpanParser();
        }
       
        [TestCaseSource(nameof(TimeSpanTestCases))]
        public void CorrectInputTests(string input, TimeSpan expectedResult)
        {
            TimeSpan result = _parser.Parse(input, _ci);
            Assert.AreEqual(expectedResult, result);
        }

        [TestCase("-3p")]
        [TestCase("3.4d8d")]
        [TestCase("30m3h")]
        [TestCase("24h-5s")]
        [TestCase("-72.5d8.s")]
        [TestCase(".5d")]
        [TestCase("13.123d-6.33h")]
        [TestCase("13.123d6.33d")]
        public void IncorrectFormat_ThenExceptionShouldBeThrown(string input)
        {
            Assert.Throws<FormatException>(() => _parser.Parse(input, _ci));
        }

        [TestCaseSource(nameof(TimeSpanCaseInsensitiveTestCases))]
        public void CaseInsensitivityTests(string input, TimeSpan expectedResult)
        {
            TimeSpan result = _parser.Parse(input, _ci);
            Assert.AreEqual(expectedResult, result);
        }
    }
}