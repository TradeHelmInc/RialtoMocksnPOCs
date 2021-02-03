using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rialto.Rialto.Common.DTO.Services;
using Rialto.Rialto.Common.DTO.Generic;

namespace Rialto.Rialto.ServiceLayer.Client
{
    public class TransferServiceClient : RialtoBaseRESTClient
    {
        #region Constructors

        public TransferServiceClient(string pBaseURL, string pUser, string pPassword)
        {

            BaseURL = pBaseURL;

            User = pUser;

            Password = pPassword;

        }

        #endregion

        #region Protected Static Consts

        protected static string _TRANSFER_SHARES = "/Trading/TransferShares/";

        #endregion

        #region Public Methods

        public TransactionResponse TransferShares(int buyShareholderId, int sellShareholderId, double tradeQuantity,
                                                    int securityId, int sellOrderId)
        {
            try
            {
                string url = BaseURL + _TRANSFER_SHARES;

                Dictionary<string, string> param = new Dictionary<string, string>();

                TransfServiceDTO dto = new TransfServiceDTO()
                {
                    BuyShareholderId = buyShareholderId,
                    SellShareholderId = sellShareholderId,
                    TradeQuantity = tradeQuantity,
                    SecurityId = securityId,
                    SellOrderId = sellOrderId

                };

                string output = JsonConvert.SerializeObject(dto);

                return DoPostJson(url, param, output);
            }
            catch (Exception ex)
            {
                return new TransactionResponse() { Success = false, Error = new ErrorMessage() { code = 1, msg = ex.Message } };
            }

        }

        #endregion
    }
}
