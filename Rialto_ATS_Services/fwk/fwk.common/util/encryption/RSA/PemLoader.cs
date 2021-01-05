using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using fwk.Common.util.encryption.common;

namespace fwk.Common.util.encryption.RSA
{
    public class PemLoader
    {

        #region Private Static Methods

        //This is the C# way to get a StreamReader from publickey in format PEM
        //See file publicKey.pem to see the Pem format
        private static StreamReader GetStreamReaderFromPublicKey(string pemPublicKey)
        {

            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            StreamReader sr = new StreamReader(ms);
            sw.Write(pemPublicKey);
            sw.Flush();
            ms.Position = 0;

            return sr;
        }


        #endregion

        #region Public Static Methods

        public static string GetPublicKeyFromPemFile(string pemPath)
        {
            return FileLoader.GetFileContent(pemPath);

        }

        // We use BouncyCastle library which is available in C++
        //https://www.findbestopensource.com/product/bouncycastlepp
        //This is what is recommended in the following link: http://superdry.apphb.com/tools/online-rsa-key-converter
        public static string GetXMLKeyFromPemKey(string pemPublicKey)
        {

            StreamReader reader = GetStreamReaderFromPublicKey(pemPublicKey);

            Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters keyPair = (Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters)new Org.BouncyCastle.OpenSsl.PemReader(reader).ReadObject();

            System.Security.Cryptography.RSA rsa = Org.BouncyCastle.Security.DotNetUtilities.ToRSA(keyPair);

            string xml = rsa.ToXmlString(false);

            return xml;

        }


        //// We use BouncyCastle library which is available in C++
        ////https://www.findbestopensource.com/product/bouncycastlepp
        ////This is what is recommended in the following link: http://superdry.apphb.com/tools/online-rsa-key-converter
        public static string GetXMLPrivateKeyFromPemPrivateKey(string pemPrivateKey)
        {

            StreamReader reader = GetStreamReaderFromPublicKey(pemPrivateKey);

            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair keyPair = (Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair)new Org.BouncyCastle.OpenSsl.PemReader(reader).ReadObject();

            AsymmetricKeyParameter privateKey = keyPair.Private;
            var rsa = DotNetUtilities.ToRSA((RsaPrivateCrtKeyParameters)privateKey);
            string xmlRsa = rsa.ToXmlString(true);
            return xmlRsa;
        }

        public static string PublicXmlToPem(RSACryptoServiceProvider rsa)
        {
            RsaKeyParameters publicKey = Org.BouncyCastle.Security.DotNetUtilities.GetRsaPublicKey(rsa); // try get public key
            if (publicKey != null) // if XML RSA key contains public key
            {
                SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);
                return FormatPem(Convert.ToBase64String(publicKeyInfo.GetEncoded()), "PUBLIC KEY");
            }

            throw new InvalidKeyException("Invalid RSA Xml Key");
        }


        private static string FormatPem(string pem, string keyType)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("-----BEGIN {0}-----", keyType);

            int line = 1, width = 64;

            while ((line - 1) * width < pem.Length)
            {
                int startIndex = (line - 1) * width;
                int len = line * width > pem.Length
                              ? pem.Length - startIndex
                              : width;
                sb.AppendFormat("{0}", pem.Substring(startIndex, len));
                line++;
            }

            sb.AppendFormat("-----END {0}-----", keyType);
            return sb.ToString();
        }

        public static void WritePublicToPem(RSACryptoServiceProvider rsa,string file)
        {
            RsaKeyParameters keys = Org.BouncyCastle.Security.DotNetUtilities.GetRsaPublicKey(rsa);

            TextWriter textWriter = new StreamWriter(file);
            PemWriter pemWriter = new PemWriter(textWriter);
            pemWriter.WriteObject(keys);
            pemWriter.Writer.Flush();

            textWriter.Close();
        }


        public static void WriteToFile(string xml, string file)
        {
            TextWriter textWriter = new StreamWriter(file);

            textWriter.WriteLine(xml);

            textWriter.Close();
        }



        #endregion

    }
}
