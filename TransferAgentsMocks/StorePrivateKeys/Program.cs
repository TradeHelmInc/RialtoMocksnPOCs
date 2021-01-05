using fwk.Common.util.encryption.common;
using fwk.Common.util.encryption.TripleDES;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StorePrivateKeys
{
    class Program
    {



        //https://freesilo.com/?p=452
        static void Main(string[] args)
        {
            string kcxKeyAndIVPath = ConfigurationManager.AppSettings["KCXKeyAndIVPath"];

            try
            {
                string kcxKeyAndIV = FileLoader.GetFileContent(kcxKeyAndIVPath);
                //string text = "Hello, world!";
                string entropy = null;
                string description;

                Console.WriteLine("Plaintext: {0}\r\n", kcxKeyAndIV);

                // Call DPAPI to encrypt data with user-specific key.
                string encrypted = DPAPI.Encrypt(DPAPI.KeyType.UserKey,
                                                  kcxKeyAndIV,
                                                  entropy,
                                                  "My Data");
                Console.WriteLine("Encrypted: {0}\r\n", encrypted);

                // Call DPAPI to decrypt data.
                string decrypted = DPAPI.Decrypt(encrypted,
                                                    entropy,
                                                out description);
                Console.WriteLine("Decrypted: {0} <<<{1}>>>\r\n",
                                   decrypted, description);

                Console.ReadKey();
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    Console.WriteLine(ex.Message);
                    ex = ex.InnerException;
                }
            }
        }
    }
}
