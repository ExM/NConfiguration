using System;
using System.Globalization;

namespace NConfiguration.Serialization.SimpleTypes.Parsing.Time
{
	public class DefaultTimeSpanParser : IParser<TimeSpan>
	{
		public bool TryParse(string rawInput, out TimeSpan result)
		{
			return TimeSpan.TryParse(rawInput, CultureInfo.InvariantCulture, out result);
		}
	}
}