using System;
namespace Configuration.GenericView
{
	public interface IPlainConverter
	{
		TDst Convert<TSrc, TDst>(TSrc text);
	}
}
