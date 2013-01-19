
namespace Configuration
{
	/// <summary>
	/// store application settings
	/// </summary>
	public interface IAppSettingSource: IAppSettings
	{
		/// <summary>
		/// source identifier the application settings
		/// </summary>
		string Identity { get; }
	}
}
