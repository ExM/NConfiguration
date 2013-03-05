using System;
namespace Configuration
{
	public interface IChangeable
	{
		event EventHandler Changed;
	}
}
