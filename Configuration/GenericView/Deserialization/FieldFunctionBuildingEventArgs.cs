using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Configuration.GenericView.Deserialization
{
	public class FieldFunctionBuildingEventArgs : EventArgs
	{
		public Type Type { get; private set; }

		public object[] CustomAttributes { get; private set; }
		public FieldFunctionType Function { get; set; }
		public string Name { get; set; }
		public bool Required { get; set; }
		public bool Ignore { get; set; }

		public FieldFunctionBuildingEventArgs(Type type, string name, object[] customAttributes)
		{
			Type = type;
			CustomAttributes = customAttributes;
			Function = FieldFunctionType.Primitive;
			Name = name;
			Required = Type.IsValueType && !BuildToolkit.IsNullable(Type);
			Ignore = false;
		}
	}
}
