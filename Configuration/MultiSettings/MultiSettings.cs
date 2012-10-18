using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Configuration
{
	public class MultiSettings : IAppSettings
	{
		private LinkedList<IAppSettings> _list = new LinkedList<IAppSettings>();
		private ICombineFactory _combineFactory;

		public MultiSettings()
			: this(new CombineFactory())
		{
		}

		public MultiSettings(ICombineFactory combineFactory)
		{
			if(combineFactory == null)
				throw new ArgumentNullException("combineFactory");
			_combineFactory = combineFactory;
		}
		
		public void Add(IAppSettings setting)
		{
			_list.AddLast(setting);
		}
		
		/// <summary>
		/// Trying to load the configuration.
		/// </summary>
		/// <returns>
		/// Instance of the configuration, or null if no section name
		/// </returns>
		/// <param name='sectionName'>instance of application settings</param>
		/// <typeparam name='T'>type of configuration</typeparam>
		public T TryLoad<T>(string sectionName) where T : class
		{
			if(sectionName == null)
				throw new ArgumentNullException("sectionName");
			
			if(_list.Count == 0)
				return null;
			
			Func<T,T,T> combinator = _combineFactory.GetCombinator<T>();
			var node = _list.First;
			T result = node.Value.TryLoad<T>(sectionName);
			node = node.Next;
			
			while(node != null)
			{
				T nextCfg = node.Value.TryLoad<T>(sectionName);
				result = combinator(result, nextCfg);
				node = node.Next;
			}
			
			return result;
		}

		public string Identity
		{
			get
			{
				return GetHashCode().ToString();
			}
		}
	}
}

