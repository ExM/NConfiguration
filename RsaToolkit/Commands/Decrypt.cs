using System;
using NDesk.Options;
using System.Configuration;
using System.Collections.Specialized;
using System.Xml;

namespace RsaToolkit.Commands
{
	public class Decrypt : BaseCommand
	{
		private string _containerName = null;

		private string _configFile = null;
		private string _sectionName = null;

		public override void Validate()
		{
			NotNull(_containerName, "containerName");
			NotNull(_configFile, "configFile");
			NotNull(_sectionName, "sectionName");
		}

		protected override OptionSet OptionSetCreater()
		{
			return new OptionSet()
			{
				{ "n=|containerName=", "key container name", v => _containerName = v },
				{ "c=|configFile=", "file name of configuration in XML format", v => _configFile = v },
				{ "s=|sectionName=", "section name", v => _sectionName = v },
			};
		}

		public override string Description
		{
			get { return "Decrypt section in the file of configurations"; }
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
			if (el == null)
				throw new ApplicationException("section not found");

			var cryptData = el["EncryptedData", "http://www.w3.org/2001/04/xmlenc#"];
			if (cryptData == null)
				throw new ApplicationException("crypt data not found");

			var decryptedData = provider.Decrypt(cryptData);
			decryptedData = doc.ImportNode(decryptedData, true);
			doc.DocumentElement.ReplaceChild(decryptedData, el);

			doc.Save(_configFile);
		}
	}
}
