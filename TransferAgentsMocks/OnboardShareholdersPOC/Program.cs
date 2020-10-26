using Rialto.KoreConX.Common.DTO.Generic;
using Rialto.KoreConX.Common.DTO.Shareholders;
using Rialto.KoreConX.ServiceLayer.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
                Console.WriteLine("CONTINUE DEVELOPMENT--> Decrypt personal info!!");

                Console.WriteLine("");
            }

            
        }

        #endregion

        static void Main(string[] args)
        {
            string BaseURL = ConfigurationManager.AppSettings["BaseURL"];
            KoreATSId = ConfigurationManager.AppSettings["KoreATSId"];
            CompanyId = ConfigurationManager.AppSettings["CompanyId"];
            KoreSecurityId = ConfigurationManager.AppSettings["KoreSecurityId"];
            
            SecuritiesServiceClient = new SecuritiesServiceClient(BaseURL);
            PersonsServiceClient = new PersonsServiceClient(BaseURL);

            GetShareholders();

            Console.WriteLine(" Finished downloading all the shareholders");
            Console.ReadLine();
        }
    }
}
