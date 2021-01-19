using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using fwk.Common.util.encryption.common;
using System.Security.Cryptography.X509Certificates;

namespace fwk.Common.util.encryption.RSA
{
    public class PemLoader
    {

        #region Private Static Methods

        //This is the C# way to get a StreamReader from publickey in format PEM
        //See file publicKey.pem to see the Pem format
        private static StreamReader GetStreamReaderFromPublicKey(string pemPublicKey)
        {

            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            StreamReader sr = new StreamReader(ms);
            sw.Write(pemPublicKey);
            sw.Flush();
            ms.Position = 0;

            return sr;
        }


        #endregion

        #region Public Static Methods

        public static string GetPublicKeyFromPemFile(string pemPath)
        {
            return FileLoader.GetFileContent(pemPath);

        }

        private static bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }

        public static RSACryptoServiceProvider DecodeX509PublicKey(byte[] x509key)
        {
            // encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
            byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] seq = new byte[15];
            // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
            MemoryStream mem = new MemoryStream(x509key);
            BinaryReader binr = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;

            try
            {

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();   //advance 2 bytes
                else
                    return null;

                seq = binr.ReadBytes(15);       //read the Sequence OID
                if (!CompareBytearrays(seq, SeqOID))    //make sure Sequence for OID is correct
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8103) //data read as little endian order (actual data order for Bit String is 03 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8203)
                    binr.ReadInt16();   //advance 2 bytes
                else
                    return null;

                bt = binr.ReadByte();
                if (bt != 0x00)     //expect null byte next
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();   //advance 2 bytes
                else
                    return null;

                twobytes = binr.ReadUInt16();
                byte lowbyte = 0x00;
                byte highbyte = 0x00;

                if (twobytes == 0x8102) //data read as little endian order (actual data order for Integer is 02 81)
                    lowbyte = binr.ReadByte();  // read next bytes which is bytes in modulus
                else if (twobytes == 0x8202)
                {
                    highbyte = binr.ReadByte(); //advance 2 bytes
                    lowbyte = binr.ReadByte();
                }
                else
                    return null;
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };   //reverse byte order since asn.1 key uses big endian order
                int modsize = BitConverter.ToInt32(modint, 0);

                byte firstbyte = binr.ReadByte();
                binr.BaseStream.Seek(-1, SeekOrigin.Current);

                if (firstbyte == 0x00)
                {   //if first byte (highest order) of modulus is zero, don't include it
                    binr.ReadByte();    //skip this null byte
                    modsize -= 1;   //reduce modulus buffer size by 1
                }

                byte[] modulus = binr.ReadBytes(modsize);   //read the modulus bytes

                if (binr.ReadByte() != 0x02)            //expect an Integer for the exponent data
                    return null;
                int expbytes = (int)binr.ReadByte();        // should only need one byte for actual exponent data (for all useful values)
                byte[] exponent = binr.ReadBytes(expbytes);

                // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSAParameters RSAKeyInfo = new RSAParameters();
                RSAKeyInfo.Modulus = modulus;
                RSAKeyInfo.Exponent = exponent;
                RSA.ImportParameters(RSAKeyInfo);
                return RSA;
            }
            catch (Exception)
            {
                return null;
            }

            finally { binr.Close(); }

        }


        public static string Get4096XMLKeyFromPrivatePemKey(string pemPrivateKeyPath)
        {
            AsymmetricCipherKeyPair keyPair;

            using (var reader = File.OpenText(pemPrivateKeyPath))
                keyPair = (AsymmetricCipherKeyPair)new PemReader(reader).ReadObject();

            ///keyPair.Private;
            return null;

        }

        public static string Get4096XMLKeyFromPemKey(string pemPublicKey)
        {
            RSACryptoServiceProvider rsa0 = DecodeX509PublicKey(Convert.FromBase64String(pemPublicKey));

            return rsa0.ToXmlString(false);
        
        }


        private static byte[] DecodePkcs8PrivateKey(string instr)
        {
            const string pemp8header = "-----BEGIN PRIVATE KEY-----";
            const string pemp8footer = "-----END PRIVATE KEY-----";
            string pemstr = instr.Trim();
            byte[] binkey;
            if (!pemstr.StartsWith(pemp8header) || !pemstr.EndsWith(pemp8footer))
                return null;
            StringBuilder sb = new StringBuilder(pemstr);
            sb.Replace(pemp8header, "");  //remove headers/footers, if present
            sb.Replace(pemp8footer, "");

            string pubstr = sb.ToString().Trim();   //get string after removing leading/trailing whitespace

            try
            {
                binkey = Convert.FromBase64String(pubstr);
            }
            catch (FormatException)
            {        //if can't b64 decode, data is not valid
                return null;
            }
            return binkey;
        }

    

        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)       //expect integer
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();    // data size in next byte
            else
                if (bt == 0x82)
                {
                    highbyte = binr.ReadByte(); // data size in next 2 bytes
                    lowbyte = binr.ReadByte();
                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                    count = BitConverter.ToInt32(modint, 0);
                }
                else
                {
                    count = bt;     // we already have the data size
                }



            while (binr.ReadByte() == 0x00)
            { //remove high order zeros in data
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);     //last ReadByte wasn't a removed zero, so back up a byte
            return count;
        }

        // We use BouncyCastle library which is available in C++
        //https://www.findbestopensource.com/product/bouncycastlepp
        //This is what is recommended in the following link: http://superdry.apphb.com/tools/online-rsa-key-converter
        public static string GetXMLKeyFromPemKey(string pemPublicKey)
        {

            StreamReader reader = GetStreamReaderFromPublicKey(pemPublicKey);

            Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters keyPair = (Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters)new Org.BouncyCastle.OpenSsl.PemReader(reader).ReadObject();

            System.Security.Cryptography.RSA rsa = Org.BouncyCastle.Security.DotNetUtilities.ToRSA(keyPair);

            string xml = rsa.ToXmlString(false);

            return xml;

        }


        //// We use BouncyCastle library which is available in C++
        ////https://www.findbestopensource.com/product/bouncycastlepp
        ////This is what is recommended in the following link: http://superdry.apphb.com/tools/online-rsa-key-converter
        public static string GetXMLPrivateKeyFromPemPrivateKey(string pemPrivateKey)
        {

            StreamReader reader = GetStreamReaderFromPublicKey(pemPrivateKey);

            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair keyPair = (Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair)new Org.BouncyCastle.OpenSsl.PemReader(reader).ReadObject();

            AsymmetricKeyParameter privateKey = keyPair.Private;
            var rsa = DotNetUtilities.ToRSA((RsaPrivateCrtKeyParameters)privateKey);
            string xmlRsa = rsa.ToXmlString(true);
            return xmlRsa;
        }

        public static string PublicXmlToPem(RSACryptoServiceProvider rsa)
        {
            RsaKeyParameters publicKey = Org.BouncyCastle.Security.DotNetUtilities.GetRsaPublicKey(rsa); // try get public key
            if (publicKey != null) // if XML RSA key contains public key
            {
                SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);
                return FormatPem(Convert.ToBase64String(publicKeyInfo.GetEncoded()), "PUBLIC KEY");
            }

            throw new InvalidKeyException("Invalid RSA Xml Key");
        }


        private static string FormatPem(string pem, string keyType)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("-----BEGIN {0}-----", keyType);

            int line = 1, width = 64;

            while ((line - 1) * width < pem.Length)
            {
                int startIndex = (line - 1) * width;
                int len = line * width > pem.Length
                              ? pem.Length - startIndex
                              : width;
                sb.AppendFormat("{0}", pem.Substring(startIndex, len));
                line++;
            }

            sb.AppendFormat("-----END {0}-----", keyType);
            return sb.ToString();
        }

        public static void WritePublicToPem(RSACryptoServiceProvider rsa,string file)
        {
            RsaKeyParameters keys = Org.BouncyCastle.Security.DotNetUtilities.GetRsaPublicKey(rsa);

            TextWriter textWriter = new StreamWriter(file);
            PemWriter pemWriter = new PemWriter(textWriter);
            pemWriter.WriteObject(keys);
            pemWriter.Writer.Flush();

            textWriter.Close();
        }


        public static void WriteToFile(string xml, string file)
        {
            TextWriter textWriter = new StreamWriter(file);

            textWriter.WriteLine(xml);

            textWriter.Close();
        }



        #endregion

    }
}
