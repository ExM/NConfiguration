using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDesk.Options;
using System.Security.Cryptography;
using System.IO;

namespace RsaToolkit.Commands
{
	public class Import: BaseCommand
	{
		private string _keyFile = null;
		private string _containerName = null;

		public override void Validate()
		{
			NotNull(_keyFile, "keyFile");
			NotNull(_containerName, "containerName");
		}

		protected override OptionSet OptionSetCreater()
		{
			return new OptionSet()
			{
				{ "f=|keyFile=", "file name for key in XML format", v => _keyFile = v },
				{ "n=|containerName=", "key container name", v => _containerName = v },
			};
		}

		public override string Description
		{
			get { return "Import a RSA-key from the specified XML-file to the key container"; ; }
		}

		public override void Run()
		{
			RSACryptoServiceProvider rsa = null;
			try
			{
				var cp = new CspParameters();
				cp.KeyContainerName = _containerName;
				cp.Flags = CspProviderFlags.UseMachineKeyStore;

				rsa = new RSACryptoServiceProvider(cp);
				rsa.FromXmlString(File.ReadAllText(_keyFile));
				rsa.PersistKeyInCsp = true;
				rsa.Clear();
			}
			catch (Exception)
			{
				if (rsa != null)
				{
					rsa.PersistKeyInCsp = false;
					rsa.Clear();
				}

				throw;
			}
		}
	}
}
