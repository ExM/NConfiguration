using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NConfiguration.Json.Parsing
{
	public enum TokenType
	{
		Object,
		Array,
		String,
		Null,
		Boolean,
		Number
	}
}
