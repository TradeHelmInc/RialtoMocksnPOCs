using Rialto.Common.DTO.Services.Plaid;
using Rialto.Rialto.Common.DTO.Generic;
using Rialto.Rialto.ServiceLayer.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaidCredentailsLoadTestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string baseURL = ConfigurationManager.AppSettings["BaseURL"];


                OnPlaidCredentialsLoadDTO plaidCredentials = new OnPlaidCredentialsLoadDTO()
                {
                    PlaidAccessToken = ConfigurationManager.AppSettings["PlaidAccessToken"],
                    UserIdentifier = ConfigurationManager.AppSettings["UserIdentifier"],
                    PlaidItemId = ConfigurationManager.AppSettings["PlaidItemId"],
                };

                Console.WriteLine(string.Format("Invoking plad credentials service for user identifier (email) {0}", plaidCredentials.PlaidAccessToken));

                OnPlaidCredentialsLoadClient plaidCredentialsClient = new OnPlaidCredentialsLoadClient(baseURL);

                TransactionResponse txResp = plaidCredentialsClient.OnPlaidCredentialsLoad(plaidCredentials);

                if (txResp.Success)
                {
                    Console.WriteLine(" Finished successfully invoking Plaid credentials load");
                }
                else
                    Console.WriteLine("Error invoking Plaid credentials load:" + txResp.Error.msg);

                Console.WriteLine(" Finished invoking onbarding signal service");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Critical error invoking plaid credentials load service:{0}", ex.Message));
            }

            Console.ReadLine();
        }
    }
}
