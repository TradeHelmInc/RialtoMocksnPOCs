using Rialto.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetTradesService
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
                string getTradesServiceURL = ConfigurationManager.AppSettings["GetTradesServiceURL"];

                TransferService transService = new TransferService(tradingCS, orderCS, kcxURL, getTradesServiceURL);

                transService.Run();

                Console.WriteLine(string.Format("Transfer Service successfully initialyzed at {0}", getTradesServiceURL));

                Console.ReadKey();
            }
            catch (Exception ex)
            {

                Console.WriteLine("Critical error initializing Transfer Service");

            }
        }
    }
}
