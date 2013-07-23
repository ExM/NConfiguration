
namespace NConfiguration
{
	/// <summary>
	/// store application settings
	/// </summary>
	public interface IIdentifiedSource: IAppSettings
	{
		/// <summary>
		/// source identifier the application settings
		/// </summary>
		string Identity { get; }
	}
}
