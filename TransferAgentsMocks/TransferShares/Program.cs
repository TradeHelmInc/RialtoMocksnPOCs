using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace TransferShares
{
    class Program
    {

        #region Protected Static Attributes

        protected static HttpSelfHostServer Server { get; set; }

        #endregion

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Initializing Transfer Service");

                string tradingCS = ConfigurationManager.AppSettings["TradingDBConnectionString"];
                string orderCS = ConfigurationManager.AppSettings["OrdersDBConnectionString"];
                string kcxURL = ConfigurationManager.AppSettings["KCXURL"];
                string transferServiceURL = ConfigurationManager.AppSettings["TransferServiceURL"];

                HttpSelfHostConfiguration config = new HttpSelfHostConfiguration(transferServiceURL);

                config.Routes.MapHttpRoute(name: "DefaultApi",
                                           routeTemplate: "{controller}/{action}",
                                           defaults: new { id = RouteParameter.Optional });

                Server = new HttpSelfHostServer(config);
                Server.OpenAsync().Wait();

                Console.WriteLine(string.Format("Transfer Service successfully initialyzed at {0}", transferServiceURL));
            }
            catch (Exception ex)
            {

                Console.WriteLine("Critical error initializing Transfer Service");
            
            }
        }

    }
}
