
using System.Collections.Generic;

namespace Configuration
{
	/// <summary>
	/// Store application settings
	/// </summary>
	public interface IAppSettings
	{
		/// <summary>
		/// Returns a collection of instances of configurations
		/// </summary>
		/// <typeparam name="T">type of instance of configuration</typeparam>
		/// <param name="sectionName">section name</param>
		IEnumerable<T> LoadCollection<T>(string sectionName);
	}
}
