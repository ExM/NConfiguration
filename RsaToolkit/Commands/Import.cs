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
				rsa = new RSACryptoServiceProvider(new CspParameters
				{
					KeyContainerName = _containerName,
					Flags = CspProviderFlags.UseMachineKeyStore
				});
				RSAKeyExtensions.FromXmlString(rsa, File.ReadAllText(_keyFile));
				rsa.PersistKeyInCsp = true;
				rsa.Clear();
				
				if (_readAccess != null || _writeAccess != null)
				{
					var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
					var keyFile = Path.Combine(appDataFolder, "Microsoft", "Crypto", "RSA", "MachineKeys",
						rsa.CspKeyContainerInfo.UniqueKeyContainerName);
					
					setAccessRules(keyFile);
				}
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

		private void setAccessRules(string keyFile)
		{
			var fi = new FileInfo(keyFile);
			
			var acl = fi.GetAccessControl();
			
			foreach(var identity in getIdentityList(_writeAccess))
				acl.AddAccessRule(new FileSystemAccessRule(identity, FileSystemRights.FullControl, AccessControlType.Allow));

			foreach (var identity in getIdentityList(_readAccess))
				acl.AddAccessRule(new FileSystemAccessRule(identity, FileSystemRights.ReadData, AccessControlType.Allow));

			fi.SetAccessControl(acl);
		}

		private IEnumerable<string> getIdentityList(string identityList)
		{
			if (identityList == null)
				return Enumerable.Empty<string>();

			return identityList.Split(';')
				.Select(_ => _.Trim())
				.Where(_ => !string.IsNullOrEmpty(_));
		}
		
	}
}
