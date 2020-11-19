using MySql.Data.MySqlClient;
using Rialto.BusinessEntities;
using Rialto.BusinessEntities.KoreConX;
using Rialto.DataAccessLayer;
using Rialto.KoreConX.Common.DTO.Generic;
using Rialto.KoreConX.Common.DTO.Securities;
using Rialto.KoreConX.ServiceLayer.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.LogicLayer
{
    public class TradingService
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

        public TradingService(string pTradingConnectionString, string pOrderConnectionString,string pKCXURL)
        {
            SecurityManager = new SecurityManager(pTradingConnectionString);

            ShareholderManager = new ShareholderManager(pTradingConnectionString);

            OrderManager = new OrderManager(pOrderConnectionString, pTradingConnectionString);

            TransferAgentManager = new TransferAgentManager(pTradingConnectionString);

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
                throw new Exception(string.Format("There is not a SHAREHOLDER for id {0}", shareHolderId));


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

        #endregion


        #endregion

        #region Public Methods

        public List<Trade> GetTradesToClear()
        {
            List<Order> ordersToClear =  OrderManager.GetOrdersToCear();

            List<Trade> trades = new List<Trade>();
            foreach (Order buyOrder in ordersToClear.Where(x=>x.Side==Side.Buy))
            {
                if (!trades.Any(x => x.MatchingId == buyOrder.MatchingId))
                {

                    Order sellOrder = ordersToClear.Where(x => x.Side==Side.Sell && x.MatchingId == buyOrder.MatchingId).FirstOrDefault();

                    Trade trade = new Trade();

                    Security sec = SecurityManager.GetSecurity(buyOrder.Symbol);
                    Shareholder buyer= ShareholderManager.GetShareholder(buyOrder.ShareholderId);
                    Shareholder seller= ShareholderManager.GetShareholder(sellOrder.ShareholderId);

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

        public bool CreateTrade(int buyShareholderId, int sellShareholderId, int securityId, double tradeQuantity, double price)
        {
            Security sec = GetSecurity(securityId);
            Shareholder buyer = GetShareholder(buyShareholderId);
            Shareholder seller = GetShareholder(sellShareholderId);
            return false;
        }

        public string TransferShares(int buyShareholderId,int sellShareholderId,double tradeQuantity,int securityId,int sellOrderId)
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

        #endregion
    }
}
