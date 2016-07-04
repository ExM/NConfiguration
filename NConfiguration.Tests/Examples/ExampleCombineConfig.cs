using NConfiguration.Combination;
using System.Runtime.Serialization;

namespace NConfiguration.Examples
{
	public class ExampleCombineConfig : ICombinable, ICombinable<ExampleCombineConfig>
	{
		[DataMember]
		public string F = null;

		public void Combine(ICombiner combiner, object other)
		{
			Combine(combiner, other as ExampleCombineConfig);
		}

		public void Combine(ICombiner combiner, ExampleCombineConfig other)
		{
			if (other == null)
				return;

			F = other.F ?? F;
		}
	}
}

