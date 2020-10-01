using fwk.Common.enums;
using Rialto.KoreConX.Common;
using Rialto.KoreConX.Common.DTO.Generic;
using Rialto.KoreConX.Common.DTO.Holdings;
using Rialto.KoreConX.Common.DTO.Securities;
using Rialto.KoreConX.ServiceLayer.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoldTransfer
{
    class Program
    {
        #region Protected Static Attributes

        public static HoldingsServiceClient HoldingsServiceClient { get; set; }

        #endregion

        #region Private Util Methods

        private static void DoLog(string msg, MessageType type)
        {
            Console.WriteLine("");
            if (type == MessageType.Error)
                Console.ForegroundColor = ConsoleColor.Red;

            if (type == MessageType.Debug)
                Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine(msg);

            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("");
        }

        private static int GetNumberSharesTraded(HoldSharesDTO dto)
        {
            while (true)
            {
                DoLog("Enter the number of shares traded:", MessageType.Debug);
                string strNumberTraded = Console.ReadLine();

                try
                {
                    int sharesTraded = Convert.ToInt32(strNumberTraded);

                    if (sharesTraded > dto.number_of_shares)
                    {
                        Console.WriteLine(string.Format("You cannot trade mor than {0} sent to sell", dto.number_of_shares));
                        Console.WriteLine("");
                    }
                    else
                    {
                        return sharesTraded;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Value {0} is not a valid number for sell", strNumberTraded));
                    Console.WriteLine("");
                }

            }
        }


        #endregion

        #region Private Static Static Methods

        #region Sell Shares

        private static void AvailableShares()
        {
            //1- First step, once we know the security <KoreSecurityId> to sell and the shareholder <KoreShareholderId> that is selling
            //we can build a HoldSharesDTO
            //The third component of the 2 previous Ids is the ATS Id in the KoreChain: ATS_ID <always a fixed number and populated just once>
            HoldSharesDTO dto = BuildHoldDto();

            //2-First we have to know if we have enough shares to sell, so we invoke service Available Shares
            //for responses structure see PutOnHold advise in PutOnHold() method
            DoLog(string.Format("<@Transfer_agent_transactions - status=KCX_REQ_AVAILABLE_SHARES > - Requesting available shares for KoreShareholderId {0} and KoreSecurityId {1}", dto.securities_holder_id, dto.koresecurities_id), MessageType.Information);
            ValidationResponse resp = HoldingsServiceClient.AvailableShares(dto.securities_holder_id, dto.koresecurities_id, dto.number_of_shares, dto.ats_id);
            if (resp.message != null)
            {
                DoLog(string.Format("<@Transfer_agent_transactions - status=KCX_ERROR_ENOUGH_SHARES > - Error Requesting available shares for KoreShareholderId {0} and KoreSecurityId {1}: {2}",
                    dto.securities_holder_id, dto.koresecurities_id, resp.message.GenMessage != null ? resp.message.GenMessage.msg : resp.message.message), MessageType.Error);
            }
            else
            {
                if (resp.data != null && resp.data.exists)
                {
                    //3-If everything is ok, we put the shares on hold
                    DoLog(string.Format("<@Transfer_agent_transactions - status=KCX_ENOUGH_SHARES_VALIDATED > - Enough shares validated for KoreShareholderId {0} and KoreSecurityId {1}", dto.securities_holder_id, dto.koresecurities_id), MessageType.Information);
                    PutOnHold(dto);
                }
                else
                {
                    //4-If there are not enough shares, we update the status table, we inform the error. 
                    DoLog(string.Format("<@Transfer_agent_transactions - status=KCX_NOT_ENOUGH_SHARES > - Not Enough shares available  for KoreShareholderId {0} and KoreSecurityId {1}", dto.securities_holder_id, dto.koresecurities_id), MessageType.Error);
                }
            }
        }

        private static void ReleaseShares(TransactionResponse txHoldResp, HoldSharesDTO holdDTO)
        {
            ReleaseSharesDTO relDto = ReleaseHoldDto(holdDTO);

            //1-So we had a successful hold. We create a release dto which looks exactly the same as the hold
            // with just one exception. We have to provide the same transaction id that was returned when putting on hold!
            relDto.ats_transaction_id = txHoldResp.data.id;

            TransactionResponse txtReleaeResp = null;
            try
            {
                //2-It´s important to remark that this will only cover scenarios when an order is CANCELLED or EXPIRED.
                //When we have at least a partial fill , this will be covered in a different test case
                DoLog(string.Format("<@Transfer_agent_transactions - status=KCX_REQ_RELEASE_SHARES > - Releasing {0} shares for KoreSecurityId {1} and TransactionId {2}", relDto.number_of_shares, relDto.koresecurities_id, relDto.ats_transaction_id), MessageType.Information);
                txtReleaeResp = HoldingsServiceClient.ReleaseShares(relDto);

                //3- Whatever happened with the release service, leave it properly registered at Transfer_agent_transactions table 
                //
                if (txtReleaeResp.GenericError == null)
                {
                    DoLog(string.Format("<@Transfer_agent_transactions - status= KCX_RELEASE_SHARES_SUCCESS > - Shares for KoreSecurityId {0} successfully released. TransactionId:{1}", relDto.koresecurities_id, txtReleaeResp.data.id), MessageType.Information);

                }
                else
                {
                    DoLog(string.Format("<@Transfer_agent_transactions - status= KCX_RELEASE_SHARES_REJECTED > - Shares for KoreSecurityId {0} could not be released. Error:{1}", relDto.koresecurities_id, txtReleaeResp.GenericError.message), MessageType.Error);
                }

            }
            catch (Exception ex)
            {
                DoLog(string.Format("<@Transfer_agent_transactions - status= KCX_RELEASE_SHARES_REJECTED > Critical error releasing shares for security {0}:{1}", relDto.koresecurities_id, ex.Message), MessageType.Error);
            }
        }

        
        private static TransactionResponse PutOnHold(HoldSharesDTO dto)
        {
            TransactionResponse txHoldResp = null;

            try
            {
                //1-Put Hold responses can have to different response structures depending if the request was successfull or not
                //an unsuccessful response doesn´t has to be a technical issue. It can be that some of the ids used for the request was no longer available at KCX
                //and sometimes even that there is not enough shares to salle
                //<ADVISE> -That´s why it´s advisable that the DTO that holds the answer, can handle right and wrong cases
                //but errors should not be handled with exceptions, as valid business scenarios could be handled through error messages which are very easily mistaken by
                //as technical errors (Ex: connection down)
                DoLog(string.Format("<@Transfer_agent_transactions - status=KCX_REQ_HOLD_ON_SHARES > - Putting {0} shares on hold for KoreSecurityId {1}", dto.number_of_shares, dto.koresecurities_id), MessageType.Information);
                txHoldResp = HoldingsServiceClient.PutHoldOnShares(dto);

                if (txHoldResp.GenericError == null)
                {
                    //1.a - If we could put the shares on hold, we update the table Transfer_agent_transactions and we send the order to the exchange.
                    DoLog(string.Format("<@Transfer_agent_transactions - status= KCX_HOLD_SHARES_SUCCESS > - Shares for KoreSecurityId {0} successfully put on hold. TransactionId:{1}", dto.koresecurities_id, txHoldResp.data.id), MessageType.Information);

                    int tradedShares = GetNumberSharesTraded(dto);
                
                    //1.b-Once the user cancelled the order (or it expired) we can release the shares
                    TransferShares(dto,txHoldResp, tradedShares);

                    //1.2-We release the remaining shares
                    dto.number_of_shares=dto.number_of_shares-tradedShares;
                    ReleaseShares(txHoldResp, dto);

                }
                else
                {
                    //2-The hold was rejected. We update the Transfer_agent_transactions table and the sell cannot continue
                    DoLog(string.Format("<@Transfer_agent_transactions - status= KCX_HOLD_ON_SHARES_REJECTED > - Shares for KoreSecurityId {0} could not be put on hold put. Error:{1}", dto.koresecurities_id, txHoldResp.GenericError.message), MessageType.Error);
                }

            }
            catch (Exception ex)
            {
                //3-Orders rejected
                DoLog(string.Format("<@Transfer_agent_transactions - status= KCX_HOLD_ON_SHARES_REJECTED > Critical error putting {0} on hold:{1}", dto.koresecurities_id, ex.Message), MessageType.Error);
            }

            return txHoldResp;
        }

        private static void TransferShares(HoldSharesDTO holdDTO, TransactionResponse txHoldResp, int tradedShares)
        {

            TransferSharesDTO transferDto = TransferDto(holdDTO, tradedShares);

            //1- The same way we did with Hold/Release, we crate a transfer dto, where we will provde the Hold trnsactionId
            // Here I will have to know who the buyer is <int his POC we just use the usr id loaded in the config files>. 
            // But in real ATS I will have to identify who te buy is. Beware I might have multiple buyers! 
            //In this example I just consider that there is one buyer
            transferDto.transfer_authorization_transaction_id = txHoldResp.data.id;
            transferDto.transferred_to_id = ConfigurationManager.AppSettings["KoreBuyerShareholderId"];


            TransactionResponse txtTransferResp = null;
            try
            {
                DoLog(string.Format("<@Transfer_agent_transactions - status=KCX_REQ_TRANSFER_SHARES > - Transfering {0} shares for KoreSecurityId {1} and TransactionId {2} from shareholder {3} to shareholder {4}"
                                    , tradedShares, transferDto.koresecurities_id, transferDto.transfer_authorization_transaction_id, transferDto.owner_id, transferDto.transferred_to_id), MessageType.Information);
                txtTransferResp = HoldingsServiceClient.TransferShares(transferDto);

                //3- Whatever happened with the release service, leave it properly registered at Transfer_agent_transactions table 
                //
                if (txtTransferResp.GenericError == null)
                {
                    DoLog(string.Format("<@Transfer_agent_transactions - status= KCX_TRANSFER_SHARES_SUCCESS > - Shares for KoreSecurityId {0} successfully Transfered. TransactionId:{1}", transferDto.koresecurities_id, txtTransferResp.data.id), MessageType.Information);

                }
                else
                {
                    DoLog(string.Format("<@Transfer_agent_transactions - status= KCX_TRANSFER_SHARES_REJECTED > - Shares for KoreSecurityId {0} could not be transferred. Error:{1}", transferDto.koresecurities_id, txtTransferResp.GenericError.message), MessageType.Error);
                }

            }
            catch (Exception ex)
            {
                DoLog(string.Format("<@Transfer_agent_transactions - status= KCX_TRANSFER_SHARES_REJECTED > Critical error transferring shares for security {0}:{1}", transferDto.koresecurities_id, ex.Message), MessageType.Error);
            }
        }

        #endregion

        #endregion

        #region Public Static Methods

        public static void OnSell()
        {
            AvailableShares();

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
            dto.securities_holder_id = ConfigurationManager.AppSettings["KoreSellerShareholderId"];

            return dto;

        }

        public static ReleaseSharesDTO ReleaseHoldDto(HoldSharesDTO holdDto)
        {

            ReleaseSharesDTO relDto = new ReleaseSharesDTO();
            relDto.ats_id = holdDto.ats_id;
            relDto.ats_transaction_id = null;
            relDto.koresecurities_id = holdDto.koresecurities_id;
            relDto.last_updated_at = holdDto.last_updated_at;
            relDto.reason_code = holdDto.reason_code;
            relDto.number_of_shares = holdDto.number_of_shares;
            relDto.securities_holder_id = holdDto.securities_holder_id;

            return relDto;
        }

        public static TransferSharesDTO TransferDto(HoldSharesDTO holdDto,int sharesTraded)
        {

            TransferSharesDTO transferDto = new TransferSharesDTO();
            transferDto.company_id = "_???";
            transferDto.koresecurities_id = holdDto.koresecurities_id;
            transferDto.owner_id = holdDto.securities_holder_id;
            transferDto.transferred_to_id = "xxx";
            transferDto.transfer_authorization_transaction_id = "???";
            transferDto.total_securities = sharesTraded;
            transferDto.effective_date = DateTime.Now ;


            return transferDto;
        }

        public


        #endregion

 static void Main(string[] args)
        {
            string BaseURL = ConfigurationManager.AppSettings["BaseURL"];

            HoldingsServiceClient = new HoldingsServiceClient(BaseURL);

            OnSell();

            Console.ReadKey();

        }
    }
}
