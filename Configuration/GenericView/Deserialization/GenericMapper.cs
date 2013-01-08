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
			var builder = new MapFunctionBuilder<T>(this, deserializer);

			return builder.Compile();
		}

		public virtual LoadDescription CreateLoadDescription(Type type, string name, object[] customAttributes)
		{
			var attrList = customAttributes.ToList();
			var desc = new LoadDescription();

			desc.CheckDataContractAttributes(attrList, false);
			desc.CheckXmlAttributes(attrList, true);
			desc.CheckFieldName(name, false);
			desc.CheckFieldType(type, true);

			return desc;
		}
	}
}
