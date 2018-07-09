using System;

namespace NConfiguration.Serialization.SimpleTypes.Parsing.Time
{
	public class AggregateTimeSpanParser : IParser<TimeSpan>
	{
		private readonly IParser<TimeSpan>[] _parsers;

		public AggregateTimeSpanParser(IParser<TimeSpan>[] parsers = null)
		{
			_parsers = parsers ?? new IParser<TimeSpan>[]
			{
				new DefaultTimeSpanParser(),
				new ShortFormatTimeSpanParser()
			};
		}

		public bool TryParse(string rawInput, out TimeSpan result)
		{
			result = default(TimeSpan);
			foreach (var parser in _parsers)
			{
				if (parser.TryParse(rawInput, out result))
				{
					return true;
				}
			}
			return false;
		}
	}
}
