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
	public class GenericMapper
	{
		protected HashSet<Type> PrimitiveTypes { get; private set; }

		public GenericMapper()
		{
			PrimitiveTypes = new HashSet<Type>
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

			var builder = new ComplexFunctionBuilder(targetType, deserializer);

			if (BuildToolkit.DataContractAvailable(targetType) == AttributeState.Found)
			{ // DataContract deserialize
				builder.FieldFunctionBuilding += DataContractFieldReader;
			}
			else if (BuildToolkit.XmlAvailable(targetType) == AttributeState.Found)
			{ // Xml deserialize
				builder.FieldFunctionBuilding += XmlFieldReader;
			}
			else
			{ // Native name deserialize
				builder.FieldFunctionBuilding += NativeNameFieldReader;
			}
			
			return builder.Compile();
		}

		void builder_FieldFunctionBuilding(object sender, FieldFunctionBuildingEventArgs e)
		{
			throw new NotImplementedException();
		}

		public static object CreatePrimitiveTargetReader(Type type)
		{
			var mi = typeof(BuildToolkit).GetMethod("PrimitiveTarget").MakeGenericMethod(type);
			var funcType = typeof(Func<,>).MakeGenericType(typeof(ICfgNode),type);
			return Delegate.CreateDelegate(funcType, mi);
		}

		public void NativeNameFieldReader(object sender, FieldFunctionBuildingEventArgs e)
		{
			DefaultFunctionType(e);
		}

		private void DefaultFunctionType(FieldFunctionBuildingEventArgs e)
		{
			if (IsPrimitive(e.Type))
				e.Function = FieldFunctionType.Primitive;
			else if (IsCollection(e.Type))
				e.Function = FieldFunctionType.Collection;
			else
				e.Function = FieldFunctionType.Complex;
		}
		
		public void DataContractFieldReader(object sender, FieldFunctionBuildingEventArgs e)
		{
			if (e.CustomAttributes.Any(o => o is IgnoreDataMemberAttribute))
			{
				e.Ignore = true;
				return;
			}

			DefaultFunctionType(e);

			var dmAttr = e.CustomAttributes.Select(o => o as DataMemberAttribute).FirstOrDefault(a => a != null);

			if (dmAttr != null && !string.IsNullOrWhiteSpace(dmAttr.Name))
				e.Name = dmAttr.Name;

			if(dmAttr != null)
				e.Required = dmAttr.IsRequired;
		}

		public void XmlFieldReader(object sender, FieldFunctionBuildingEventArgs e)
		{
			if (e.CustomAttributes.Any(o => o is XmlIgnoreAttribute))
			{
				e.Ignore = true;
				return;
			}

			DefaultFunctionType(e);

			var attrAttr = e.CustomAttributes.Select(o => o as XmlAttributeAttribute).FirstOrDefault(a => a != null);
			var elAttr = e.CustomAttributes.Select(o => o as XmlElementAttribute).FirstOrDefault(a => a != null);

			if (attrAttr != null && elAttr != null)
				throw new ArgumentOutOfRangeException(string.Format("found XmlAttributeAttribute and XmlElementAttribute for member '{0}'", e.Name));

			if (attrAttr != null && !string.IsNullOrWhiteSpace(attrAttr.AttributeName))
				e.Name = attrAttr.AttributeName;

			if (elAttr != null && !string.IsNullOrWhiteSpace(elAttr.ElementName))
				e.Name = elAttr.ElementName;
		}
	}
}
