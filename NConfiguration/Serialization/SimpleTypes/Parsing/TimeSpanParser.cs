using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace NConfiguration.Serialization.SimpleTypes.Parsing
{
	public static class TimeSpanParser
	{
		enum GroupName
		{
			Minus,
			Day,
			Hour,
			Minute,
			Second
		}

		private const RegexOptions RegularExpressionOptions =
			RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled;
		private const string MinusPattern = "(-)";
		private const string PatternBase = @"(\d+(?:\.\d+)?)";

		private static readonly KeyValuePair<GroupName, Func<double, TimeSpan>>[] _groupNumberToSpans =
		{
			new KeyValuePair<GroupName, Func<double, TimeSpan>>(GroupName.Day, TimeSpan.FromDays),
			new KeyValuePair<GroupName, Func<double, TimeSpan>>(GroupName.Hour, TimeSpan.FromHours),
			new KeyValuePair<GroupName, Func<double, TimeSpan>>(GroupName.Minute, TimeSpan.FromMinutes),
			new KeyValuePair<GroupName, Func<double, TimeSpan>>(GroupName.Second, TimeSpan.FromSeconds)
		};

		private static string BuildGroupRegex(GroupName groupName, string postfix, string pattern = PatternBase)
		{
			return $"((?<{groupName}>{pattern}){postfix})?";
		}

		private static string BuildRegexp()
		{
			return "-"
					+ BuildGroupRegex(GroupName.Minus, "", MinusPattern)
					+ BuildGroupRegex(GroupName.Day, "d")
					+ BuildGroupRegex(GroupName.Hour, "h")
					+ BuildGroupRegex(GroupName.Minute, "m")
					+ BuildGroupRegex(GroupName.Second, "s")
					+"$";

		}

        private static readonly Regex _formatPattern = new Regex(BuildRegexp(), RegularExpressionOptions);

		public static TimeSpan Parse(string rawInput, CultureInfo cultureInfo)
		{
			TimeSpan result;
			if (TimeSpan.TryParse(rawInput, cultureInfo, out result))
			{
				return result;
			}
			return ParseShortFormat(rawInput, cultureInfo);
		}

		private static TimeSpan ParseShortFormat(string rawInput, CultureInfo cultureInfo)
		{
			var result = TimeSpan.Zero;

			if (string.IsNullOrWhiteSpace(rawInput))
			{
				throw new FormatException();
			}
			if (rawInput == string.Empty)
			{
				return result;
			}

			var hasNotValidSymbols = _formatPattern.Replace(rawInput, string.Empty, 1).Length > 0;
			if (hasNotValidSymbols)
			{
				throw new FormatException();
			}

			var match = _formatPattern.Match(rawInput);
			var multiplier = match.Groups[GroupName.Minus.ToString()].Success? -1: 1;
			foreach (var groupNumberToSpan in _groupNumberToSpans)
			{
				var group = match.Groups[groupNumberToSpan.Key.ToString()];
				if (group.Success)
				{
					var number = double.Parse(group.Value, cultureInfo);
					result = result.Add(groupNumberToSpan.Value(number * multiplier));
				}
			}
			return result;
		}
	}
}