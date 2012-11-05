﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDesk.Options;
using System.Security.Cryptography;
using System.IO;

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

		public override string Description
		{
			get { return "//TODO"; }
		}

		public override void Run()
		{
			try
			{
				var cp = new CspParameters();
				cp.KeyContainerName = _containerName;
				cp.Flags = CspProviderFlags.UseMachineKeyStore | CspProviderFlags.UseExistingKey;
				File.WriteAllText(_keyFile, new RSACryptoServiceProvider(cp).ToXmlString(true));
			}
			catch (CryptographicException ex)
			{
				throw new ApplicationException("key not found in " + _containerName, ex);
			}
		}
	}
}
