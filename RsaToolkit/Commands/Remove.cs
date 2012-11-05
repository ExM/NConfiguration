using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDesk.Options;
using System.Security.Cryptography;

namespace RsaToolkit.Commands
{
	public class Remove: BaseCommand
	{
		private string _containerName = null;

		public override void Validate()
		{
			NotNull(_containerName, "containerName");
		}

		protected override OptionSet OptionSetCreater()
		{
			return new OptionSet()
			{
				{ "n=|containerName=", "key container name", v => _containerName = v },
			};
		}

		public override string Description
		{
			get { return "Remove a RSA-key from the specified key container"; }
		}

		public override void Run()
		{
			try
			{
				var cp = new CspParameters();
				cp.KeyContainerName = _containerName;
				cp.Flags = CspProviderFlags.UseMachineKeyStore | CspProviderFlags.UseExistingKey;
				var rsa = new RSACryptoServiceProvider(cp);
				rsa.PersistKeyInCsp = false;
				rsa.Clear();
			}
			catch (CryptographicException ex)
			{
				throw new ApplicationException("key not found in " + _containerName, ex);
			}
		}
	}
}
