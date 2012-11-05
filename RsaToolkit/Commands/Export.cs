using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDesk.Options;

namespace RsaToolkit.Commands
{
	public class Export: BaseCommand
	{
		private string _keyFile = null;
		private string _containerName = null;

		public override void Validate()
		{
			NotEmpty(_keyFile, "keyFile");
			NotEmpty(_containerName, "containerName");
		}

		protected override OptionSet OptionSetCreater()
		{
			return new OptionSet()
			{
				{ "f=|keyFile=", "file name for key in XML format", v => _keyFile = v },
				{ "n=|containerName=", "key container name", v => _containerName = v },
			};
		}

		public override void Run()
		{
			//TODO
			Console.WriteLine("run Export {0} {1}", _keyFile, _containerName);
		}
	}
}
