using fwk.Common.interfaces;
using MySql.Data.MySqlClient;
using Rialto.BusinessEntities;
using Rialto.BusinessEntities.KoreConX;
using Rialto.DataAccessLayer;
using Rialto.KoreConX.Common;
using Rialto.KoreConX.Common.DTO.Generic;
using Rialto.KoreConX.Common.DTO.Holdings;
using Rialto.KoreConX.Common.DTO.Securities;
using Rialto.KoreConX.ServiceLayer.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.LogicLayer
{
    public class TradingService : BaseLayer
    {
        #region Protected Attributes

        protected SecurityManager SecurityManager { get; set; }

        protected ShareholderManager ShareholderManager { get; set; }

        protected OrderManager OrderManager { get; set; }

        protected TransferAgentManager TransferAgentManager { get; set; }

        #region KoreConX 

        protected HoldingsServiceClient KCXHoldingsServiceClient { get; set; }

        #endregion

        #endregion

        #region Constructors

        public TradingService(string pTradingConnectionString, string pOrderConnectionString, string pKCXURL, ILogger pLogger)
        {
            SecurityManager = new SecurityManager(pTradingConnectionString);

            ShareholderManager = new ShareholderManager(pTradingConnectionString);

            OrderManager = new OrderManager(pOrderConnectionString, pTradingConnectionString);

            TransferAgentManager = new TransferAgentManager(pTradingConnectionString);

            Logger = pLogger;

            #region KCX

            KCXHoldingsServiceClient = new HoldingsServiceClient(pKCXURL);

            #endregion
        }


        #endregion

        #region Private Method

        protected Security GetSecurity(int securityId)
        {
            Security sec = SecurityManager.GetSecurity(securityId);

            if (sec == null)
                throw new Exception(string.Format("There is not a security for id {0}", securityId));


            sec.KoreConXSecurityId = SecurityManager.GetKoreConXSecurityId(sec.Id);

            return sec;
        }

        protected Shareholder GetShareholder(int shareHolderId)
        {
            Shareholder shr = ShareholderManager.GetShareholder(shareHolderId);

            if (shr == null)
                throw new Exception(string.Format("There is not a shareholder (investor) for id {0}", shareHolderId));


            shr.KoreConXShareholderId = ShareholderManager.GetKoreShareholderId(shr.Id);

            return shr;
        }

        protected Order GetOrder(int orderId)
        {
            Order order = OrderManager.GetOrder(orderId);

            if (order == null)
                throw new Exception(string.Format(" There is not a sell order for order id {0}", orderId));

            //We can work with all the transactions. There are not going to be so many orders
            order.KoreConXTransactions = OrderManager.GetKoreConXTransaction(orderId);

            return order;
        }

        private void ValidateTradesToClear(Security sec, Shareholder buyer, Shareholder seller, Order buyOrder, Order sellOrder)
        {

            if (sec == null)
                throw new Exception(string.Format("Critical error recovering trades to clear: Could not find a security for symbol {0}", buyOrder.Symbol));

            if (buyer == null)
                throw new Exception(string.Format("Critical error recovering trades to clear: COuld not find a buyer shareholder id {0}", buyOrder.ShareholderId));

            if (seller == null)
                throw new Exception(string.Format("Critical error recovering trades to clear: COuld not find a buyer shareholder id {0}", sellOrder.ShareholderId));

            if (!buyOrder.ExecutedQty.HasValue)
                throw new Exception(string.Format("Buy order id {0} does not have an executed quantity!", buyOrder.Id));

            if (!sellOrder.ExecutedQty.HasValue)
                throw new Exception(string.Format("Sell order id {0} does not have an executed quantity!", sellOrder.Id));


            if (!buyOrder.TradePrice.HasValue)
                throw new Exception(string.Format("Buy order id {0} does not have an execution price!", buyOrder.Id));

            if (!sellOrder.TradePrice.HasValue)
                throw new Exception(string.Format("Sell order id {0} does not have an execution price!", sellOrder.Id));

            if (!buyOrder.ExecutionTime.HasValue)
                throw new Exception(string.Format("Buy order id {0} does not have an execution date and time!", buyOrder.Id));

            if (!sellOrder.ExecutionTime.HasValue)
                throw new Exception(string.Format("Sell order id {0} does not have an execution date and time!", sellOrder.Id));
        }

        private void ValidateTransferShares(Security sec, Shareholder buyer, Shareholder seller, Order sellOrder, TransferAgent transfAgent)
        {
            //So far, the only available transfer agent is KoreConX , So if that transfer agent is not assigned to the securit
            //the sell cannot continue
            if (sec.KoreConXSecurityId == null)
                throw new Exception(string.Format("Could not find a transfer agent for security {0} (id={1})", sec.Symbol, sec.Id));

            if (sec.KoreConXSecurityId != null)// If the transfer agent is KoreConX, we validate that the securities have their Kore Security Ids
            {
                if (buyer.KoreConXShareholderId == null)
                    throw new Exception(string.Format("Critical Error, could not find shareholder {0}(id={1}) in KoreConX transfer agent", buyer.Name, buyer.Id));

                if (seller.KoreConXShareholderId == null)
                    throw new Exception(string.Format("Critical Error, could not find shareholder {0}(id={1}) in KoreConX transfer agent", seller.Name, seller.Id));

                if (sellOrder.MatchingId == null)
                    throw new Exception(string.Format("The order {0} does not have a matching id!", sellOrder.MatchingId));

                if (sellOrder.State != State.PARTIAL && sellOrder.State != State.EXECUTED)
                    throw new Exception(string.Format("Sell order id {0} is not executed!!",sellOrder.Id));

                if (sellOrder.ShareholderId != seller.Id)
                    throw new Exception(string.Format("Sell order Id {0} has a shareholder {1} which does not match with shareholder {2}",
                                                        sellOrder.Id, sellOrder.ShareholderId, sellOrder.Id));

                //We validate that there is a hold
                if (!sellOrder.KoreConXTransactions.Any(x => x.State == KoreConXTransactionState.KCX_HOLD_SHARES_SUCCESS))
                    throw new Exception(string.Format("Order Id {0} shares were not put on hold in KoreConX, so there is not a transaction Id to release it!", sellOrder.Id));

                if(sellOrder.GetHoldTransaction()==null)
                    throw new Exception(string.Format("Order Id {0} shares  were not even put on hold!!", sellOrder.Id));

                //We validate that there is a TransactionId
                if (sellOrder.GetHoldTransaction().TransactionId==null)
                    throw new Exception(string.Format("Order Id {0} shares  were put on hold but there is not a transaction Id!!", sellOrder.Id));

                if (transfAgent == null)
                    throw new Exception("There is NOT a KoreConX transfer agent in the system!");

                if (transfAgent.TransferAgentATSId == null)
                    throw new Exception("There is NOT a KoreConX ATS ID in the system!");
            }
        }

        private TransferAgent ValidateSell(Shareholder seller, Security sec)
        {
            if (sec.KoreConXSecurityId != null)
            {
                if (seller.KoreConXShareholderId == null || seller.KoreConXShareholderId.KoreShareholderId == null)
                    throw new Exception(string.Format("Seller {0} is not a Registerd in transfer agent KoreConX!", seller.Name));

                TransferAgent transfAgent = TransferAgentManager.GetTransferAgent(TransferAgentManager._KCX_ID);

                if (transfAgent == null)
                    throw new Exception(string.Format("Could not find KoreConX transfer agent loaded"));

                if (transfAgent.TransferAgentATSId == null)
                    throw new Exception(string.Format("KoreConX Rialto ATS ID is not properly loaded"));

                return transfAgent;
            }
            else 
                throw new Exception(string.Format("Security {0} is not registered in any of the available Transfer Agents", sec.Symbol));
        
        
        }

        private void ValidateBuy(Shareholder buyer, Security sec)
        {
            if (sec.KoreConXSecurityId != null)
            {
                if (buyer.KoreConXShareholderId == null || buyer.KoreConXShareholderId.KoreShareholderId == null)
                    throw new Exception(string.Format("Buyer {0} is not a Registerd in transfer agent Kore Con X!", buyer.Name));
            }
            else
                throw new Exception(string.Format("Security {0} is not registered in any of the available Transfer Agents", sec.Symbol));


        }

        #region KoreConX methods

        private void KCXDoPersistKCXTransction(Order sellOrder, int qty,KoreConXTransactionState state)
        {

            KoreConXTransaction tx = sellOrder.GetHoldTransaction().Clone();

            tx.State = state;
            tx.NumberOfShares = qty;

            OrderManager.PersistTransaction(tx);
        
        }

        private string KCXDoTransferShares(Security sec, Shareholder buyer, Shareholder seller, Order sellOrder, int tradeQuantity)
        {
           
            KCXDoPersistKCXTransction(sellOrder, tradeQuantity, KoreConXTransactionState.KCX_TRANSFER_SHARES_REQUEST);
            try
            {

                TransferSharesDTO kcxTransferDto = new TransferSharesDTO()
                {
                    company_id = sec.KoreConXSecurityId.CompanyId,
                    koresecurities_id = sec.KoreConXSecurityId.KoreSecurityId,
                    transfer_authorization_transaction_id = sellOrder.GetHoldTransaction().TransactionId,
                    transferred_to_id = buyer.KoreConXShareholderId.KoreShareholderId,
                    owner_id = seller.KoreConXShareholderId.KoreShareholderId,
                    total_securities = tradeQuantity,
                    effective_date = DateTime.Now,
                };


                TransactionResponse txtTransferResp = KCXHoldingsServiceClient.TransferShares(kcxTransferDto);

                if (txtTransferResp.GenericError == null)
                {
                    KCXDoPersistKCXTransction(sellOrder, tradeQuantity, KoreConXTransactionState.KCX_TRANSFER_SHARES_SUCCESS);
                    //TODO : Cambiar la orde a status CLEARED

                    return txtTransferResp.data.id;
                }
                else
                {
                    KCXDoPersistKCXTransction(sellOrder, tradeQuantity, KoreConXTransactionState.KCX_TRANSFER_SHARES_REJECTED);
                    return null;
                }

            }
            catch (Exception ex)
            {
                KCXDoPersistKCXTransction(sellOrder, tradeQuantity, KoreConXTransactionState.KCX_TRANSFER_SHARES_CRITICAL);
                return null;
            }
        }

        private string ReleaseKoreConX(Order sellOrder,Shareholder seller,Security sec, TransferAgent transfAgent, KoreConXTransaction holdTx, double releaseQty)
        {
            TransactionResponse txHoldResp = null;
            ReleaseSharesDTO relDto = new ReleaseSharesDTO();
            relDto.ats_id = transfAgent.TransferAgentATSId;
            relDto.ats_transaction_id = holdTx.TransactionId;
            relDto.koresecurities_id = sec.KoreConXSecurityId.KoreSecurityId;
            relDto.last_updated_at = DateTime.Now.ToString("yyyy-MM-dd");
            relDto.reason_code = "";
            relDto.number_of_shares = Convert.ToInt32(releaseQty);
            relDto.securities_holder_id = seller.KoreConXShareholderId.KoreShareholderId;

            //1-So we had a successful hold. We create a release dto which looks exactly the same as the hold
            // with just one exception. We have to provide the same transaction id that was returned when putting on hold!
            relDto.ats_transaction_id = txHoldResp.data.id;

            TransactionResponse txtReleaeResp = null;
            try
            {
                //2-It´s important to remark that this will only cover scenarios when an order is CANCELLED or EXPIRED.
                //When we have at least a partial fill , relDto.number_of_shares will be covered in a different test case
                KCXDoPersistKCXTransction(sellOrder, relDto.number_of_shares, KoreConXTransactionState.KCX_REQ_RELEASE_SHARES);
                txtReleaeResp = KCXHoldingsServiceClient.ReleaseShares(relDto);

                //3- Whatever happened with the release service, leave it properly registered at Transfer_agent_transactions table 
                //
                if (txtReleaeResp.GenericError == null && txtReleaeResp.data != null && txtReleaeResp.data.id != null)
                {
                    //DoLog(string.Format("<@Transfer_agent_transactions - status= KCX_RELEASE_SHARES_SUCCESS > - Shares for KoreSecurityId {0} successfully released. TransactionId:{1}", relDto.koresecurities_id, txtReleaeResp.data.id), MessageType.Information);
                    KCXDoPersistKCXTransction(sellOrder, relDto.number_of_shares, KoreConXTransactionState.KCX_RELEASE_SHARES_SUCCESS_FULL);
                    return txtReleaeResp.data.id;
                }
                else
                {
                    //DoLog(string.Format("<@Transfer_agent_transactions - status= KCX_RELEASE_SHARES_REJECTED > - Shares for KoreSecurityId {0} could not be released. Error:{1}", relDto.koresecurities_id, txtReleaeResp.GenericError.message), MessageType.Error);
                    if (txtReleaeResp.GenericError != null)
                    {
                        throw new Exception(string.Format("Shares for KoreSecurityId {0} could not be released. Error:{1}", relDto.koresecurities_id, txHoldResp.GenericError.message));
                    }
                    else if (txtReleaeResp.data == null || txtReleaeResp.data.id == null)
                    {
                        throw new Exception(string.Format("Invalid KCX format for release transaction id"));
                    }
                    else
                        throw new Exception(string.Format("Unknown error releasing shares at KCX"));
                
                }

            }
            catch (Exception ex)
            {
                //TODO: Impl REASON!
                KCXDoPersistKCXTransction(sellOrder, relDto.number_of_shares, KoreConXTransactionState.KCX_RELEASE_SHARES_REJECTED);
                    
                throw new Exception(string.Format("CRITICAL ERROR releasing {0} shares for security {1} (Kore Security Id {2}):{3}", 
                                    relDto.number_of_shares, sec.Symbol, sec.KoreConXSecurityId.KoreSecurityId, ex.Message));
            }
        }

        private string PutOnHoldKoreConX(HoldSharesDTO dto)
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
                //TO DOC KCX_REQ_HOLD_ON_SHARES
                txHoldResp = KCXHoldingsServiceClient.PutHoldOnShares(dto);

                if (txHoldResp.GenericError == null && txHoldResp.data!=null && txHoldResp.data.id!=null)
                {
                    //1.a - If we could put the shares on hold, we update the table Transfer_agent_transactions and we send the order to the exchange.
                    //TO DOC KCX_HOLD_SHARES_SUCCESS
                    return txHoldResp.data.id;
                }
                else
                {
                    if (txHoldResp.GenericError != null)
                    {
                        throw new Exception(string.Format("Shares for KoreSecurityId {0} could not be put on hold. Error:{1}", dto.koresecurities_id, txHoldResp.GenericError.message));
                    }
                    else if (txHoldResp.data == null || txHoldResp.data.id == null)
                    {
                        throw new Exception(string.Format("Invalid KCX format for hold transaction id"));
                    }
                    else
                        throw new Exception(string.Format("Unknown error putting shares on hold at KCX"));
                }

            }
            catch (Exception ex)
            {
                //3-Orders rejected
                //TO DOC KCX_RELEASE_SHARES_REJECTED
                throw new Exception(string.Format("Critical error putting {0} on hold:{1}", dto.number_of_shares, ex.Message));
            }
        }

        private string SellOnKoreConX(Shareholder seller, Security sec, TransferAgent transfAgent, double orderQty)
        {
            //1- First step, once we know the security <KoreSecurityId> to sell and the shareholder <KoreShareholderId> that is selling
            //we can build a HoldSharesDTO
            //The third component of the 2 previous Ids is the ATS Id in the KoreChain: ATS_ID <always a fixed number and populated just once>
            HoldSharesDTO dto = new HoldSharesDTO();
            dto.ats_id = transfAgent.TransferAgentATSId;
            dto.ats_transaction_id = string.Format("tr{0}{1}{2}", seller.KoreConXShareholderId.KoreShareholderId, sec.KoreConXSecurityId.KoreSecurityId, DateTime.Now.ToString("yyyyMMddhhmmss"));
            dto.koresecurities_id = sec.KoreConXSecurityId.KoreSecurityId;
            dto.last_updated_at = DateTime.Now.ToString("yyyy-MM-dd");
            dto.reason_code = HoldReasons.PendingSell.ToString();
            dto.number_of_shares = Convert.ToInt32(orderQty);
            dto.securities_holder_id = seller.KoreConXShareholderId.KoreShareholderId;

            //DoLog(string.Format("<@Transfer_agent_transactions - status=KCX_REQ_AVAILABLE_SHARES > - Requesting available shares for KoreShareholderId {0} and KoreSecurityId {1}", dto.securities_holder_id, dto.koresecurities_id), MessageType.Information);
            ValidationResponse resp = KCXHoldingsServiceClient.AvailableShares(dto.securities_holder_id, dto.koresecurities_id, dto.number_of_shares, dto.ats_id);

            if (resp.message != null)
            {
                //TO DOC --> KCX_ERROR_ENOUGH_SHARES
                throw new Exception(string.Format("Error requesting for available shares for Seller {0} (KoreShareholderId {1}) and Security {2} (KoreSecurityId {3})", seller.Name, seller.KoreConXShareholderId.KoreShareholderId, sec.Symbol, sec.KoreConXSecurityId.KoreSecurityId));
            }
            else
            {
                if (resp.data != null && resp.data.exists)
                {
                    //3-If everything is ok, we put the shares on hold
                    //TO DOC --> KCX_ENOUGH_SHARES_VALIDATED
                    return PutOnHoldKoreConX(dto);
                }
                else
                {
                    //4-If there are not enough shares, we update the status table, we inform the error. 
                    //TO DOC: KCX_NOT_ENOUGH_SHARES
                    throw new Exception(string.Format("Not Enough shares available for Seller {0} (KoreShareholderId {1}) and Security {2} (KoreSecurityId {3})", seller.Name, seller.KoreConXShareholderId.KoreShareholderId, sec.Symbol, sec.KoreConXSecurityId.KoreSecurityId));
                    
                }
            }
        }

        #endregion


        #endregion

        #region Public Methods

        public List<Trade> GetTradesToClear()
        {
            try
            {
                List<Order> ordersToClear = OrderManager.GetOrdersToCear();

                List<Trade> trades = new List<Trade>();
                foreach (Order buyOrder in ordersToClear.Where(x => x.Side == Side.Buy))
                {
                    if (!trades.Any(x => x.MatchingId == buyOrder.MatchingId))
                    {

                        Order sellOrder = ordersToClear.Where(x => x.Side == Side.Sell && x.MatchingId == buyOrder.MatchingId).FirstOrDefault();

                        Trade trade = new Trade();

                        Security sec = SecurityManager.GetSecurity(buyOrder.Symbol);
                        Shareholder buyer = ShareholderManager.GetShareholder(buyOrder.ShareholderId);
                        Shareholder seller = ShareholderManager.GetShareholder(sellOrder.ShareholderId);

                        ValidateTradesToClear(sec, buyer, seller, buyOrder, sellOrder);

                        trade.ExecutionDate = buyOrder.ExecutionTime.Value;
                        trade.SecurityId = sec.Id;
                        trade.Symbol = sec.Symbol;
                        trade.BuyerId = buyOrder.ShareholderId;
                        trade.BuyerName = buyer.Name;
                        trade.SellerId = sellOrder.ShareholderId;
                        trade.SellerName = seller.Name;
                        trade.TradeSize = Convert.ToInt32(Math.Min(buyOrder.ExecutedQty.Value, sellOrder.ExecutedQty.Value));
                        trade.TradePrice = Math.Min(buyOrder.TradePrice.Value, sellOrder.TradePrice.Value);
                        trade.BuyOrderId = buyOrder.Id;
                        trade.SellOrderId = sellOrder.Id;
                        trade.TradeNotional = trade.TradeSize * trade.TradePrice;
                        trade.Status = "Not Cleared";

                        trades.Add(trade);

                    }
                    else
                        throw new Exception(string.Format("There is not a counterparty for Matching Id {0}", buyOrder.MatchingId));
                }

                return trades;
            }
            catch (Exception ex)
            {
                DoLog(ex.Message, fwk.Common.enums.MessageType.Error);
                throw;
            }
        }

        public bool CreateTrade(int buyShareholderId, int sellShareholderId, int securityId, double tradeQuantity, double price)
        {
            Security sec = GetSecurity(securityId);
            Shareholder buyer = GetShareholder(buyShareholderId);
            Shareholder seller = GetShareholder(sellShareholderId);
            return false;
        }

        public string TransferShares(int buyShareholderId,int sellShareholderId,double tradeQuantity,int securityId,int sellOrderId)
        {
            try
            {
                Security sec = GetSecurity(securityId);
                Shareholder buyer = GetShareholder(buyShareholderId);
                Shareholder seller = GetShareholder(sellShareholderId);
                Order sellOrder = GetOrder(sellOrderId);

                TransferAgent transfAgent = null;

                if (sec.KoreConXSecurityId != null)
                    transfAgent = TransferAgentManager.GetTransferAgent(TransferAgentManager._KCX_ID);


                ValidateTransferShares(sec, buyer, seller, sellOrder, transfAgent);

                if (sec.KoreConXSecurityId != null)
                    return KCXDoTransferShares(sec, buyer, seller, sellOrder, Convert.ToInt32(tradeQuantity));
                else
                    return null;
            }
            catch (Exception ex)
            {
                DoLog(ex.Message, fwk.Common.enums.MessageType.Error);
                throw;
            }
        
        }

        public string OnSell(int sellShareholderId, int securityId, double orderQty)
        {
            try
            {
                Security sec = GetSecurity(securityId);

                Shareholder seller = GetShareholder(sellShareholderId);

                TransferAgent transfAgent = ValidateSell(seller, sec);

                if (sec.KoreConXSecurityId != null)
                    return SellOnKoreConX(seller, sec, transfAgent, orderQty);
                else
                    throw new Exception(string.Format("The security {0} does not have a valid transfer agent assigned", sec.Symbol));
            }
            catch (Exception ex)
            {
                DoLog(ex.Message, fwk.Common.enums.MessageType.Error);
                throw;
            }
        }

        public void OnBuy(int buyShareholderId, int securityId, double orderQty)
        {
            try
            {
                Security sec = GetSecurity(securityId);
                Shareholder buyer = GetShareholder(buyShareholderId);

                if (sec == null)
                    throw new Exception(string.Format("Could not find a security for Id {0}", securityId));

                if (buyer == null)
                    throw new Exception(string.Format("Could not find a firm (investor) for ShareholderId {0}", buyShareholderId));

                ValidateBuy(buyer, sec);
            }
            catch (Exception ex)
            {

                DoLog(ex.Message, fwk.Common.enums.MessageType.Error);
                throw;
            }
        }

        //Cancel,Release,Rejected
        public string OnOrderCancelledOrExpired(int sellOrderId, double releaseQty)
        {
            Order order = GetOrder(sellOrderId);

            if (order == null)
                throw new Exception(string.Format(" There is not a sell order for order id {0}", sellOrderId));

            try
            {

                Security sec = SecurityManager.GetSecurity(order.Symbol);

                Shareholder seller = GetShareholder(order.ShareholderId);

                if (sec == null)
                    throw new Exception(string.Format(" There is not a security for symbol {0}", sec.Symbol));


                if (seller == null)
                    throw new Exception(string.Format("Could not find a firm (investor) for ShareholderId {0}", order.ShareholderId));

                if (order.IsLiveOrder())
                {
                    TransferAgent transfAgent = ValidateSell(seller, sec);

                    if (sec.KoreConXSecurityId != null && sec.KoreConXSecurityId.KoreSecurityId != null)
                    {
                        KoreConXTransaction holdTx = order.GetHoldTransaction();

                        if (holdTx == null)
                            throw new Exception(string.Format("CRITICAL ERROR!!: There is not a hold transaction for order id {0}", sellOrderId));

                        return ReleaseKoreConX(order, seller, sec, transfAgent, holdTx, releaseQty);
                    }
                    else
                        throw new Exception(string.Format("Could not release shares for security {0} because it does not have a Transfer Agent assigned!", sec.Symbol));
                }
                else
                    throw new Exception(string.Format(" Invalid Status to relase the orderId {0}:{1}", order.Id, order.State.ToString()));
            }
            catch (Exception ex)
            {
                //TODO: Impl REASON!
                DoLog(ex.Message, fwk.Common.enums.MessageType.Error);
                KCXDoPersistKCXTransction(order, Convert.ToInt32(releaseQty), KoreConXTransactionState.KCX_RELEASE_SHARES_REJECTED);
                throw;
            }
        }

        #endregion
    }
}
