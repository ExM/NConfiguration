using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Configuration.GenericView.Deserialization
{
	internal static class LoadToolkit
	{
		public static T RequiredNode<T>(IGenericDeserializer deserializer, ICfgNode node, string name)
		{
			var field = node.GetChild(name);
			if (field == null)
				throw new FormatException(string.Format("field '{0}' not found", name));

			return deserializer.Deserialize<T>(field);
		}

		public static T OptionalNode<T>(IGenericDeserializer deserializer, ICfgNode node, string name)
		{
			var field = node.GetChild(name);
			if (field == null)
				return default(T);

			return deserializer.Deserialize<T>(field);
		}

		public static List<T> List<T>(IGenericDeserializer deserializer, ICfgNode node, string name)
		{
			return node.GetCollection(name)
				.Select(i => deserializer.Deserialize<T>(i))
				.ToList();
		}

		public static T[] Array<T>(IGenericDeserializer deserializer, ICfgNode node, string name)
		{
			return node.GetCollection(name)
				.Select(i => deserializer.Deserialize<T>(i))
				.ToArray();
		}

		public static T OptionalField<T>(ICfgNode node, string name)
		{
			var field = node.GetChild(name);
			if (field == null)
				return default(T);

			return field.As<T>();
		}

		public static T RequiredField<T>(ICfgNode node, string name)
		{
			var field = node.GetChild(name);
			if (field == null)
				throw new FormatException(string.Format("field '{0}' not found", name));

			return field.As<T>();
		}
	}
}
