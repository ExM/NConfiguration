using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NConfiguration.Serialization.SimpleTypes.Parsing.Time
{
    public class ShortFormatTimeSpanParser: IParser<TimeSpan>
    {
        private static Regex GetExpression(string unitOfMeasurement)
        {
            return new Regex($@"(\d+(?:\.\d+)?){unitOfMeasurement}", RegexOptions.IgnoreCase);
        }

        private readonly KeyValuePair<Regex, Func<double, TimeSpan>>[] _regexToSpans = 
        {
            new KeyValuePair<Regex, Func<double, TimeSpan>>(GetExpression("d"), TimeSpan.FromDays),
            new KeyValuePair<Regex, Func<double, TimeSpan>>(GetExpression("h"), TimeSpan.FromHours),
            new KeyValuePair<Regex, Func<double, TimeSpan>>(GetExpression("m"), TimeSpan.FromMinutes),
            new KeyValuePair<Regex, Func<double, TimeSpan>>(GetExpression("s"), TimeSpan.FromSeconds)
        };

        public bool TryParse(string rawInput, out TimeSpan result)
        {
            bool success = false;
            result = TimeSpan.Zero;
            foreach (var regexToSpan in _regexToSpans)
            {
                var match = regexToSpan.Key.Match(rawInput);
                if (match.Success)
                {
                    success = true;
                    result = result.Add(regexToSpan.Value(double.Parse(match.Groups[1].Value)));
                }
            }
            return success;
        }
    }
}
