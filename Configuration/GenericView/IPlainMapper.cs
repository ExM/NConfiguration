using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Collections.Concurrent;
using System.Xml.Linq;

namespace Configuration.GenericView
{
	public interface IPlainMapper
	{
		object CreateFunction(Type src, Type dst);
	}
}

