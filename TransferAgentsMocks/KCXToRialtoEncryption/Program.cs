﻿using fwk.Common.util.encryption.RSA;
using System;
using System.Collections.Generic;
using System.Configuration;
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

        protected static string EncryptedFile { get; set; }

        protected static string TextToEncrypt { get; set; }

        protected static string Mode { get; set; }


        protected static void GenerateKeyPair()
        {
            const int PROVIDER_RSA_FULL = 1;
            const string CONTAINER_NAME = "KeyContainer";
            CspParameters cspParams;
            cspParams = new CspParameters(PROVIDER_RSA_FULL);
            cspParams.KeyContainerName = CONTAINER_NAME;
            cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
            cspParams.ProviderName = "Microsoft Strong Cryptographic Provider";
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cspParams);

            //Pair of public and private key as XML string.
            //Do not share this to other party
            

            //Public Key to file in Pem format
            string publicOnlyKeyXML = rsa.ToXmlString(false);
            string pemPublic = PemLoader.PublicXmlToPem(rsa);
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
            string xmlPubKey = encrypter.GetXmlPubKey(PublicPemFile);
            RSACryptoServiceProvider rsaEncrypter = new RSACryptoServiceProvider();
            rsaEncrypter.FromXmlString(xmlPubKey);
            Console.WriteLine(string.Format("Xml Public Key Received: {0}", xmlPubKey));
            Console.WriteLine("");

            //we retrieve the encrypted text that we received
            string encryptedText = PemLoader.GetFileContent(EncryptedFile);
            var encrArr = Convert.FromBase64String(encryptedText);
            Console.WriteLine(string.Format("Retrieving encrypted text: {0}", encryptedText));
            Console.WriteLine("");
            
            //we retrieve the private key from xml file
            Console.WriteLine(string.Format("Retrieving private key from file: {0}", PrivateKeyXmlFile));
            string privKeyXml = PemLoader.GetFileContent(PrivateKeyXmlFile);
            RSACryptoServiceProvider rsaDecrypter = new RSACryptoServiceProvider();
            rsaDecrypter.FromXmlString(privKeyXml);
            Console.WriteLine(string.Format("Xml Private Key Loaded: {0}", privKeyXml));
            Console.WriteLine("");


            //We decrypt the text
            byte[] decrArr = rsaDecrypter.Decrypt(encrArr, RSAEncryptionPadding.Pkcs1);
            string strDecripted = Encoding.UTF8.GetString(decrArr);

            Console.WriteLine(string.Format("The decrypted string is : {0}", strDecripted));

        }

      
        static void Main(string[] args)
        {
            PublicPemFile = ConfigurationManager.AppSettings["PublicKeyFile"];
            PrivateKeyXmlFile = ConfigurationManager.AppSettings["PrivateKeyFile"];
            EncryptedFile = ConfigurationManager.AppSettings["EncryptedFile"];
            TextToEncrypt = ConfigurationManager.AppSettings["TextToEncrypt"];
            Mode = ConfigurationManager.AppSettings["Mode"];

            if (Mode=="1")
                GenerateKeyPair();
            
            if (Mode=="2")
                DecryptTest();
           
            Console.ReadKey();
        }
    }
}
