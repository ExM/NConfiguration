using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Configuration.GenericView.Deserialization
{
	public class LoadDescription
	{
		public string Name { get; set; }
		public LoadType? LoadType { get; set; }
		public bool? Required { get; set; }

		public bool IsComplete
		{
			get
			{
				if (Name == null)
					return false;

				if (LoadType == null)
					return false;

				if (LoadType.Value == Deserialization.LoadType.Array ||
					LoadType.Value == Deserialization.LoadType.List)
					return true;

				return Required != null;
			}
		}

		public void SetName(string name, bool throwCollision)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			if (Name == null)
			{
				Name = name;
				return;
			}

			if (string.Equals(Name, name, StringComparison.InvariantCultureIgnoreCase))
				return;

			if (throwCollision)
				throw new ArgumentException(string.Format("ambiguous field name '{0}' or '{1}'", Name, name));
		}

		public static T PopAttribute<T>(List<object> attrs) where T: Attribute
		{
			var N = attrs.Count;
			for (int i = 0; i < N; i++)
			{
				T attr = attrs[i] as T;
				if (attr == null)
					continue;
				
				attrs.RemoveAt(i);
				return attr;
			}
			return null;
		}

		public void CheckXmlAttributes(List<object> customAttributes, bool throwCollision)
		{
			var attrAttr = PopAttribute<XmlAttributeAttribute>(customAttributes);
			if(attrAttr != null && !string.IsNullOrWhiteSpace(attrAttr.AttributeName))
				SetName(attrAttr.AttributeName, throwCollision);

			var elemAttr = PopAttribute<XmlElementAttribute>(customAttributes);
			if (elemAttr != null && !string.IsNullOrWhiteSpace(elemAttr.ElementName))
				SetName(elemAttr.ElementName, throwCollision);

			if(PopAttribute<XmlArrayAttribute>(customAttributes) != null)
				throw new NotImplementedException("XmlArrayAttribute not implemented");

			if(PopAttribute<XmlArrayItemAttribute>(customAttributes) != null)
				throw new NotImplementedException("XmlArrayItemAttribute not implemented");

			if(PopAttribute<XmlTextAttribute>(customAttributes) != null)
				throw new NotImplementedException("XmlTextAttribute not implemented");

			

			//customAttributes.First
		}

		public void CheckDataContractAttributes(List<object> customAttributes, bool throwCollision)
		{

		}

		public void CheckFieldName(string name, bool throwCollision)
		{
			SetName(name, throwCollision);
		}

		public void CheckFieldType(Type type, bool throwCollision)
		{
			
		}
	}
}
