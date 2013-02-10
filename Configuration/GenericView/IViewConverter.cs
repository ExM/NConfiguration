using System;
namespace Configuration.GenericView
{
	public interface IViewConverter<TSrc>
	{
		TDst Convert<TDst>(TSrc text);
	}
}
