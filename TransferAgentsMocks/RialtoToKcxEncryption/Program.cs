using fwk.Common.util.encryption.common;
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

            Logger=new Logger();

            try
            {
                RSAEncryption encrypter = new RSAEncryption();

                Logger.DoLog(string.Format("Loading public key file in path {0}",pubKeyFile),fwk.Common.enums.MessageType.Information);

                string xmlPubKey = encrypter.GetXmlPubKey(pubKeyFile);

                Logger.DoLog(string.Format("Encrypting text = {0}", textToEncrypt), fwk.Common.enums.MessageType.Information);

                string output = encrypter.EncryptToStr(xmlPubKey, textToEncrypt);

                Logger.DoLog(string.Format("Text encrypted = {0}", output), fwk.Common.enums.MessageType.Information);

                //Logger.DoLog(string.Format(" Decrypting roundtrip..."), fwk.Common.enums.MessageType.Information);

                //encrypter.DecryptFromStr(output);

            }
            catch (Exception ex)
            {
                Logger.DoLog(string.Format("Critial error encrypting text = {0}", textToEncrypt), fwk.Common.enums.MessageType.Error);
            }

            Console.ReadKey();
        }
    }
}
