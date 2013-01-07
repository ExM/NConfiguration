using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace Configuration.GenericView.Deserialization
{
	public class GenericMapper
	{
		public GenericMapper()
		{
		}

		public Func<ICfgNode, T> CreateFunction<T>(IGenericDeserializer deserializer)
		{
			var builder = new MapFunctionBuilder<T>(deserializer);

			return builder.Compile();
		}

	}
}
