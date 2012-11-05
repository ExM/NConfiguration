using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDesk.Options;

namespace RsaToolkit.Commands
{
	public class Remove: BaseCommand
	{
		private string _containerName = null;

		public override void Validate()
		{
			NotEmpty(_containerName, "containerName");
		}

		protected override OptionSet OptionSetCreater()
		{
			return new OptionSet()
			{
				{ "n=|containerName=", "key container name", v => _containerName = v },
			};
		}

		public override void Run()
		{
			//TODO
			Console.WriteLine("run Remove {0}", _containerName);
		}
	}
}
