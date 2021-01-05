using fwk.Common.util.encryption.common;
using fwk.Common.util.encryption.TripleDES;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.KoreConX.Common.Util
{
      public class AESManager
    {

        #region Protected Attributes

        protected string AESKeyandIVEncr { get; set; }

        protected bool AESKeyIsEncrypted { get; set; }

        #endregion

        #region Constructors

        public AESManager(string pAESKeyandIVEncr, bool pAESKeyIsEncrypted)
        {
            AESKeyandIVEncr = pAESKeyandIVEncr;

            AESKeyIsEncrypted = pAESKeyIsEncrypted;
        }

        #endregion

        #region Public Methods
        public string DecryptAES(string PDToDecrypt)
        {
            RijndaelManaged AES = new RijndaelManaged();
           

            string description="";

            string keyandIV = AESKeyIsEncrypted ? DPAPI.Decrypt(AESKeyandIVEncr, null, out description) : AESKeyandIVEncr;

            //string keyandIV = AESKeyandIVEncr;
            Byte[] key = UTF8Encoding.UTF8.GetBytes(keyandIV.Substring(0, 32));
            Byte[] IV = UTF8Encoding.UTF8.GetBytes(keyandIV.Substring(32, 16));
            Byte[] inputArr = Convert.FromBase64String(PDToDecrypt);
            string decrypted = "";

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
                                decrypted = sr.ReadToEnd();
                            }
                        }
                    }
                }
            }

            return decrypted;
        }

        #endregion
    }
}
