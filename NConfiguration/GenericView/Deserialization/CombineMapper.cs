using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace NConfiguration.GenericView.Deserialization
{
	public class CombineMapper : ICombineMapper
	{
		public CombineMapper()
		{
		}

		public object CreateFunction(Type targetType, IGenericCombiner combiner)
		{
			throw new NotImplementedException();
		}
	}
}
