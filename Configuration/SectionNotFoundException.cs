using System;
namespace Configuration
{
	/// <summary>
	/// Section not found exception.
	/// </summary>
	public class SectionNotFoundException: ApplicationException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Configuration.SectionNotFoundException"/> class.
		/// </summary>
		/// <param name='sectionName'>
		/// Section name.
		/// </param>
		public SectionNotFoundException(string sectionName)
			:base(string.Format("section `{0}' not found", sectionName))
		{
		}
	}
}

