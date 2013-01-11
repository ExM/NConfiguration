using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Configuration.GenericView
{
	internal interface IEnumParser<T> where T: struct
	{
		T ParseOne(string text);
		T ParseFlags(string text);
	}
}
