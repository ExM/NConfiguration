
namespace NConfiguration
{
	/// <summary>
	/// This configuration is loaded from the file.
	/// The directory containing the file can be used to search for a relative path.
	/// </summary>
	public interface IFilePathOwner
	{
		/// <summary>
		/// Directory containing the configuration file
		/// </summary>
		string Path { get; }
	}
}
