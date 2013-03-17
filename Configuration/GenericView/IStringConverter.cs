using System;
namespace Configuration.GenericView
{
	/// <summary>
	/// Converter string into a simple values
	/// </summary>
	public interface IStringConverter
	{
		/// <summary>
		/// Returns an object of the specified type and whose value is equivalent to the specified object.
		/// </summary>
		/// <typeparam name="T">required type of object</typeparam>
		/// <param name="text">A string that represents the object to convert.</param>
		T Convert<T>(string text);
	}
}
