using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDesk.Options;

namespace RsaToolkit.Commands
{
	public class Decrypt : BaseCommand
	{
		private string _keyFile = null;
		private string _containerName = null;

		private string _configFile = null;
		private string _sectionName = null;

		public override void Validate()
		{
			if (string.IsNullOrWhiteSpace(_keyFile))
				NotEmpty(_containerName, "containerName");
			else
			{
				if (!string.IsNullOrWhiteSpace(_containerName))
					throw new ArgumentOutOfRangeException("want to set the option 'keyFile' or 'containerName'");
			}

			NotEmpty(_configFile, "configFile");
			NotEmpty(_sectionName, "sectionName");
		}

		protected override OptionSet OptionSetCreater()
		{
			return new OptionSet()
			{
				{ "f=|keyFile=", "file name of key in XML format", v => _keyFile = v },
				{ "n=|containerName=", "key container name", v => _containerName = v },

				{ "c=|configFile=", "file name of configuration in XML format", v => _configFile = v },
				{ "s=|sectionName=", "section name", v => _sectionName = v },
			};
		}

		public override string Description
		{
			get { return "//TODO"; }
		}

		public override void Run()
		{
			//TODO
			Console.WriteLine("run Decrypt {0} {1} {2} {3}", _keyFile, _containerName, _configFile, _sectionName);
		}
	}
}
