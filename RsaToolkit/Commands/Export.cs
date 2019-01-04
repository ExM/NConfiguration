using System;
using NDesk.Options;
using System.Security.Cryptography;
using System.IO;
using NETCore.Encrypt.Extensions.Internal;

namespace RsaToolkit.Commands
{
	public class Export: BaseCommand
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
			get { return "Export a RSA-key from the specified key container to the XML-file"; }
		}

		public override void Run()
		{
			try
			{
				var cp = new CspParameters();
				cp.KeyContainerName = _containerName;
				cp.Flags = CspProviderFlags.UseMachineKeyStore | CspProviderFlags.UseExistingKey;
				File.WriteAllText(_keyFile, RSAKeyExtensions.ToXmlString(new RSACryptoServiceProvider(cp), true));
			}
			catch (CryptographicException ex)
			{
				throw new ApplicationException("key not found in " + _containerName, ex);
			}
		}
	}
}
