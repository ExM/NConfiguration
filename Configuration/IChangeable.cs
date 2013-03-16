using System;
namespace Configuration
{
	/// <summary>
	/// Object implementing this interface must notify changes.
	/// </summary>
	public interface IChangeable
	{
		/// <summary>
		/// Instance changed.
		/// </summary>
		event EventHandler Changed;
	}
}
