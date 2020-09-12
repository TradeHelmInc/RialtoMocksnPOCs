using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fwk.Common.util.encryption.RSA
{
    public class PemLoader
    {

        #region Private Static Methods

        //READ THIS FILE with WHATEVER READER METHOD YOU HAVE IN C++
        private static string GetFileContent(string file)
        {
            string result = "";
            using (StreamReader streamReader = new StreamReader(file))
            {
                result = streamReader.ReadToEnd();
            }

            return result;

        }

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
            return GetFileContent(pemPath);

        }

        // We use BouncyCastle library which is available in C++
        //https://www.findbestopensource.com/product/bouncycastlepp
        //This is what is recommended in the following link: http://superdry.apphb.com/tools/online-rsa-key-converter
        public static string GetXMLPublicKeyFromPemPublicKey(string pemPublicKey)
        {

            StreamReader reader = GetStreamReaderFromPublicKey(pemPublicKey);

            Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters keyPair = (Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters)new Org.BouncyCastle.OpenSsl.PemReader(reader).ReadObject();

            System.Security.Cryptography.RSA rsa = Org.BouncyCastle.Security.DotNetUtilities.ToRSA(keyPair);

            string xml = rsa.ToXmlString(false);

            return xml;

        }


        #endregion

    }
}
