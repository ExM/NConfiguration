using NConfiguration.Combination;
using NConfiguration.Serialization;
using System.Collections.Generic;

namespace NConfiguration
{
	/// <summary>
	/// Store application settings
	/// </summary>
	public interface IAppSettings
	{
		IConfigNodeProvider Nodes { get; }

		IDeserializer Deserializer { get; }

		ICombiner Combiner { get; }
	}
}
