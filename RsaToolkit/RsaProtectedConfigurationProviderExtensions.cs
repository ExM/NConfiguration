using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace RsaToolkit
{
	public static class RsaProtectedConfigurationProviderExtensions
	{
		private static readonly string _KeyName = "Rsa Key";

		public static XmlNode DecryptFixup(this RsaProtectedConfigurationProvider provider, XmlNode encryptedNode)
		{
			XmlDocument xmlDocument = new XmlDocument();
			RSACryptoServiceProvider cryptoServiceProvider = provider.GetCryptoServiceProviderFixup(false, true);
			xmlDocument.PreserveWhitespace = true;
			LoadXml(xmlDocument, encryptedNode.OuterXml);
			EncryptedXml encryptedXml = (EncryptedXml) new FipsAwareEncryptedXml(xmlDocument);
			encryptedXml.AddKeyNameMapping(_KeyName, (object) cryptoServiceProvider);
			encryptedXml.DecryptDocument();
			cryptoServiceProvider.Clear();
			return (XmlNode) xmlDocument.DocumentElement;
		}

		public static XmlNode EncryptFixup(this RsaProtectedConfigurationProvider provider, XmlNode node)
		{
			RSACryptoServiceProvider cryptoServiceProvider = provider.GetCryptoServiceProviderFixup(false, false);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.PreserveWhitespace = true;
			LoadXml(xmlDocument, "<foo>" + node.OuterXml + "</foo>");
			EncryptedXml encryptedXml = new EncryptedXml(xmlDocument);
			XmlElement documentElement = xmlDocument.DocumentElement;
			byte[] numArray;
			EncryptedData encryptedData;
			EncryptedKey encryptedKey;
			using (SymmetricAlgorithm algorithmProvider = provider.GetSymAlgorithmProviderFixUp())
			{
				numArray = encryptedXml.EncryptData(documentElement, algorithmProvider, true);
				encryptedData = new EncryptedData();
				encryptedData.Type = "http://www.w3.org/2001/04/xmlenc#Element";
				encryptedData.EncryptionMethod = provider.GetSymEncryptionMethodFixUp();
				encryptedData.KeyInfo = new KeyInfo();
				encryptedKey = new EncryptedKey();
				encryptedKey.EncryptionMethod = new EncryptionMethod("http://www.w3.org/2001/04/xmlenc#rsa-1_5");
				encryptedKey.KeyInfo = new KeyInfo();
				encryptedKey.CipherData = new CipherData();
				encryptedKey.CipherData.CipherValue = EncryptedXml.EncryptKey(algorithmProvider.Key,
					(RSA) cryptoServiceProvider, provider.UseOAEP);
			}

			encryptedKey.KeyInfo.AddClause((KeyInfoClause) new KeyInfoName()
			{
				Value = _KeyName
			});
			KeyInfoEncryptedKey infoEncryptedKey = new KeyInfoEncryptedKey(encryptedKey);
			encryptedData.KeyInfo.AddClause((KeyInfoClause) infoEncryptedKey);
			encryptedData.CipherData = new CipherData();
			encryptedData.CipherData.CipherValue = numArray;
			EncryptedXml.ReplaceElement(documentElement, encryptedData, true);
			cryptoServiceProvider.Clear();
			foreach (XmlNode childNode1 in xmlDocument.ChildNodes)
			{
				if (childNode1.NodeType == XmlNodeType.Element)
				{
					foreach (XmlNode childNode2 in childNode1.ChildNodes)
					{
						if (childNode2.NodeType == XmlNodeType.Element)
							return childNode2;
					}
				}
			}

			return (XmlNode) null;
		}

		private static void LoadXml(XmlDocument xmlDoc, string xmlText)
		{
			using (XmlTextReader xmlTextReader = new XmlTextReader((TextReader) new StringReader(xmlText)))
			{
				xmlTextReader.DtdProcessing = DtdProcessing.Ignore;
				xmlDoc.Load((XmlReader) xmlTextReader);
			}
		}

		private static byte[] GetRandomKey()
		{
			byte[] data = new byte[24];
			new RNGCryptoServiceProvider().GetBytes(data);
			return data;
		}

		private static EncryptionMethod GetSymEncryptionMethodFixUp(this RsaProtectedConfigurationProvider provider)
		{
			if (!provider.UseFIPS)
				return new EncryptionMethod("http://www.w3.org/2001/04/xmlenc#tripledes-cbc");
			return new EncryptionMethod("http://www.w3.org/2001/04/xmlenc#aes256-cbc");
		}

		private static SymmetricAlgorithm GetSymAlgorithmProviderFixUp(this RsaProtectedConfigurationProvider provider)
		{
			SymmetricAlgorithm symmetricAlgorithm;
			if (provider.UseFIPS)
			{
				symmetricAlgorithm = (SymmetricAlgorithm) new AesCryptoServiceProvider();
			}
			else
			{
				symmetricAlgorithm = (SymmetricAlgorithm) new TripleDESCryptoServiceProvider();
				byte[] randomKey = GetRandomKey();
				symmetricAlgorithm.Key = randomKey;
				symmetricAlgorithm.Mode = CipherMode.ECB;
				symmetricAlgorithm.Padding = PaddingMode.PKCS7;
			}

			return symmetricAlgorithm;
		}

		private static RSACryptoServiceProvider GetCryptoServiceProviderFixup(
			this RsaProtectedConfigurationProvider provider,
			bool exportable,
			bool keyMustExist)
		{
			CspParameters parameters = new CspParameters();
			parameters.KeyContainerName = provider.KeyContainerName;
			parameters.KeyNumber = 1;
			parameters.ProviderType = 1;
			if (provider.CspProviderName != null && provider.CspProviderName.Length > 0)
				parameters.ProviderName = provider.CspProviderName;
			if (provider.UseMachineContainer)
				parameters.Flags |= CspProviderFlags.UseMachineKeyStore;
			if (!exportable && !keyMustExist)
				parameters.Flags |= CspProviderFlags.UseNonExportableKey;
			if (keyMustExist)
				parameters.Flags |= CspProviderFlags.UseExistingKey;
			return new RSACryptoServiceProvider(parameters);
		}
		
		internal class FipsAwareEncryptedXml : EncryptedXml
		{
			public FipsAwareEncryptedXml(XmlDocument doc)
				: base(doc)
			{
			}

			public override SymmetricAlgorithm GetDecryptionKey(
				EncryptedData encryptedData,
				string symmetricAlgorithmUri)
			{
				if (FipsAwareEncryptedXml.IsAesDetected(encryptedData, symmetricAlgorithmUri))
				{
					EncryptedKey encryptedKey = (EncryptedKey) null;
					foreach (object obj in encryptedData.KeyInfo)
					{
						KeyInfoEncryptedKey infoEncryptedKey = obj as KeyInfoEncryptedKey;
						if (infoEncryptedKey != null)
						{
							encryptedKey = infoEncryptedKey.EncryptedKey;
							break;
						}
					}
					if (encryptedKey != null)
					{
						byte[] numArray = this.DecryptEncryptedKey(encryptedKey);
						if (numArray != null)
						{
							AesCryptoServiceProvider cryptoServiceProvider = new AesCryptoServiceProvider();
							cryptoServiceProvider.Key = numArray;
							return (SymmetricAlgorithm) cryptoServiceProvider;
						}
					}
				}
				return base.GetDecryptionKey(encryptedData, symmetricAlgorithmUri);
			}

			private static bool IsAesDetected(EncryptedData encryptedData, string symmetricAlgorithmUri)
			{
				if (encryptedData == null || encryptedData.KeyInfo == null || symmetricAlgorithmUri == null && encryptedData.EncryptionMethod == null)
					return false;
				if (symmetricAlgorithmUri == null)
					symmetricAlgorithmUri = encryptedData.EncryptionMethod.KeyAlgorithm;
				return string.Equals(symmetricAlgorithmUri, "http://www.w3.org/2001/04/xmlenc#aes256-cbc", StringComparison.InvariantCultureIgnoreCase);
			}
		}
	}
}