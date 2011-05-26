
namespace Configuration
{
	/// <summary>
	/// store application settings
	/// </summary>
	public interface IAppSettings
	{
		/// <summary>
		/// saves a copy of the settings of this type
		/// </summary>
		/// <typeparam name="T">setting type</typeparam>
		/// <param name="instance">instance of the setting</param>
		/// <param name="sectionName">section name</param>
		void Save<T>(T instance, string sectionName = null) where T : class;
		/// <summary>
		/// creates an instance of the settings of this type
		/// </summary>
		/// <typeparam name="T">setting type</typeparam>
		/// <param name="mode">behavior in without setting</param>
		/// <param name="sectionName">section name</param>
		T Load<T>(EmptyResult mode, string sectionName = null) where T : class;
	}
}
