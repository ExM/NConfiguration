using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDesk.Options;
using System.Security.Cryptography;
using System.IO;

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
			if(_keyFile == null)
				NotNull(_containerName, "containerName");

			if (_containerName == null)
				NotNull(_keyFile, "keyFile");
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

		public override string Description
		{
			get { return "Create RSA-key and saves it in key container and(or) in XML-file"; }
		}

		public override void Run()
		{
			var cp = new CspParameters();
			if(_containerName != null)
				cp.KeyContainerName = _containerName;
			cp.Flags = CspProviderFlags.UseMachineKeyStore;

			var rsa = new RSACryptoServiceProvider(_keySize, cp);
			if (_keyFile != null)
				File.WriteAllText(_keyFile, rsa.ToXmlString(true));
			rsa.PersistKeyInCsp = (_containerName != null);
			rsa.Clear();
		}
	}
}
