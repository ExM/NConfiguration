
namespace Configuration
{
	/// <summary>
	/// store application settings
	/// </summary>
	public interface IAppSettings
	{
		/// <summary>
		/// creates an instance of the settings of this type
		/// </summary>
		/// <typeparam name="T">setting type</typeparam>
		/// <param name="sectionName">section name</param>
		T Load<T>(string sectionName) where T : class;
	}
}
