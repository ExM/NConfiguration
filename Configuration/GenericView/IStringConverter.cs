using System;
namespace Configuration.GenericView
{
	public interface IStringConverter
	{
		T Convert<T>(string text);
	}
}
