using fwk.Common.util.encryption.RSA;
using Rialto.KoreConX.Common.DTO.Generic;
using Rialto.KoreConX.Common.DTO.Shareholders;
using Rialto.KoreConX.Common.Util;
using Rialto.KoreConX.ServiceLayer.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OnboardShareholdersPOC
{
    class Program
    {
        #region Protected Static Attributes

        public static SecuritiesServiceClient SecuritiesServiceClient { get; set; }

        public static PersonsServiceClient PersonsServiceClient { get; set; }

        public static string CompanyId { get; set; }

        protected static string KoreATSId { get; set; }

        protected static string KoreSecurityId { get; set; }

        protected static string AESKeyandIV { get; set; }

        protected static AESEncrypter AESEncrypter { get; set; }

        #endregion

        #region Protected Methods

        public static void GetShareholders()
        {
            Console.WriteLine(string.Format("Requesting shareholders for security {0}", KoreSecurityId));
            DataResponse shareholders = SecuritiesServiceClient.RequestShareholders(company_id: CompanyId, koresecurities_id: KoreSecurityId, requestor_id: KoreATSId);

            //TODO : all the validations
            foreach (string shareholderId in shareholders.data.data)
            { 
                //TODO: Request all the persoanl information for shareholderId
                Console.WriteLine(string.Format("Requesting personal information for shareholder id {0}", shareholderId));

                PersonResponse personResp = PersonsServiceClient.RequestPerson(shareholderId);

                Console.WriteLine(string.Format("Received {0} {1} from {2}. PD field:{3}", personResp.data.first_name, personResp.data.last_name, personResp.data.country, personResp.data.pd));

                if (string.IsNullOrEmpty(personResp.data.pd))
                {
                    Console.WriteLine(string.Format("Shareholder {0} {1} does not have personal data (PD) loaded. It will be uploaded as a draft", personResp.data.first_name, personResp.data.last_name));

                }
                else
                {
                    string pdFields = AESEncrypter.DecryptAES(personResp.data.pd);

                    Console.WriteLine(string.Format("Decrypting personal information <PD>:{0}", pdFields));
                }

                
                Console.ReadKey();
                Console.Clear();
            }

            
        }

        #endregion

        static void Main(string[] args)
        {
            string BaseURL = ConfigurationManager.AppSettings["BaseURL"];
            KoreATSId = ConfigurationManager.AppSettings["KoreATSId"];
            CompanyId = ConfigurationManager.AppSettings["CompanyId"];
            KoreSecurityId = ConfigurationManager.AppSettings["KoreSecurityId"];

            AESKeyandIV = ConfigurationManager.AppSettings["AESKeyandIV"];

            AESEncrypter = new AESEncrypter(AESKeyandIV);
            
            SecuritiesServiceClient = new SecuritiesServiceClient(BaseURL);
            PersonsServiceClient = new PersonsServiceClient(BaseURL);

            GetShareholders();

            Console.WriteLine(" Finished downloading all the shareholders");
            Console.ReadLine();
        }
    }
}
