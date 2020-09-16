using Rialto.KoreConX.Common;
using Rialto.KoreConX.Common.DTO.Generic;
using Rialto.KoreConX.Common.DTO.Holdings;
using Rialto.KoreConX.ServiceLayer.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoldReleasePOC
{
    class Program
    {

        #region Protected Static Attributes

        public static HoldingsServiceClient HoldingsServiceClient { get; set; }

        #endregion

        #region Public Statuc Methods


        public static void PutOnHoldAndThenReleaseExample()
        {

            HoldSharesDTO dto = BuildHoldDto();

            TransactionResponse txHoldResp = null;

            try
            {

                txHoldResp = HoldingsServiceClient.PutHoldOnShares(dto);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Critical error putting {0} on hold:{1}", dto.koresecurities_id, ex.Message));
            }

             dto.ats_transaction_id = txHoldResp.data.id;
             TransactionResponse txReleaseResp = HoldingsServiceClient.ReleaseShares(dto);
        }
       


        #endregion

        #region Public Static Constructor Methods

        public static HoldSharesDTO BuildHoldDto()
        {

            HoldSharesDTO dto = new HoldSharesDTO();
            dto.ats_id = ConfigurationManager.AppSettings["KoreATSId"];
            dto.ats_transaction_id = "tr1";
            dto.koresecurities_id = ConfigurationManager.AppSettings["KoreSecurityId"];
            dto.last_updated_at = DateTime.Now.ToString("YYYY-MM-dd");
            dto.reason_code = HoldReasons.PendingSell.ToString();
            dto.number_of_shares = Convert.ToInt32(ConfigurationManager.AppSettings["SellQty"]);
            dto.securities_holder_id =  ConfigurationManager.AppSettings["KoreShareholderId"] ;

            return dto;

        }

        public 


        #endregion



        static void Main(string[] args)
        {
            string BaseURL = ConfigurationManager.AppSettings["BaseURL"];

            HoldingsServiceClient  = new HoldingsServiceClient(BaseURL);

            PutOnHoldAndThenReleaseExample();

            Console.ReadKey();

        }
    }
}
