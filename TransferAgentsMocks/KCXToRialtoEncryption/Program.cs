using fwk.Common.util.encryption.common;
using fwk.Common.util.encryption.RSA;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KCXToRialtoEncryption
{
    class Program
    {

        protected static string PublicPemFile { get; set; }

        protected static string PrivateKeyXmlFile { get; set; }

        protected static string PublicKeyXmlFile { get; set; }

        protected static string EncryptedFile { get; set; }

        protected static string AESKeyandIV { get; set; }

        protected static string TextToEncrypt { get; set; }

        protected static string Mode { get; set; }

        protected static string PDToDecrypt { get; set; }


        protected static void GenerateKeyPair()
        {
            const int PROVIDER_RSA_FULL = 1;
            const string CONTAINER_NAME = "KeyContainer";
            CspParameters cspParams;
            cspParams = new CspParameters(PROVIDER_RSA_FULL);
            cspParams.KeyContainerName = CONTAINER_NAME;
            cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
            cspParams.ProviderName = "Microsoft Strong Cryptographic Provider";
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(4096,cspParams);

            //Pair of public and private key as XML string.
            //Do not share this to other party
            

            //Public Key to file in Pem format
            string publicOnlyKeyXML = rsa.ToXmlString(false);
            string pemPublic = PemLoader.PublicXmlToPem(rsa);
            PemLoader.WriteToFile(publicOnlyKeyXML, PublicKeyXmlFile);
            PemLoader.WritePublicToPem(rsa, PublicPemFile);
            Console.WriteLine(string.Format("Public Key generated and saved: {0}", pemPublic));
            Console.WriteLine("");

            //Private key to file in XML format
            string xmlPrivKey = rsa.ToXmlString(true);
            PemLoader.WriteToFile(xmlPrivKey, PrivateKeyXmlFile);
            Console.WriteLine(string.Format("Private Key generated and saved: {0}", xmlPrivKey));
            Console.WriteLine("");

            //Saving the encryption
            var txtArr = Encoding.UTF8.GetBytes(TextToEncrypt);
            byte[] encrArr = rsa.Encrypt(txtArr, RSAEncryptionPadding.Pkcs1);
            byte[] decrArr = rsa.Decrypt(encrArr, RSAEncryptionPadding.Pkcs1);
            string strEncrypted = Convert.ToBase64String(encrArr);

            PemLoader.WriteToFile(strEncrypted, EncryptedFile);
            Console.WriteLine(string.Format("Text \"{0}\" encrypted and saved: {1}", TextToEncrypt,strEncrypted));
            Console.WriteLine("");

            string strDecrypted = Encoding.UTF8.GetString(decrArr);

            if (strDecrypted == TextToEncrypt)
                Console.WriteLine("ENCRYPTION TEST SUCESSFULL!");
            else
                Console.WriteLine("ENCRYPTION TEST NOT SUCESSFULL!");
        }

        protected static void DecryptTest()
        {
            RSAEncryption encrypter = new RSAEncryption();

            //We retrieve the public key from pem file
            Console.WriteLine(string.Format("Retrieving public key from file: {0}", PublicPemFile));
            string xmlPubKey = encrypter.GetXmlFromPemKey(PublicPemFile);
            RSACryptoServiceProvider rsaEncrypter = new RSACryptoServiceProvider();
            rsaEncrypter.FromXmlString(xmlPubKey);
            Console.WriteLine(string.Format("Xml Public Key Received: {0}", xmlPubKey));
            Console.WriteLine("");

            //we retrieve the encrypted text that we received
            string encryptedText = FileLoader.GetFileContent(EncryptedFile);
            var encrArr = Convert.FromBase64String(encryptedText);
            Console.WriteLine(string.Format("Retrieving encrypted text: {0}", encryptedText));
            Console.WriteLine("");
            
            //we retrieve the private key from xml file
            Console.WriteLine(string.Format("Retrieving private key from file: {0}", PrivateKeyXmlFile));
            string privKeyXml = FileLoader.GetFileContent(PrivateKeyXmlFile);
            RSACryptoServiceProvider rsaDecrypter = new RSACryptoServiceProvider();
            rsaDecrypter.FromXmlString(privKeyXml);
            Console.WriteLine(string.Format("Xml Private Key Loaded: {0}", privKeyXml));
            Console.WriteLine("");


            //We decrypt the text
            byte[] decrArr = rsaDecrypter.Decrypt(encrArr, RSAEncryptionPadding.Pkcs1);
            string strDecripted = Encoding.UTF8.GetString(decrArr);

            Console.WriteLine(string.Format("The decrypted string is : {0}", strDecripted));

        }


        protected static void DecryptAES()
        {
            RijndaelManaged AES = new RijndaelManaged();

            string keyandIV = FileLoader.GetFileContent(AESKeyandIV);
            Byte[] keyAndIvBytes = UTF8Encoding.UTF8.GetBytes(FileLoader.GetFileContent(AESKeyandIV));
            Byte[] key = UTF8Encoding.UTF8.GetBytes(keyandIV.Substring(0, 32));
            Byte[] IV = UTF8Encoding.UTF8.GetBytes(keyandIV.Substring(32, 16));
            Byte[] inputArr = Convert.FromBase64String(PDToDecrypt);
            string decrypted = "";

            Console.WriteLine(string.Format("Decrypting PD data field {0}", PDToDecrypt));
            Console.WriteLine("");
            using (var provider = new AesCryptoServiceProvider())
            {
                provider.Key = key;
                using (var ms = new MemoryStream(inputArr))
                {
                    // Read the first 16 bytes which is the IV.
                    //byte[] iv = new byte[16];
                    //ms.Read(iv, 0, 16);
                    provider.IV = IV;

                    using (var decryptor = provider.CreateDecryptor())
                    {
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (var sr = new StreamReader(cs))
                            {
                                decrypted=  sr.ReadToEnd();
                            }
                        }
                    }
                }
            }

            Console.WriteLine(string.Format("Decripted PD:{0}",decrypted));
            Console.WriteLine("");
            
        }

      
        static void Main(string[] args)
        {
            PublicPemFile = ConfigurationManager.AppSettings["PublicKeyPmlFile"];
            PrivateKeyXmlFile = ConfigurationManager.AppSettings["PrivateKeyFile"];
            PublicKeyXmlFile = ConfigurationManager.AppSettings["PublicKeyXmlFile"];
            EncryptedFile = ConfigurationManager.AppSettings["EncryptedFile"];
            TextToEncrypt = ConfigurationManager.AppSettings["TextToEncrypt"];
            AESKeyandIV=ConfigurationManager.AppSettings["AESKeyandIV"];
            Mode = ConfigurationManager.AppSettings["Mode"];
            PDToDecrypt = ConfigurationManager.AppSettings["PDToDecrypt"];

            if (Mode=="1")
                GenerateKeyPair();
            
            if (Mode=="2")
                DecryptTest();

            if (Mode == "3")
                DecryptAES();
           
            Console.ReadKey();
        }
    }
}
