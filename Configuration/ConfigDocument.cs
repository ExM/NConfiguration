using System;
namespace Configuration
{
	public class ConfigDocument: ConfigValue
	{
		public ConfigDocument ()
		{
		}
		
		public ConfigDocument Add(params ConfigElement[] elements)
		{
			
			return this;
		}
	}
}

