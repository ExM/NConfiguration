using System.Runtime.Serialization;

namespace NConfiguration.ExampleTypes
{
	public class CombinableConfig
	{
		[DataMember(Name = "Field1")]
		public string Field1 { get; set; }
	}
}

