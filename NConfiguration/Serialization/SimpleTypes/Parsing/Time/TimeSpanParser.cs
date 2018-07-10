using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace NConfiguration.Serialization.SimpleTypes.Parsing.Time
{
	public class TimeSpanParser : ITimeSpanParser
	{
		private const RegexOptions RegularExpressionOptions =
			RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled;
		private const string PatternBase = @"(-?\d+(?:\.\d+)?)";

		private static Regex GetExpression(string unitOfMeasurement)
		{
			return new Regex($@"{PatternBase}{unitOfMeasurement}", RegularExpressionOptions);
		}

		private static readonly Regex _checkFormatRegex = 
			new Regex($@"({PatternBase}d)?({PatternBase}h)?({PatternBase}m)?({PatternBase}s)?", RegularExpressionOptions);

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
			var result = TimeSpan.Zero;
			var hasPositive = false;
			var hasNegative = false;
			foreach (var regexToSpan in _regexToSpans)
			{
				var match = regexToSpan.Key.Match(rawInput);
				if (match.Success)
				{
					var number = double.Parse(match.Groups[1].Value, cultureInfo);
					result = result.Add(regexToSpan.Value(number));
					hasPositive |= number > 0;
					hasNegative |= number < 0;
				}
			}
			if (hasPositive ^ hasNegative)
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
