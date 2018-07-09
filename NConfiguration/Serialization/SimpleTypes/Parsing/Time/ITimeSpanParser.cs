using System;
using System.Globalization;

namespace NConfiguration.Serialization.SimpleTypes.Parsing.Time
{
	interface ITimeSpanParser
	{
		TimeSpan Parse(string rawInput, CultureInfo cultureInfo);
	}
}
