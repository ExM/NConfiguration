using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace Configuration.GenericView.Deserialization
{
	public delegate Expression FieldReaderCreator(MapFunctionBuilder builder, Type fieldType, string name, object[] customAttributes);

	public class GenericMapper
	{
		public GenericMapper()
		{
		}

		public virtual object CreateFunction(Type targetType, IGenericDeserializer deserializer)
		{
			var builder = new MapFunctionBuilder(targetType, deserializer, NativeNameFieldReader);

			return builder.Compile();
		}

		public static Expression NativeNameFieldReader(MapFunctionBuilder builder, Type fieldType, string name, object[] customAttributes)
		{
			var mi = typeof(GenericMapper).GetMethod("OptionalField").MakeGenericMethod(fieldType);
			return Expression.Call(null, mi, Expression.Constant(name), builder.CfgNode);
		}

		public static LoadDescription CreateLoadDescription(Type targetType, Type fieldType, string name, object[] customAttributes)
		{
			var attrList = customAttributes.ToList();
			var desc = new LoadDescription();

			//desc.CheckDataContractAttributes(attrList, false);
			//desc.CheckXmlAttributes(attrList, true);
			//desc.CheckFieldName(name, false);
			//desc.CheckFieldType(type, true);

			return desc;
		}

		public static bool DataContractAvailable(Type targetType)
		{
			//TODO: check DataContract attributes

			// DataContractAttribute
			// DataMemberAttribute
			// IgnoreDataMemberAttribute
			// CollectionDataContractAttribute
			// EnumMemberAttribute
			return false;
		}

		public static bool XmlAvailable(Type targetType)
		{
			//TODO: check Xml attributes

			// XmlAttributeAttribute
			// XmlElementAttribute>(customAttributes);
			// XmlArrayAttribute
			// XmlArrayItemAttribute
			// XmlTextAttribute
			// XmlIgnoreAttribute
			return false;
		}

		public static T OptionalField<T>(string name, ICfgNode node)
		{
			var field = node.GetChild(name);
			if (field == null)
				return default(T);

			return field.As<T>();
		}
	}
}
