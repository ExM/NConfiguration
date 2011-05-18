
namespace Configuration
{
	/// <summary>
	/// интерфейс представления настроек
	/// </summary>
	public interface IAppSettings
	{
		void Save<T>(T instance, string sectionName = null) where T : class;
		T Load<T>(EmptyResult mode, string sectionName = null) where T : class;
	}
}
