using System;

namespace Configuration.GenericView.Deserialization
{
	public interface IGenericMapper
	{
		object CreateFunction(Type targetType, IGenericDeserializer deserializer);
	}
}