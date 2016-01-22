using System;

namespace NConfiguration.Combination
{
	public class CombinableTestClass: ICombinable
	{
		public string Text;

		public void Combine(object other)
		{
			if (other == null)
				return;

			Text = ((CombinableTestClass)other).Text + Text;
		}
	}
}

