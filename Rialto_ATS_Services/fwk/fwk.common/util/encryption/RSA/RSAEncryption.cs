using fwk.Common.util.encryption.common;
using fwk.Common.util.encryption.TripleDES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace fwk.Common.util.encryption.RSA
{
    public class RSAEncryption
    {

        #region Protected Attributes

        protected RSACryptoServiceProvider RSACryptoServiceProvider { get; set; }

        protected bool IsEncrypted { get; set; }

        protected string PrivateKeyFilePath { get; set; }

        #endregion

        #region Constructors 

        public RSAEncryption(string pPrivateKeyFilePaht=null,bool isEncrypted=false)
        {
            RSACryptoServiceProvider = new RSACryptoServiceProvider();

            IsEncrypted = isEncrypted;

            PrivateKeyFilePath = pPrivateKeyFilePaht;
        
        }
        
        public RSAEncryption()
        {
            RSACryptoServiceProvider = new RSACryptoServiceProvider();

            IsEncrypted = false;

            PrivateKeyFilePath = null;
        
        }

        #endregion

        #region Public Methods

        public string GetXmlFromPemKey(string file)
        {
            string pemKey = null;

            if (IsEncrypted)
            {
                string description="";

                pemKey = DPAPI.Decrypt(FileLoader.GetFileContent(file), null, out description);
            }
            else
            {
                pemKey = PemLoader.GetPublicKeyFromPemFile(file);
            }

            string xmlKey = PemLoader.GetXMLKeyFromPemKey(pemKey);
            
            return xmlKey;
        
        }



        public byte[] Encrypt(string xmlPubKey,string txt)
        {
            RSACryptoServiceProvider.FromXmlString(xmlPubKey);

            var txtArr = Encoding.UTF8.GetBytes(txt);

            return RSACryptoServiceProvider.Encrypt(txtArr, RSAEncryptionPadding.Pkcs1);

        }

        public string EncryptToStr(string xmlPubKey, string txt)
        {
            byte[] encrypted = Encrypt(xmlPubKey, txt);

            var base64Encrypted = Convert.ToBase64String(encrypted);

            return base64Encrypted;

        }

        private string DoDecrypt(string privKeyXml, string encryptedText)
        {

            RSACryptoServiceProvider rsaDecrypter = new RSACryptoServiceProvider();
            rsaDecrypter.FromXmlString(privKeyXml);

            var encrArr = Convert.FromBase64String(encryptedText);

            byte[] decrArr = rsaDecrypter.Decrypt(encrArr, RSAEncryptionPadding.Pkcs1);
            string strDecripted = Encoding.UTF8.GetString(decrArr);

            return strDecripted;
        }


        public string Decrypt(string encryptedText)
        {
            string privKeyXml = "";
            if (IsEncrypted)
            {
                string description = "";
                privKeyXml = DPAPI.Decrypt(FileLoader.GetFileContent(PrivateKeyFilePath), null, out description);
            }
            else
            {
                privKeyXml = FileLoader.GetFileContent(PrivateKeyFilePath);
            }


            return DoDecrypt(privKeyXml, encryptedText);
        }


        //public string DecryptFromStr(string txtEncr)
        //{

        //    byte[] encrArr =Convert.FromBase64String(txtEncr);

        //    byte[] decrypted = RSACryptoServiceProvider.Decrypt(encrArr,RSAEncryptionPadding.Pkcs1);

        //    var base64Encrypted = Convert.ToBase64String(decrypted);

        //    return base64Encrypted;

        //}

        //private byte[] RSADecrypt(byte[] DataToDecrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        //{
        //    try
        //    {
        //        byte[] decryptedData;
        //        using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
        //        {
        //            RSA.ImportParameters(RSAKeyInfo);
        //            decryptedData = RSA.Decrypt(DataToDecrypt, DoOAEPPadding);
        //        }
        //        return decryptedData;
        //    }
        //    catch (CryptographicException e)
        //    {
        //        Console.WriteLine(e.ToString());

        //        return null;
        //    }

        //}

        //private byte[] RSAEncrypt(byte[] DataToEncrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        //{
        //    try
        //    {
        //        byte[] encryptedData;
        //        using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
        //        {
        //            RSA.ImportParameters(RSAKeyInfo);
        //            encryptedData = RSA.Encrypt(DataToEncrypt, DoOAEPPadding);
        //        }
        //        return encryptedData;
        //    }
        //    catch (CryptographicException e)
        //    {
        //        Console.WriteLine(e.Message);

        //        return null;
        //    }

        //}

        //private  string Base64Encode(string plainText)
        //{
        //    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        //    return System.Convert.ToBase64String(plainTextBytes);
        //}

        //// We use BouncyCastle library which is available in C++
        ////https://www.findbestopensource.com/product/bouncycastlepp
        ////This is what is recommended in the following link: http://superdry.apphb.com/tools/online-rsa-key-converter
        //public  string DecryptFromStr(string pemPrivateKey, string pemPublicKey, string msgEncr)
        //{
        //    UnicodeEncoding ByteConverter = new UnicodeEncoding();
        //    var pubKeyArr = Convert.FromBase64String("MCgCIQCfkl4xV5T/v3r1bifOc1mVHa9yak5pGjUfAv0r+s6+AwIDAQAB");
        //    var prvKeyArr = Convert.FromBase64String("MIGsAgEAAiEAn5JeMVeU/7969W4nznNZlR2vcmpOaRo1HwL9K/rOvgMCAwEAAQIg\nMce6pM/6xpIYrMoxluE7JBkVe9Sme9d6NPPJJX3NyBECEgCmwIarl1hSBnTqZNwJ\n8hZhqwIQAPT6CO/l/ma1sDi7eM7tCQISAKH90lYLlr9IinfSN3hp95g1AhAAlyNf\nuioqX1G+y/GVogyJAhEmQQB52juSQ574HnampzXUpQ==");


        //    //var pubKeyArr = Convert.FromBase64String(Base64Encode(pemPublicKey));
        //    //var prvKeyArr = Convert.FromBase64String(Base64Encode(pemPrivateKey));
        //    var expArr = ByteConverter.GetBytes("65537");
        //    //var toDecr = Convert.FromBase64String(Base64Encode(msgEncr));


        //    using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
        //    {
        //        RSAParameters myRSAParameters = RSA.ExportParameters(false);
        //        myRSAParameters.Modulus = pubKeyArr;
        //        myRSAParameters.Exponent = expArr;
        //        myRSAParameters.D = prvKeyArr;

        //        byte[] dataToEncrypt = ByteConverter.GetBytes("Data to Encrypt");

        //        byte[] encrData = RSAEncrypt(dataToEncrypt, myRSAParameters, false);

        //        byte[] decryptedData = RSADecrypt(encrData, myRSAParameters, false);

        //        string decrptedStr = ByteConverter.GetString(decryptedData);

        //        return decrptedStr;
        //    }

        //}


        #endregion
    }
}
