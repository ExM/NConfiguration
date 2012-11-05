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
		public static BaseCommand CreateFromName(string name)
		{
			switch (name)
			{
				case "create": return new Create();
				case "import": return new Import();
				case "export": return new Export();
				case "remove": return new Remove();
				case "encrypt": return new Encrypt();
				case "decrypt": return new Decrypt();
				default: return null;
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

		protected void NotEmpty(string text, string name)
		{
			if (string.IsNullOrWhiteSpace(text))
				throw new ArgumentNullException(name, "required - " + Options[name].Description);
		}
	}
}
