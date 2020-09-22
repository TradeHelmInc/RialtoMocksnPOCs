using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KCXToRialtoEncryption
{
    class Program
    {
        protected static void GenerateKeyPair()
        {
            //Generate a public/private key pair.
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            //Save the public key information to an RSAParameters structure.
            RSAParameters RSAKeyInfo = RSA.ExportParameters(true);
            //public key    
            string pubKey = Convert.ToBase64String(RSAKeyInfo.Exponent);
            // private key  
            string privKey = Convert.ToBase64String(RSAKeyInfo.D);
        
        }


        static void Main(string[] args)
        {
            GenerateKeyPair();
            Console.ReadKey();
        }
    }
}
