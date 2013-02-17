using Configuration.Xml;
using NUnit.Framework;
using Configuration.GenericView;
using Configuration.Tests;
using System;
using Configuration.Json.Parsing;

namespace Configuration.Json
{
	[TestFixture]
	public class ParsingTests
	{
		[TestCase(@"[""Unclosed array""")]
		[TestCase(@"{unquoted_key: ""keys must be quoted""}")]
		[TestCase(@"[""extra comma"",]")]
		[TestCase(@"[""double extra comma"",,]")]
		[TestCase(@"[   , ""<-- missing value""]")]
		[TestCase(@"[""Comma after the close""],")]
		[TestCase(@"[""Extra close""]]")]
		[TestCase(@"{""Extra comma"": true,}")]
		[TestCase(@"{""Extra value after close"": true} ""misplaced quoted value""")]
		[TestCase(@"{""Illegal expression"": 1 + 2}")]
		[TestCase(@"{""Illegal invocation"": alert()}")]
		[TestCase(@"{""Numbers cannot have leading zeroes"": 013}")]
		[TestCase(@"{""Numbers cannot be hex"": 0x14}")]
		[TestCase(@"[""Illegal backslash escape: \x15""]")]
		[TestCase(@"[\naked]")]
		[TestCase(@"[""Illegal backslash escape: \017""]")]
		[TestCase(@"{""Missing colon"" null}")]
		[TestCase(@"{""Double colon"":: null}")]
		[TestCase(@"{""Comma instead of colon"", null}")]
		[TestCase(@"[""Colon instead of comma"": false]")]
		[TestCase(@"[""Bad value"", truth]")]
		[TestCase(@"['single quote']")]
		[TestCase(@"[""	tab	character	in	string	""]")]
		[TestCase(@"[""tab\   character\   in\  string\  ""]")]
		[TestCase(@"[""line
break""]")]
		[TestCase(@"[""line\
break""]")]
		[TestCase(@"[0e]")]
		[TestCase(@"[0e+]")]
		[TestCase(@"[0e+-1]")]
		[TestCase(@"{""Comma instead if closing brace"": true,")]
		[TestCase(@"[""mismatch""}")]
		[ExpectedException(typeof(FormatException))]
		public void BadParse(string text)
		{
			JValue.Parse(text);
		}

		string _text1 = @"[
    ""JSON Test Pattern pass1"",
    {""object with 1 member"":[""array with 1 element""]},
    {},
    [],
    -42,
    true,
    false,
    null,
    {
        ""integer"": 1234567890,
        ""real"": -9876.543210,
        ""e"": 0.123456789e-12,
        ""E"": 1.234567890E+34,
        """":  23456789012E66,
        ""zero"": 0,
        ""one"": 1,
        ""space"": "" "",
        ""quote"": ""\"""",
        ""backslash"": ""\\"",
        ""controls"": ""\b\f\n\r\t"",
        ""slash"": ""/ & \/"",
        ""alpha"": ""abcdefghijklmnopqrstuvwyz"",
        ""ALPHA"": ""ABCDEFGHIJKLMNOPQRSTUVWYZ"",
        ""digit"": ""0123456789"",
        ""0123456789"": ""digit"",
        ""special"": ""`1~!@#$%^&*()_+-={':[,]}|;.</>?"",
        ""hex"": ""\u0123\u4567\u89AB\uCDEF\uabcd\uef4A"",
        ""true"": true,
        ""false"": false,
        ""null"": null,
        ""array"":[  ],
        ""object"":{  },
        ""address"": ""50 St. James Street"",
        ""url"": ""http://www.JSON.org/"",
        ""comment"": ""// /* <!-- --"",
        ""# -- --> */"": "" "",
        "" s p a c e d "" :[1,2 , 3

,

4 , 5        ,          6           ,7        ],""compact"":[1,2,3,4,5,6,7],
        ""jsontext"": ""{\""object with 1 member\"":[\""array with 1 element\""]}"",
        ""quotes"": ""&#34; \u0022 %22 0x22 034 &#x22;"",
        ""\/\\\""\uCAFE\uBABE\uAB98\uFCDE\ubcda\uef4A\b\f\n\r\t`1~!@#$%^&*()_+-=[]{}|;:',./<>?""
: ""A key can be any string""
    },
    0.5 ,98.6
,
99.44
,

1066,
1e1,
0.1e1,
1e-1,
1e00,2e+00,2e-00
,""rosebud""]";

		[Test]
		public void SuccessParse1()
		{
			var rootVal = JValue.Parse(_text1);
		}

		string _text2 = @"[[[[[[[[[[[[[[[[[[[""Too deep""]]]]]]]]]]]]]]]]]]]";

		[Test]
		public void SuccessParse2()
		{
			var rootVal = JValue.Parse(_text2);

			Assert.That(rootVal.Type, Is.EqualTo(TokenType.Array));
			
			var arr = (JArray)rootVal;

			for(int i=0;i<18; i++)
			{
				Assert.That(arr.Items.Count, Is.EqualTo(1));
				arr = (JArray)arr.Items[0];
			}

			Assert.That(arr.Items.Count, Is.EqualTo(1));
			Assert.That(arr.Items[0].Type, Is.EqualTo(TokenType.String));
			Assert.That(((JString)arr.Items[0]).Value, Is.EqualTo("Too deep"));
		}

		string _text3 = @"{
    ""JSON Test Pattern pass3"": {
        ""The outermost value"": ""must be an object or array."",
        ""In this test"": ""It is an object.""
    }
}";

		[Test]
		public void SuccessParse3()
		{
			var rootVal = JValue.Parse(_text3);
			Assert.That(rootVal.Type, Is.EqualTo(TokenType.Object));

			var obj = (JObject)rootVal;

			Assert.That(obj.Properties[0].Key, Is.EqualTo("JSON Test Pattern pass3"));
			Assert.That(obj.Properties[0].Value.Type, Is.EqualTo(TokenType.Object));

			obj = (JObject)obj.Properties[0].Value;
			Assert.That(obj.Properties[0].Key, Is.EqualTo("The outermost value"));
			Assert.That(obj.Properties[1].Key, Is.EqualTo("In this test"));

			Assert.That(((JString)obj.Properties[1].Value).Value, Is.EqualTo("It is an object."));
		}
		
	}
}

