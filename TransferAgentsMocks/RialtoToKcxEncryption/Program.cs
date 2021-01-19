using fwk.Common.util.encryption.common;
using fwk.Common.util.encryption.RSA;
using fwk.Common.util.logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RialtoToKcxEncryption
{
    public class Program
    {
        #region Protected Static Attributes

        public static  Logger Logger { get; set; }

        #endregion


        static void Main(string[] args)
        {
            string pubKeyFile = ConfigurationManager.AppSettings["KCXPublicKeyPath"];
            string textToEncrypt = ConfigurationManager.AppSettings["TextToEncrypt"];

            string fileWithTextToEncrypt = ConfigurationManager.AppSettings["FileWithTextToEncrypt"];

            if(!string.IsNullOrEmpty(fileWithTextToEncrypt))
                textToEncrypt = FileLoader.GetFileContent(fileWithTextToEncrypt);

            Logger=new Logger();

            try
            {
                RSAEncryption encrypter = new RSAEncryption();

                Logger.DoLog(string.Format("Loading public key file in path {0}",pubKeyFile),fwk.Common.enums.MessageType.Information);

                //string xmlPubKey = encrypter.GetXmlFromPemKey(pubKeyFile);
                string xmlPubKey = encrypter.GetXmlFromPemKeyFile(pubKeyFile,KeyStrength.s4096);

                Logger.DoLog(string.Format("Encrypting text = {0}", textToEncrypt), fwk.Common.enums.MessageType.Information);

                string output = encrypter.EncryptToStr(xmlPubKey, textToEncrypt);

                Logger.DoLog(string.Format("Text encrypted = {0}", output), fwk.Common.enums.MessageType.Information);

                //Logger.DoLog(string.Format(" Decrypting roundtrip..."), fwk.Common.enums.MessageType.Information);

                //encrypter.DecryptFromStr(output);


                ////we retrieve the private key from xml file
                //Console.WriteLine(string.Format("Retrieving private key from file: {0}", ConfigurationManager.AppSettings["KCXPrivateKeyPath"]));
                //string privKeyXml = FileLoader.GetFileContent(ConfigurationManager.AppSettings["KCXPrivateKeyPath"]);
                //RSACryptoServiceProvider rsaDecrypter = new RSACryptoServiceProvider();
                //rsaDecrypter.FromXmlString(privKeyXml);
                //Console.WriteLine(string.Format("Xml Private Key Loaded: {0}", privKeyXml));
                //Console.WriteLine("");


                ////We decrypt the text
                //var encrArr = Convert.FromBase64String(output);
                //byte[] decrArr = rsaDecrypter.Decrypt(encrArr, RSAEncryptionPadding.Pkcs1);
                //string strDecripted = Encoding.UTF8.GetString(decrArr);


            }
            catch (Exception ex)
            {
                Logger.DoLog(string.Format("Critial error encrypting text = {0}", textToEncrypt), fwk.Common.enums.MessageType.Error);
            }

            Console.ReadKey();
        }
    }
}
