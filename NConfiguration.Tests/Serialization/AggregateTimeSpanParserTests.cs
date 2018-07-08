using System;
using System.Linq;
using Moq;
using NConfiguration.Extensions;
using NConfiguration.Serialization.SimpleTypes.Parsing;
using NConfiguration.Serialization.SimpleTypes.Parsing.Time;
using NUnit.Framework;

namespace NConfiguration.Serialization
{
    [TestFixture]
    public class AggregateTimeSpanParserTests
    {
        private AggregateTimeSpanParser _parser;
        private Mock<IParser<TimeSpan>>[] _parserMocks;

        [SetUp]
        public void SetUp()
        {
            _parserMocks = new []
            {
                new Mock<IParser<TimeSpan>>(),
                new Mock<IParser<TimeSpan>>(),
                new Mock<IParser<TimeSpan>>(),
            };
            _parser = new AggregateTimeSpanParser(_parserMocks.Select(m => m.Object).ToArray());
        }

        [Test]
        public void WhenNoneOfParsersSucceeded_ThenShouldReturnFalse()
        {
            const string input = "5d";
            TimeSpan result;
            foreach (var parserMock in _parserMocks)
            {
                parserMock.Setup(p => p.TryParse(input, out result)).Returns(false);
            }

            var parsed = _parser.TryParse(input, out result);
            Assert.AreEqual(false, parsed);
        }

        [Test]
        public void WhenOneOfParsersSucceeded_ThenTheNextOnesShouldNotBeCalled()
        {
            const string input = "5d";
            TimeSpan result;
            _parserMocks[0].Setup(p => p.TryParse(input, out result)).Returns(true);
            for (var i = 1; i < _parserMocks.Length; i++)
            {
                _parserMocks[i].Setup(p => p.TryParse(input, out result)).Verifiable();
            }
            _parser.TryParse(input, out result);
            _parserMocks[0].Verify(p => p.TryParse(input, out result), Times.Once);
            for (var i = 1; i < _parserMocks.Length; i++)
            {
                _parserMocks[i].Verify(p => p.TryParse(input, out result), Times.Never);
            }
        }

        [Test]
        public void WhenParserSucceeded_ThenItsResultShouldBePassedByAggregateParser()
        {
            const string input = "5d";
            TimeSpan expectedResult = TimeSpan.FromDays(5);
            TimeSpan result;
            _parserMocks[0].Setup(p => p.TryParse(input, out result))
                .OutCallback((string i, out TimeSpan r) => r = expectedResult).Returns(true);
            var parsed = _parser.TryParse(input, out result);
            Assert.AreEqual(true, parsed);
            Assert.AreEqual(expectedResult, result);
        }
    }
}
