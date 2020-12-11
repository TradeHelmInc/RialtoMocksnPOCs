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

namespace fwk.Common.util.encryption.common
{
    public class FileLoader
    {

        #region Public Static Methods

        //READ THIS FILE with WHATEVER READER METHOD YOU HAVE IN C++
        public static string GetFileContent(string file)
        {
            string result = "";
            using (StreamReader streamReader = new StreamReader(file))
            {
                result = streamReader.ReadToEnd();
            }

            return result;

        }

     


        #endregion
    }
}
