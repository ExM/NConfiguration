using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace NConfiguration.Serialization.SimpleTypes.Parsing
{
    public class TimeSpanParser
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
        private const string MinusPattern = "(^-)";
        private const string PatternBase = @"(\d+(?:\.\d+)?)";

        private static readonly KeyValuePair<GroupName, Func<double, TimeSpan>>[] _groupNumberToSpans =
        {
            new KeyValuePair<GroupName, Func<double, TimeSpan>>(GroupName.Day, TimeSpan.FromDays),
            new KeyValuePair<GroupName, Func<double, TimeSpan>>(GroupName.Hour, TimeSpan.FromHours),
            new KeyValuePair<GroupName, Func<double, TimeSpan>>(GroupName.Minute, TimeSpan.FromMinutes),
            new KeyValuePair<GroupName, Func<double, TimeSpan>>(GroupName.Second, TimeSpan.FromSeconds)
        };

        private static string _buildGroupRegex(GroupName groupName, string postfix, string pattern = PatternBase)
        {
            return $"((?<{groupName}>{pattern}){postfix}){{0,1}}";
        }

        private static string _buildRegexp()
        {
            return _buildGroupRegex(GroupName.Minus, "", MinusPattern)
                   + _buildGroupRegex(GroupName.Day, "d")
                   + _buildGroupRegex(GroupName.Hour, "h")
                   + _buildGroupRegex(GroupName.Minute, "m")
                   + _buildGroupRegex(GroupName.Second, "s$");
        }

        private static readonly Regex _formatPattern = new Regex(_buildRegexp(), RegularExpressionOptions);

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
            var match = _formatPattern.Match(rawInput);
            var hasNotValidSymbols = _formatPattern.Replace(rawInput, "", 1).Length > 0;

            if (hasNotValidSymbols)
            {
                throw new FormatException();
            }

            var result = TimeSpan.Zero;
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