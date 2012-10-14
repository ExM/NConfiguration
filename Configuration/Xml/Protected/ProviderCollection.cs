using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using System.Xml;

namespace Configuration.Xml.Protected
{
	public class ProviderCollection : IProviderCollection
	{
		private Dictionary<string, ProtectedConfigurationProvider> _map = new Dictionary<string, ProtectedConfigurationProvider>();

		public ProviderCollection()
		{
		}

		public void Clear()
		{
			_map.Clear();
		}
		
		public ProtectedConfigurationProvider Get(string name)
		{
			ProtectedConfigurationProvider provider;
			if (_map.TryGetValue(name, out provider))
				return provider;
			else
				return null;
		}

		public void Set(string name, ProtectedConfigurationProvider provider)
		{
			if (_map.ContainsKey(name))
				_map[name] = provider;
			else
				_map.Add(name, provider);
		}

		public bool Remove(string name)
		{
			return _map.Remove(name);
		}
	}
}
