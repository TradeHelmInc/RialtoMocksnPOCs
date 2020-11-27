using Newtonsoft.Json;
using Rialto.BusinessEntities;
using Rialto.Rialto.Common.DTO.Generic;
using Rialto.Rialto.ServiceLayer.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettleTradesPOC
{
    class Program
    {
        #region Protected Attributes

        protected static string TradingCS = ConfigurationManager.AppSettings["TradingDBConnectionString"];
        protected static string OrderCS = ConfigurationManager.AppSettings["OrdersDBConnectionString"];
        protected static string KCXURL = ConfigurationManager.AppSettings["KCXURL"];

        protected static Trade[] Trades { get; set; }

        protected static TransferServiceClient TransferServiceClient { get; set; }

        protected static GetTradesServiceClient GetTradesServiceClient { get; set; }

        #endregion

        #region Private Methods

        private static void DoLog(string message)
        {
            Console.WriteLine(message);
        }

        private static void ShowCommands()
        {
            Console.WriteLine();
            Console.WriteLine("-------------- Enter Commands to Invoke -------------- ");
            Console.WriteLine("CreateTrade <buyerId> <sellerId> <securityId> <qty> <price>");
            Console.WriteLine("GetTradesToClear");
            Console.WriteLine("ClearTrade <TradeNumber>");
            Console.WriteLine("-CLEAR");
            Console.WriteLine();

        }

        private static void ProcessCreateTrade(string[] param)
        {
           

            if (param.Length >= 5)
            {
               


            }
            else
                DoLog(string.Format("Missing mandatory parameters for ProcessCreateTrade message"));

        }

        private static void ProcessGetTradesToClear(string[] param)
        {


            if (param.Length >= 1)
            {
                GetResponse resp = GetTradesServiceClient.GetTrades();

                if (resp.Success)
                {
                    Trades = JsonConvert.DeserializeObject<Trade[]>(resp.data.ToString());

                    int i = 0;
                    Console.WriteLine("============= Trades to Clear =============");
                    foreach (Trade trade in Trades)
                    {
                        DoLog(string.Format("[{0}]-Buyer={1} Seller={2} Security={3} Qty={4} Price={5}", i, trade.BuyerName, trade.SellerName, trade.Symbol, trade.TradeSize, trade.TradePrice));
                        i++;
                    }
                }
                else
                {
                    DoLog(string.Format("Error accessing remote service:{0}", resp.Error.msg));
                
                }
            }
            else
                DoLog(string.Format("Missing mandatory parameters for GetTradesToClear message"));

        }


        private static void ProcessClearTrade(string[] param)
        {

            if (param.Length >= 2)
            {
                int indexToClear = Convert.ToInt32(param[1]);

                if (Trades != null && indexToClear < Trades.Length)
                {
                    Trade trade = Trades[indexToClear];

                    try
                    {
                        TransactionResponse trans = TransferServiceClient.TransferShares(trade.BuyerId, trade.SellerId, trade.TradeSize, trade.SecurityId, trade.SellOrderId);

                        if (trans.Success && trans.Id != null)
                            DoLog(string.Format("Trade {0} successully cleared. KCX Transaction Id = {1}", trade.MatchingId, trans.Id.id));
                        else
                            DoLog(string.Format("ERROR clearing trade {0} : {1}", trade.MatchingId, "Unknwon trade Id"));
                    }
                    catch (Exception ex)
                    {
                        DoLog(string.Format("ERROR clearing trade {0} : {1}", trade.MatchingId, ex.Message));

                    }
                }
                else
                    DoLog(string.Format("There is not an trade whose index is {0}", param[1]));
            }
            else
                DoLog(string.Format("Missing mandatory parameters for GetTradesToClear message"));

        }

        private static void ProcessCommand(string cmd)
        {

            string[] param = cmd.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            string mainCmd = param[0];

            if (mainCmd == "CreateTrade")
            {
                ProcessCreateTrade(param);
            }

            else if (mainCmd == "GetTradesToClear")
            {
                ProcessGetTradesToClear(param);
            }
            else if (mainCmd == "ClearTrade")
            {
                ProcessClearTrade(param);
            }
           
            else
                DoLog(string.Format("Unknown command {0}", mainCmd));
        }

        #endregion

        static void Main(string[] args)
        {

            string TransferServiceREST = ConfigurationManager.AppSettings["TransferServiceURL"];
            string GetTradesServiceREST = ConfigurationManager.AppSettings["GetTradesServiceURL"];

            TransferServiceClient = new TransferServiceClient(TransferServiceREST);
            GetTradesServiceClient = new GetTradesServiceClient(GetTradesServiceREST);
            try
            {
                ShowCommands();

                while (true)
                {
                    string cmd = Console.ReadLine();
                    ProcessCommand(cmd);
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                DoLog(string.Format("Critical Error: {0}", ex.Message));
                Console.ReadKey();

            }


        }
    }
}
