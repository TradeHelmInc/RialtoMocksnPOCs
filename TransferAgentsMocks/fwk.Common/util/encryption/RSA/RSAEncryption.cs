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

        #endregion

        #region Constructors 

        public RSAEncryption()
        {
            RSACryptoServiceProvider = new RSACryptoServiceProvider();
        
        }

        #endregion

        #region Public Methods

        public string GetXmlPubKey(string file)
        {
            string pemPublicKey = PemLoader.GetPublicKeyFromPemFile(file);

            string xmlPublicKey = PemLoader.GetXMLPublicKeyFromPemPublicKey(pemPublicKey);
            
            return xmlPublicKey;
        
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


        public string DecryptFromStr(string txtEncr)
        {

            byte[] encrArr =Convert.FromBase64String(txtEncr);

            byte[] decrypted = RSACryptoServiceProvider.Decrypt(encrArr,RSAEncryptionPadding.Pkcs1);

            var base64Encrypted = Convert.ToBase64String(decrypted);

            return base64Encrypted;

        }


        #endregion
    }
}
