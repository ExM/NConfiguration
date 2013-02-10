using System;
namespace Configuration.GenericView
{
	public interface IViewConverterFactory
	{
		IViewConverter<T> GetConverter<T>();
	}
}
