namespace NConfiguration.Serialization
{
	public class TestMemberAttrClass
	{
		public string F1;

		[Deserializer(typeof(PlaceHolderDeserializer))]
		public string F1A;

		public string P1 { get; set; }

		[Deserializer(typeof(PlaceHolderDeserializer))]
		public string P1A { get; set; }
	}
}

