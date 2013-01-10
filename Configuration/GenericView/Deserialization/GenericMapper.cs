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
		protected HashSet<Type> PrimitiveTypes { get; private set; }

		public GenericMapper()
		{
			PrimitiveTypes = InitPrimitiveTypes(
				typeof(String), typeof(Boolean), typeof(Byte), typeof(SByte), typeof(Char),
				typeof(Int16), typeof(Int32), typeof(Int64), typeof(UInt16), typeof(UInt32), typeof(UInt64), typeof(Single), typeof(Double),
				typeof(TimeSpan), typeof(DateTime),
				typeof(byte[]));
		}

		private HashSet<Type> InitPrimitiveTypes(params Type[] types)
		{
			var primitiveTypes = new HashSet<Type>();
			foreach (var t in types)
				primitiveTypes.Add(t);

			return primitiveTypes;
		}

		public bool IsPrimitive(Type type)
		{
			var ntype = Nullable.GetUnderlyingType(type);
			if(ntype != null) // is Nullable<>
				type = ntype;

			if (type.IsEnum)
				return true;

			return PrimitiveTypes.Contains(type);
		}

		public bool IsNullable(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		public bool IsCollection(Type type)
		{
			if (!type.IsGenericType)
				return false;

			var genType = type.GetGenericTypeDefinition();

			return genType == typeof(List<>) ||
				genType == typeof(IList<>) ||
				genType == typeof(ICollection<>);
		}

		public virtual object CreateFunction(Type targetType, IGenericDeserializer deserializer)
		{
			if(IsPrimitive(targetType))
				throw new ArgumentOutOfRangeException(string.Format("type '{0}' is primitive", targetType.FullName));

			if(targetType.IsArray ||
				IsCollection(targetType))
				throw new ArgumentOutOfRangeException(string.Format("type '{0}' is collection", targetType.FullName));

			var builder = new MapFunctionBuilder(targetType, deserializer, NativeNameFieldReader);

			return builder.Compile();
		}

		public Expression NativeNameFieldReader(MapFunctionBuilder builder, Type fieldType, string name, object[] customAttributes)
		{
			if(IsPrimitive(fieldType))
			{
				var methodName = (fieldType.IsByRef || IsNullable(fieldType)) ? "OptionalPrimitiveField" : "RequiredPrimitiveField";
				var mi = typeof(GenericMapper).GetMethod(methodName).MakeGenericMethod(fieldType);
				return Expression.Call(null, mi, Expression.Constant(name), builder.CfgNode);
			}

			if(fieldType.IsArray)
			{
				var itemType = fieldType.GetElementType();
				var mi = typeof(GenericMapper).GetMethod("Array").MakeGenericMethod(itemType);
				return Expression.Call(null, mi, Expression.Constant(name), builder.CfgNode, builder.Deserializer);
			}

			if(IsCollection(fieldType))
			{
				var itemType = fieldType.GetGenericArguments()[0];
				var mi = typeof(GenericMapper).GetMethod("List").MakeGenericMethod(itemType);
				return Expression.Call(null, mi, Expression.Constant(name), builder.CfgNode, builder.Deserializer);
			}

			{
				var methodName = (fieldType.IsByRef || IsNullable(fieldType)) ? "OptionalComplexField" : "RequiredComplexField";
				var mi = typeof (GenericMapper).GetMethod(methodName).MakeGenericMethod(fieldType);
				return Expression.Call(null, mi, Expression.Constant(name), builder.CfgNode, builder.Deserializer);
			}
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

		public static T OptionalPrimitiveField<T>(string name, ICfgNode node)
		{
			var field = node.GetChild(name);
			if (field == null)
				return default(T);

			return field.As<T>();
		}

		public static T RequiredPrimitiveField<T>(string name, ICfgNode node)
		{
			var field = node.GetChild(name);
			if (field == null)
				throw new FormatException(string.Format("field '{0}' not found", name));

			return field.As<T>();
		}

		public static T RequiredComplexField<T>(string name, ICfgNode node, IGenericDeserializer deserializer)
		{
			var field = node.GetChild(name);
			if (field == null)
				throw new FormatException(string.Format("field '{0}' not found", name));

			return deserializer.Deserialize<T>(field);
		}

		public static T OptionalComplexField<T>(string name, ICfgNode node, IGenericDeserializer deserializer)
		{
			var field = node.GetChild(name);
			if (field == null)
				return default(T);

			return deserializer.Deserialize<T>(field);
		}

		public static List<T> List<T>(string name, ICfgNode node, IGenericDeserializer deserializer)
		{
			return node.GetCollection(name)
				.Select(deserializer.Deserialize<T>)
				.ToList();
		}

		public static T[] Array<T>(string name, ICfgNode node, IGenericDeserializer deserializer)
		{
			return node.GetCollection(name)
				.Select(deserializer.Deserialize<T>)
				.ToArray();
		}
	}
}
