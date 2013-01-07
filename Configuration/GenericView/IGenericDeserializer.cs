using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Configuration.GenericView
{
	public interface IGenericDeserializer
	{
		T Deserialize<T>(ICfgNode cfgNode);
	}
}
