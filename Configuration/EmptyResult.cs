using System;
namespace Configuration
{
	/// <summary>
	/// behavior in without setting
	/// </summary>
	public enum EmptyResult
	{
		/// <summary>
		/// throwed of error
		/// </summary>
		Throw,
		/// <summary>
		/// return null
		/// </summary>
		Null,
		/// <summary>
		/// return default setting
		/// </summary>
		Default
	}
}

