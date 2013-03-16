using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Configuration.GenericView.Deserialization
{
	public class FieldFunctionInfo
	{
		public Type ResultType { get; private set; }
		public FieldFunctionType Function { get; set; }
		public string Name { get; set; }
		public bool Required { get; set; }
		public bool Ignore { get; set; }

		public FieldFunctionInfo(Type resultType, string fieldName)
		{
			ResultType = resultType;
			Name = fieldName;
		}
	}
}
