using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RsaToolkit.Commands;
using NDesk.Options;

namespace RsaToolkit
{
	public abstract class BaseCommand
	{
		private static Dictionary<string, Type> _nameToTypeMap = new Dictionary<string, Type>()
		{
			{"create", typeof(Create)},
			{"import", typeof(Import)},
			{"export", typeof(Export)},
			{"remove", typeof(Remove)},
			{"encrypt", typeof(Encrypt)},
			{"decrypt", typeof(Decrypt)},
		};

		public static BaseCommand CreateFromName(string name)
		{
			Type cmdType;
			if (_nameToTypeMap.TryGetValue(name, out cmdType))
				return (BaseCommand)Activator.CreateInstance(cmdType);

			return null;
		}

		public static IEnumerable<KeyValuePair<string, BaseCommand>> AllCommands
		{
			get
			{
				return _nameToTypeMap.Select(pair =>
					new KeyValuePair<string, BaseCommand>(pair.Key, (BaseCommand)Activator.CreateInstance(pair.Value)));
			}
		}

		public BaseCommand()
		{
			_options = new Lazy<OptionSet>(OptionSetCreater);
		}
		
		protected abstract OptionSet OptionSetCreater();

		private Lazy<OptionSet> _options;

		public OptionSet Options
		{
			get
			{
				return _options.Value;
			}
		}

		public virtual void Initialize(IEnumerable<string> args)
		{
			var unexpectedOptions = Options.Parse(args);
			if (unexpectedOptions.Count != 0)
				throw new OptionException("found unexpected options: " + string.Join(", ", unexpectedOptions), "keySize");
		}

		public abstract void Validate();

		public abstract void Run();

		public abstract string Description { get; }

		protected void NotEmpty(string text, string name)
		{
			if (string.IsNullOrWhiteSpace(text))
				throw new ArgumentNullException(name, "required - " + Options[name].Description);
		}
	}
}
