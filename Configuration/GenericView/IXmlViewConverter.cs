using System;
namespace Configuration.GenericView
{
	public interface IXmlViewConverter
	{
		T Convert<T>(string text);
	}
}
