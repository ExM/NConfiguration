using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NConfiguration.GenericView
{
	public interface IGenericDeserializer
	{
		T Deserialize<T>(ICfgNode cfgNode);
	}
}
