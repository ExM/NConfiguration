
namespace Configuration
{
	/// <summary>
	/// store application settings
	/// </summary>
	public interface IAppSettings
	{
		/// <summary>
		/// Trying to load the configuration.
		/// </summary>
		/// <returns>
		/// Instance of the configuration, or null if no section name
		/// </returns>
		/// <param name='sectionName'>instance of application settings</param>
		/// <typeparam name='T'>type of configuration</typeparam>
		T TryLoad<T>(string sectionName) where T : class;
	}
}
