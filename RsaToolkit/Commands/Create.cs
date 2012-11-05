using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDesk.Options;

namespace RsaToolkit.Commands
{
	public class Create: BaseCommand
	{
		private string _keyFile = null;
		private string _keySizeText = null;
		private int _keySize;
		private string _containerName = null;

		public override void Initialize(IEnumerable<string> args)
		{
			base.Initialize(args);
			SetKeySize();
		}

		private void SetKeySize()
		{
			if (_keySizeText == null)
			{
				_keySize = 1024;
				return;
			}

			if (int.TryParse(_keySizeText, out _keySize))
				return;

			throw new FormatException("unexpected value in 'keySize' option");
		}

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
					{ "s=|keySize=", "the size of the key to use in bits (default - 1024)", v => _keySizeText = v },
					{ "n=|containerName=", "key container name", v => _containerName = v },
				};
		}

		public override void Run()
		{
			//TODO
			Console.WriteLine("run create {0} {1} {2}", _keyFile, _keySize, _containerName);
		}
	}
}
