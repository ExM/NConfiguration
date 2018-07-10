using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace NConfiguration.Serialization.SimpleTypes.Parsing
{
	public class TimeSpanParser
	{
		private const RegexOptions RegularExpressionOptions =
			RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled;
		private const string PatternBase = @"(-?\d+(?:\.\d+)?)";

		private static readonly KeyValuePair<int, Func<double, TimeSpan>>[] _groupNumberToSpans =
		{
			new KeyValuePair<int, Func<double, TimeSpan>>(2, TimeSpan.FromDays),
			new KeyValuePair<int, Func<double, TimeSpan>>(4, TimeSpan.FromHours),
			new KeyValuePair<int, Func<double, TimeSpan>>(6, TimeSpan.FromMinutes),
			new KeyValuePair<int, Func<double, TimeSpan>>(8, TimeSpan.FromSeconds)
		};

		private static readonly Regex _checkFormatRegex = 
			new Regex($@"({PatternBase}d)?({PatternBase}h)?({PatternBase}m)?({PatternBase}s)?", RegularExpressionOptions);

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
			var match = _checkFormatRegex.Match(rawInput);
			if (!match.Success || match.Value != rawInput.Trim())
			{
				throw new FormatException();
			}

			var result = TimeSpan.Zero;
			var hasPositive = false;
			var hasNegative = false;
			foreach (var groupNumberToSpan in _groupNumberToSpans)
			{
				var value = match.Groups[groupNumberToSpan.Key].Value;
				if (!String.IsNullOrWhiteSpace(value))
				{
					var number = double.Parse(value, cultureInfo);
					result = result.Add(groupNumberToSpan.Value(number));
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
	}
}
