using Rialto.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnSellApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Initializing Transfer Service");

                string tradingCS = ConfigurationManager.AppSettings["TradingDBConnectionString"];
                string orderCS = ConfigurationManager.AppSettings["OrdersDBConnectionString"];
                string kcxURL = ConfigurationManager.AppSettings["KCXURL"];
                string holdingServiceURL = ConfigurationManager.AppSettings["HoldingsServiceURL"];

                HoldingsService holdingsService = new HoldingsService(tradingCS, orderCS, kcxURL, holdingServiceURL);

                holdingsService.Run();

                Console.WriteLine(string.Format("OnSell Service successfully initialyzed at {0}", holdingServiceURL));

                Console.ReadKey();
            }
            catch (Exception ex)
            {

                Console.WriteLine("Critical error initializing Transfer Service");

            }
        }
    }
}
