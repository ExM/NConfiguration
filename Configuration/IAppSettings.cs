
using System.Collections.Generic;

namespace Configuration
{
	/// <summary>
	/// store application settings
	/// </summary>
	public interface IAppSettings
	{
		IEnumerable<T> LoadCollection<T>(string sectionName);
	}
}
