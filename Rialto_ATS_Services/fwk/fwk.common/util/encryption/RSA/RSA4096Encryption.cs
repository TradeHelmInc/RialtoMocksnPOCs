using System;
using System.IO;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace fwk.common.util.encryption.RSA
{
    public class RSA4096Encryption
    {
        #region Protected Methods
        //https://dotnetfiddle.net/rHLlsq
        protected static void RSAPrivateKey4096BitsExample()
        {
            string privateKey =
        "-----BEGIN RSA PRIVATE KEY-----\nMIIEowIBAAKCAQEAhX1Vg/AFIoFxIJX8AqTsu9eAPvbAiyD1Zl10NptNovenkzBo\nGEvjP7jnNw+m4O4oVEYGAVTFZbiMA8Ij3hEGdcL43DC8uWJ2ef3IOL3KhcJhfwi8\nIcpA8sIFoUyLKnydM2J0KtZpSkbYM+mE4DpFZIzw5r5zPqf1PtN510gebS1dv/7b\n5MWm/9I5KqERu7nBxTMZAbMUcLu3GoGN4R6NGS+LE19yXxlZnZvB1hWj0luhDSyE\nSYNEpIiFE/8/pdGuAcWfMmVjex7keMzH4F9QXRX9IxDa+Gh37sszvc5O5NaphIF4\nxdncITm+G1qG/i7yTJN1ucPU8MBq1cJWiN6mKQIDAQABAoIBAQCFIQo4GxgD4bRB\nG1PKD1FJxRJRuSUtnCEhhfJww1IaRYMKeCxYjtaEppNxhlqX2Oy/n1Y93Z1AZVy+\nItBCmBgpOdmXP8P33wrpfwBAFofz/nfdiYiW6m77rCSRSRVBuiXNKVNRpaQ8P4s/\nupSaS4MJVMasWSP2SOt9TMsmuPYfSqurApV2r7X+YCwToY0VPsVqcBlCNLI5F0oY\n6AC837eCrY30nZXCquvQzP/YXyWceRSjZ9erYGObI7dlvboiVtVWiz5hH1Le69lz\n+nCqnh335SRyffs3uBWDCqNCfLn6Hon5uDlRvUJTYdYxqf+Bbf0oTcLmdLpbh2Zo\nqODdTVABAoGBAP44lkOJavxOUpGdHVdunIsor3s09KHnv1jHVbCC5HulozRgDbgG\nIa84WaeKi7s9CdBT/OLGZs/4HNhL45ftEIkg99FpqGEXe2Yz9FPoHQLyDsySlOxB\nEfa38aFxjcElu/G0pzA0xNZDx7iF7411FZbcAo8O27Ox8S/zkpITiLsJAoGBAIZs\nd9JZ9IfLa08c6K7+i1eqB2SzLutuUIT9jufQiDLIEWGImNDYLnGSeF71X62RID2H\ngP5fvBNuCXa5fHppibcX8yKGodu60LT6oYmSDUBcCcX/leLZeqzZuWXgloxnVdxg\nu7B+kbeG8mRDAiBIT1HzwCv2VFrWJYRElB/uCrohAoGAJI5ejTelesKIfQfqwDfX\nquse0Mi8pMt+aHXBjLBFysH2xgFJ97xp+5hOzBjQHwyX7K0nIDUHc8Bp5XCEOcSN\nLCrhd+uJmuyVggzWhXpLMbE9D43EfHSe8Ktiw6RgjfWvIQKpR/VOmMEGZzJbUCwV\n/quEcq3gSea8l/ieiwLkFoECgYBWtfNZAmPlkMdo4goKj/IMm4ZnY9pZTfCsyO79\nBBxHPZ1QoA+LcFgNNOFmx7tvN9VnO1hvPgwRMIL0TdRJFnkaXV2eIOCZ39kvVRkM\np+TaZkR7r1HdYlJq24tnd0dFzIarQM7xm8OdcnQ7Tqo5bsuT3rtQ4HYrnkXXG3G9\nYw0SYQKBgG3xGqf2JTu2kS7h+nXxm/cUphp2wftgUxRzzuNGz0NtGYov7SEgj+QD\nf8FZiV6x3ztzYDUVey3dQ1RbvIlqF4czA30ji/iF0HvO0Le7WdTODWPxU8xaK6fH\n+t0B13HJXqbz+NhJG6w4P4JJIUDY6wDKwmdck9STAVNK8aW6avVa\n-----END RSA PRIVATE KEY-----\n";
            RsaPrivateCrtKeyParameters key;


            AsymmetricCipherKeyPair keyPair;

            using (var sr = new StringReader(privateKey)) //here used StringReader
            {
                var pr = new PemReader(sr);
                key = (RsaPrivateCrtKeyParameters)((AsymmetricCipherKeyPair)pr.ReadObject()).Private;
            }

            const string encryptedValue = "WnTDz6ZSTtYPwXQzRbcCbxYgV7Vronot50mMisVxtZYJQksvjj0qQdHh5koYtTTDyvIA+N0EVO+hWB1YvCUT1Ut1cgrANGIknmEDI4JLg88vlCJURS8WcU8x+YSttGIj4xjQD6ArXWbZKvUyA/g2Nwd7HWDLl6qstAShu3iwrideu4wUnJhOXnNnZRPed94fRY/15WajLg4+v2XYoE90I+lgxz3HFHStfCLPSpGdtZkPNHdHDfVX5wneCm19G9KPNQa/5JXACKJUZmi/50elsKCLRz/iukZETIA642wKirwlD1eBDpn6guWtksWkarspLsUgOX/xSFsJkotmH98OPg==";
            var encryptedData = Convert.FromBase64String(encryptedValue);
            IAsymmetricBlockCipher cipher0 = new RsaBlindedEngine();

            cipher0 = new OaepEncoding(cipher0, new Sha256Digest(), new Sha256Digest(), null);
            var cipher = new BufferedAsymmetricBlockCipher(cipher0);
            cipher.Init(false, key);
            cipher.ProcessBytes(encryptedData, 0, encryptedData.Length);
            var decryptedData = cipher.DoFinal();
            var decryptedText = Encoding.UTF8.GetString(decryptedData);
            Console.WriteLine(decryptedText);
        }

        protected static void RSAPublicKey4096BitsDecrExample()
        {
            RsaKeyPairGenerator rsaKeyPairGenerator = new RsaKeyPairGenerator();
            int keySize = 4096;//in bits
            rsaKeyPairGenerator.Init(new KeyGenerationParameters(new SecureRandom(), keySize));
            var keyPair = rsaKeyPairGenerator.GenerateKeyPair();
            var publicKey = keyPair.Public;
            var privateKey = keyPair.Private;
            var rsa = new RsaEngine();
            rsa.Init(true, privateKey);//use private key for encryption-not recommended
            string message = "Hello World";
            Console.WriteLine("Original text: {message}");
            var plainTextBytes = Encoding.UTF8.GetBytes(message);
            byte[] cipherBytes = rsa.ProcessBlock(plainTextBytes, 0, message.Length);
            Console.WriteLine("Encrypted text as a byte array:");
            Console.WriteLine(BitConverter.ToString(cipherBytes));
            rsa.Init(false, publicKey);//use public key for decryption
            byte[] decryptedData = rsa.ProcessBlock(cipherBytes, 0, cipherBytes.Length);
            string decipheredText = Encoding.UTF8.GetString(decryptedData);
            Console.WriteLine("Decrypted text: {decipheredText}");
            Console.ReadLine();
        }

        #endregion

        #region Public Methods

        public static string DecryptWithPrivate(string textToDecrypt, string privKeyPemFilePath)
        {
            RsaPrivateCrtKeyParameters key;
            string privateKey = "";

            using (var reader = File.OpenText(privKeyPemFilePath))
            {
                privateKey = reader.ReadToEnd();
                var pr = new PemReader(new StringReader(privateKey));
                key = (RsaPrivateCrtKeyParameters)pr.ReadObject();
            }


            var encryptedData = Convert.FromBase64String(textToDecrypt);
            IAsymmetricBlockCipher cipher0 = new RsaBlindedEngine();

            cipher0 = new Pkcs1Encoding(cipher0);
            var cipher = new BufferedAsymmetricBlockCipher(cipher0);
            cipher.Init(false, key);
            cipher.ProcessBytes(encryptedData, 0, encryptedData.Length);
            var decryptedData = cipher.DoFinal();
            var decryptedText = Encoding.UTF8.GetString(decryptedData);
            return decryptedText;
        }

        public static string DecryptWithPublic(string textToDecrypt, string publicKeyPemFile)
        {
            var plainTextBytes = Convert.FromBase64String(textToDecrypt);
            AsymmetricKeyParameter key;
            string publicKey = "";

            using (var reader = File.OpenText(publicKeyPemFile))
            {
                publicKey = reader.ReadToEnd();
                var pr = new PemReader(new StringReader(publicKey));
                key = (AsymmetricKeyParameter)pr.ReadObject();
            }


            Pkcs1Encoding encoding = new Pkcs1Encoding(new RsaEngine());
            encoding.Init(false, key);
            byte[] decryptedData2 = encoding.ProcessBlock(plainTextBytes, 0, plainTextBytes.Length);
            
            string decipheredText = Encoding.UTF8.GetString(decryptedData2);

            return decipheredText;
        }

        #endregion
    }
}