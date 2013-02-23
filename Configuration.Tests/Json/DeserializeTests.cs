using Configuration.Xml;
using NUnit.Framework;
using Configuration.GenericView;
using Configuration.Tests;
using System;
using Configuration.Json.Parsing;
using System.Linq;
using System.Collections.Generic;

namespace Configuration.Json
{
	[TestFixture]
	public class DeserializeTests
	{
		JsonStringSettings settings = new JsonStringSettings(@"{
	""Section1"":
	{
		""Item1"":""item1.value1"",
		""Item2"": 1231232,
		""nullItem"":null
	},
	""Section2"":
	{
		""Item1"": [ 1, 2 ],
		""Item1"": null,
		""Item1"": [ null, [ 1, 2 ]]
	},
	""Section3"":
	{
		""Item1"":""item1.value1"",
		""Item1"": [ 1, 2 ],
		""Item1"": null,
		""Item1"": [ null, [ 1, 2 ]]
	}}");

		public class Section1
		{
			public string Item1 { get; set; }
			public long Item2 { get; set; }
			public int? nullItem { get; set; }
		}

		public class Section2
		{
			public List<int?> Item1 { get; set; }
		}

		public class Section3
		{
			public List<string> Item1 { get; set; }
		}

		[Test]
		public void Simple()
		{
			var section = settings.First<Section1>("Section1");
			Assert.That(section.Item1, Is.EqualTo("item1.value1"));
			Assert.That(section.Item2, Is.EqualTo(1231232));
			Assert.That(section.nullItem, Is.Null);
		}

		[Test]
		public void MultiIntArray()
		{
			var section = settings.First<Section2>("Section2");
			Assert.That(section.Item1, Is.EquivalentTo(new int?[] { 1, 2, null, null, 1, 2}));
		}

		[Test]
		public void MultiStringArray()
		{
			var section = settings.First<Section3>("Section3");
			Assert.That(section.Item1, Is.EquivalentTo(new string[] {"item1.value1", "1", "2", null, null, "1", "2" }));
		}
	}
}

