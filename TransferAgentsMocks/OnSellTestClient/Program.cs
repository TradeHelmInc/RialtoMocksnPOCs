using Rialto.Rialto.Common.DTO.Generic;
using Rialto.Rialto.Common.DTO.Services;
using Rialto.Rialto.ServiceLayer.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnSellTestClient
{
    class Program
    {
        #region Protected Attributes

        protected static OnSellServiceClient OnSellServiceClient { get; set; }

        #endregion

        #region Protected Static Methods

        protected static void DoLog(string msg)
        {
            Console.WriteLine(msg);
        }

        protected static OnSellDTO BuildOnSellDTO()
        {
            OnSellDTO dto = new OnSellDTO()
            {
                SellShareholderId=Convert.ToInt32(ConfigurationManager.AppSettings["SellShareholderId"]),
                SecurityId=Convert.ToInt32(ConfigurationManager.AppSettings["SecurityId"]),
                OrderQty=Convert.ToDouble(ConfigurationManager.AppSettings["OrderQty"])
            };

            return dto;
        
        }

        protected static void HandleResponse(OnSellDTO dto,TransactionResponse txHoldResp)
        {

            if (txHoldResp.Error == null)
            {
                //1.a - If we could put the shares on hold, we update the table Transfer_agent_transactions and we send the order to the exchange.
                Console.WriteLine(string.Format("HoldShares SUCESS SellerId={0} SecurityId={1} and Qty={2}", dto.SellShareholderId, dto.SecurityId, dto.OrderQty));
            }
            else
            {
                //2-The hold was rejected. We update the Transfer_agent_transactions table and the sell cannot continue
                Console.WriteLine(string.Format("HoldShares FAILURE SellerId={0} SecurityId={1} and Qty={2}:{3}", dto.SellShareholderId, dto.SecurityId, dto.OrderQty,
                                                                                                                    txHoldResp.Error.msg));
            }
        }

        #endregion

        static void Main(string[] args)
        {
            string BaseURL = ConfigurationManager.AppSettings["OnSellServiceURL"];

            OnSellServiceClient = new OnSellServiceClient(BaseURL);

            Console.WriteLine(string.Format("OnSell Client successfully initialyzed at {0}", BaseURL));

            OnSellDTO dto = BuildOnSellDTO();

            Console.WriteLine(string.Format("Invoking OnSell service with SellerId={0} SecurityId={1} and Qty={2}", dto.SellShareholderId, dto.SecurityId, dto.OrderQty));

            TransactionResponse resp = OnSellServiceClient.OnSell(dto);

            HandleResponse(dto, resp);

            Console.ReadKey();
        }
    }
}
