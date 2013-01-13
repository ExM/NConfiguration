using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Configuration.GenericView.Deserialization
{
	public delegate Expression FieldReaderCreator(MapFunctionBuilder builder, Type fieldType, string name, object[] customAttributes);

	public class GenericMapper
	{
		protected HashSet<Type> PrimitiveTypes { get; private set; }

		public GenericMapper()
		{
			PrimitiveTypes = new HashSet<Type>()
			{
				typeof(String), typeof(Boolean), typeof(Char),
				typeof(Byte), typeof(SByte),
				typeof(Int16), typeof(Int32), typeof(Int64), typeof(UInt16), typeof(UInt32), typeof(UInt64),
				typeof(Single), typeof(Double),
				typeof(TimeSpan), typeof(DateTime),
				typeof(byte[])
			};
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

			return genType == typeof(List<>)
				|| genType == typeof(IList<>)
				|| genType == typeof(ICollection<>)
				|| genType == typeof(IEnumerable<>);
		}

		public virtual object CreateFunction(Type targetType, IGenericDeserializer deserializer)
		{
			if(IsPrimitive(targetType))
				return CreatePrimitiveTargetReader(targetType);

			if(targetType.IsArray ||
				IsCollection(targetType))
				throw new ArgumentOutOfRangeException(string.Format("type '{0}' is collection", targetType.FullName));

			var builder = new MapFunctionBuilder(targetType, deserializer, NativeNameFieldReader);

			return builder.Compile();
		}

		private object CreatePrimitiveTargetReader(Type type)
		{
			var mi = typeof(GenericMapper).GetMethod("PrimitiveTarget").MakeGenericMethod(type);
			var funcType = typeof(Func<,>).MakeGenericType(typeof(ICfgNode),type);
			return Delegate.CreateDelegate(funcType, mi);
		}

		public Expression NativeNameFieldReader(MapFunctionBuilder builder, Type fieldType, string name, object[] customAttributes)
		{
			if(IsPrimitive(fieldType))
			{
				var methodName = (!fieldType.IsValueType || IsNullable(fieldType)) ? "OptionalPrimitiveField" : "RequiredPrimitiveField";
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
				var methodName = (!fieldType.IsValueType || IsNullable(fieldType)) ? "OptionalComplexField" : "RequiredComplexField";
				var mi = typeof (GenericMapper).GetMethod(methodName).MakeGenericMethod(fieldType);
				return Expression.Call(null, mi, Expression.Constant(name), builder.CfgNode, builder.Deserializer);
			}
		}

		public enum AttributeState
		{
			Found,
			NotFound,
			NotImplemented
		}

		private static readonly Dictionary<Type, AttributeState> DataContractAttributeStates = new Dictionary<Type, AttributeState>
		{
			{typeof(DataContractAttribute), AttributeState.Found}, // not used
			{typeof(DataMemberAttribute), AttributeState.Found}, // change name, set required
			{typeof(IgnoreDataMemberAttribute), AttributeState.Found}, // ignore
			{typeof(CollectionDataContractAttribute), AttributeState.NotImplemented} // not implemented
		};

		public static AttributeState DataContractAvailable(Type targetType)
		{
			return CustomAttributesAvailable(targetType, DataContractAttributeStates);
		}

		private static readonly Dictionary<Type, AttributeState> XmlAttributeStates = new Dictionary<Type, AttributeState>()
		{
			{typeof(XmlRootAttribute), AttributeState.Found}, // not used
			{typeof(XmlAttributeAttribute), AttributeState.Found}, // change name
			{typeof(XmlElementAttribute), AttributeState.Found}, // change name
			{typeof(XmlArrayAttribute), AttributeState.NotImplemented}, // not implemented
			{typeof(XmlArrayItemAttribute), AttributeState.NotImplemented}, // not implemented
			{typeof(XmlTextAttribute), AttributeState.NotImplemented}, // not implemented
			{typeof(XmlAnyElementAttribute), AttributeState.NotImplemented}, // not implemented
			{typeof(XmlIgnoreAttribute), AttributeState.Found}, // ignore
		};

		public static AttributeState XmlAvailable(Type targetType)
		{
			return CustomAttributesAvailable(targetType, XmlAttributeStates);
		}

		private static AttributeState CustomAttributesAvailable(Type targetType, Dictionary<Type, AttributeState> attrStates)
		{
			AttributeState result = AttributeState.NotFound;
			CheckAttributes(targetType.GetCustomAttributes(true), ref result, attrStates);
			foreach (var mi in targetType.FindMembers(MemberTypes.Field | MemberTypes.Property, BindingFlags.Public | BindingFlags.Instance, (m, o) => true, null))
				CheckAttributes(mi.GetCustomAttributes(true), ref result, attrStates);
			return result;
		}

		private static void CheckAttributes(object[] attrs, ref AttributeState result, Dictionary<Type, AttributeState> attrStates)
		{
			if (result == AttributeState.NotImplemented)
				return;

			foreach (object attr in attrs)
			{
				AttributeState state;
				if (!attrStates.TryGetValue(attr.GetType(), out state))
					continue;
				
				if (state == AttributeState.NotImplemented)
				{
					result = AttributeState.NotImplemented;
					return;
				}
					
				result = AttributeState.Found;
			}
		}

		public static T PrimitiveTarget<T>(ICfgNode node)
		{
			return node.As<T>();
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
			Console.WriteLine("Array {0} {1}", typeof(T), name);

			return node.GetCollection(name)
				.Select(deserializer.Deserialize<T>)
				.ToArray();
		}
	}
}
