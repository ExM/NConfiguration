using System;
using System.Collections.Generic;
using System.Linq;
using NDesk.Options;
using System.Security.Cryptography;
using System.IO;
using System.Security.Principal;
using System.Security.AccessControl;
using NETCore.Encrypt.Extensions.Internal;

namespace RsaToolkit.Commands
{
	public class Import: BaseCommand
	{
		private string _keyFile = null;
		private string _containerName = null;
		private string _writeAccess = null;
		private string _readAccess = null;

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
				{ "w=|writeAccess=", "list of users for full access (separator ;)", v => _writeAccess = v },
				{ "r=|readAccess=", "list of users for read access (separator ;)", v => _readAccess = v }
			};
		}

		public override string Description
		{
			get { return "Import a RSA-key from the specified XML-file to the key container"; }
		}

		public override void Run()
		{
			RSACryptoServiceProvider rsa = null;
			try
			{
				var cp = new CspParameters();
				cp.KeyContainerName = _containerName;
				cp.Flags = CspProviderFlags.UseMachineKeyStore;
				//TODO
				//cp.CryptoKeySecurity = createAccessRules(); 

				rsa = new RSACryptoServiceProvider(cp);
				RSAKeyExtensions.FromXmlString(rsa, File.ReadAllText(_keyFile));
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

		/*
		private CryptoKeySecurity createAccessRules()
		{
			System.Security.AccessControl.

			var defaultRules = true;
			var result = new CryptoKeySecurity();

			foreach(var identity in getIdentityList(_writeAccess))
			{
				result.AddAccessRule(new CryptoKeyAccessRule(new NTAccount(identity), CryptoKeyRights.FullControl, AccessControlType.Allow));
				defaultRules = false;
			}

			foreach (var identity in getIdentityList(_readAccess))
			{
				result.AddAccessRule(new CryptoKeyAccessRule(new NTAccount(identity), CryptoKeyRights.GenericRead, AccessControlType.Allow));
				defaultRules = false;
			}

			return defaultRules ? null : result;
		}

		private IEnumerable<string> getIdentityList(string identityList)
		{
			if (identityList == null)
				return Enumerable.Empty<string>();

			return identityList.Split(';')
				.Select(_ => _.Trim())
				.Where(_ => !string.IsNullOrEmpty(_));
		}
		*/
	}
}
