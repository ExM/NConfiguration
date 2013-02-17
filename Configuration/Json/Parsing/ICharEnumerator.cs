using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Configuration.Json.Parsing
{
	public interface ICharEnumerator
	{
		char Current { get; }
		bool MoveNext();
		void MovePrev();
	}
}
