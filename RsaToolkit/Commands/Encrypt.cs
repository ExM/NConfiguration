using System;
using NDesk.Options;
using System.Xml;
using System.Configuration;
using System.Collections.Specialized;

namespace RsaToolkit.Commands
{
	public class Encrypt : BaseCommand
	{
		private string _containerName = null;

		private string _configFile = null;
		private string _sectionName = null;
		private string _providerName = null;

		public override void Validate()
		{
			NotNull(_containerName, "containerName");
			NotNull(_configFile, "configFile");
			NotNull(_sectionName, "sectionName");
			NotNull(_providerName, "providerName");
		}

		protected override OptionSet OptionSetCreater()
		{
			return new OptionSet()
			{
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
				
			provider.Initialize("RSA-key from key container", new NameValueCollection()
			{
				{"keyContainerName", _containerName},
				{"useMachineContainer", "true"}
			});

			XmlDocument doc = new XmlDocument();
			doc.Load(_configFile);

			var el = doc.DocumentElement[_sectionName];
			if(el == null)
				throw new ApplicationException("section not found");
				
			var cryptEl = doc.CreateElement(_sectionName);
			var prNameAttr = doc.CreateAttribute("configProtectionProvider");
			prNameAttr.Value = _providerName;
			cryptEl.Attributes.Append(prNameAttr);

			var cryptData = provider.EncryptFixup(el);
			cryptData = doc.ImportNode(cryptData, true);
			cryptEl.AppendChild(cryptData);
			doc.DocumentElement.ReplaceChild(cryptEl, el);

			doc.Save(_configFile);
		}
	}
}
