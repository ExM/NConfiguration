using NConfiguration.Combination;
using System.Runtime.Serialization;

namespace NConfiguration.ExampleTypes
{
	public class CustomCombinableConfig : ICombinable, ICombinable<CustomCombinableConfig>
	{
		[DataMember(Name = "Field1")]
		public string Field1 { get; set; }

		public void Combine(ICombiner combiner, CustomCombinableConfig other)
		{
			if (other == null)
				return;

			if (other.Field1 != null)
				Field1 += other.Field1;
		}

		public virtual void Combine(ICombiner combiner, object other)
		{
			Combine(combiner, other as CustomCombinableConfig);
		}
	}
}

