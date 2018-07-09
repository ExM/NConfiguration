using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace NConfiguration.Serialization.SimpleTypes.Parsing.Time
{
	public class TimeSpanParser : ITimeSpanParser
	{
		private const string PatternBase = @"(-?\d+(?:\.\d+)?)";

		private static Regex GetExpression(string unitOfMeasurement)
		{
			return new Regex($@"{PatternBase}{unitOfMeasurement}", RegexOptions.IgnoreCase);
		}

		private static readonly Regex _checkFormatRegex = new Regex($@"({PatternBase}[dhms])+", RegexOptions.IgnoreCase);

		private readonly KeyValuePair<Regex, Func<double, TimeSpan>>[] _regexToSpans =
		{
			new KeyValuePair<Regex, Func<double, TimeSpan>>(GetExpression("d"), TimeSpan.FromDays),
			new KeyValuePair<Regex, Func<double, TimeSpan>>(GetExpression("h"), TimeSpan.FromHours),
			new KeyValuePair<Regex, Func<double, TimeSpan>>(GetExpression("m"), TimeSpan.FromMinutes),
			new KeyValuePair<Regex, Func<double, TimeSpan>>(GetExpression("s"), TimeSpan.FromSeconds)
		};

		public TimeSpan Parse(string rawInput, CultureInfo cultureInfo)
		{
			TimeSpan result;
			if (TimeSpan.TryParse(rawInput, cultureInfo, out result))
			{
				return result;
			}
			return ParseShortFormat(rawInput, cultureInfo);
		}

		private TimeSpan ParseShortFormat(string rawInput, CultureInfo cultureInfo)
		{
			ValidateShortFormat(rawInput);
			bool success = false;
			var result = TimeSpan.Zero;
			foreach (var regexToSpan in _regexToSpans)
			{
				var match = regexToSpan.Key.Match(rawInput);
				if (match.Success)
				{
					success = true;
					result = result.Add(regexToSpan.Value(double.Parse(match.Groups[1].Value, cultureInfo)));
				}
			}
			if (success)
			{
				return result;
			}
			throw new FormatException();
		}

		private static void ValidateShortFormat(string rawInput)
		{
			var match = _checkFormatRegex.Match(rawInput);
			if (!match.Success || match.Value != rawInput.Trim())
			{
				throw new FormatException();
			}
		}
	}
}
