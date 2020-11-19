using Rialto.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferSharesApp
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
                string transferServiceURL = ConfigurationManager.AppSettings["TransferServiceURL"];

                TransferService transService = new TransferService(tradingCS, orderCS, kcxURL, transferServiceURL);

                transService.Run();

                Console.WriteLine(string.Format("Transfer Service successfully initialyzed at {0}", transferServiceURL));

                Console.ReadKey();
            }
            catch (Exception ex)
            {

                Console.WriteLine("Critical error initializing Transfer Service");
            
            }
        }
    }
}
