using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDesk.Options;
using System.Security.Cryptography;
using System.IO;
using System.Xml;
using System.Configuration;
using System.Collections.Specialized;

namespace RsaToolkit.Commands
{
	public class Encrypt : BaseCommand
	{
		private string _keyFile = null;
		private string _containerName = null;

		private string _configFile = null;
		private string _sectionName = null;
		private string _providerName = null;

		public override void Validate()
		{
			if (_keyFile == null)
				NotNull(_containerName, "containerName");
			else
			{
				if (_containerName != null)
					throw new ArgumentOutOfRangeException("want to set the option 'keyFile' or 'containerName'");
			}

			NotNull(_configFile, "configFile");
			NotNull(_sectionName, "sectionName");
			NotNull(_providerName, "providerName");
		}

		protected override OptionSet OptionSetCreater()
		{
			return new OptionSet()
			{
				{ "f=|keyFile=", "file name of key in XML format", v => _keyFile = v },
				{ "n=|containerName=", "key container name", v => _containerName = v },

				{ "c=|configFile=", "file name of configuration in XML format", v => _configFile = v },
				{ "s=|sectionName=", "section name", v => _sectionName = v },
				{ "p=|providerName=", "provider name", v => _providerName = v }
			};
		}

		public override string Description
		{
			get { return "Encrypt section in the file of configurations"; }
		}

		public override void Run()
		{
			var provider = new RsaProtectedConfigurationProvider();
				
			if (_containerName != null)
				provider.Initialize("RSA-key from key container", new NameValueCollection()
				{
					{"keyContainerName", _containerName},
					{"useMachineContainer", "true"}
				});

			if (_keyFile != null)
			{
				provider.Initialize("RSA-key from XML-file", new NameValueCollection());
				provider.ImportKey(_keyFile, false);
			}

			XmlDocument doc = new XmlDocument();
			doc.Load(_configFile);

			var el = doc.DocumentElement[_sectionName];
			if(el == null)
				throw new ApplicationException("section not found");
				
			var cryptEl = doc.CreateElement(_sectionName);
			var prNameAttr = doc.CreateAttribute("configProtectionProvider");
			prNameAttr.Value = _providerName;
			cryptEl.Attributes.Append(prNameAttr);

			var cryptData = provider.Encrypt(el);
			cryptData = doc.ImportNode(cryptData, true);
			cryptEl.AppendChild(cryptData);
			doc.DocumentElement.ReplaceChild(cryptEl, el);

			doc.Save(_configFile);
		}
	}
}
