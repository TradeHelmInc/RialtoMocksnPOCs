using Rialto.Common.DTO.Services.Plaid;
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
            string baseURL = ConfigurationManager.AppSettings["BaseURL"];

            
            OnPlaidCredentialsLoadDTO plaidCredentials = new OnPlaidCredentialsLoadDTO()
            {
                PlaidAccessToken = ConfigurationManager.AppSettings["PlaidAccessToken"],
                UserIdentifier = ConfigurationManager.AppSettings["UserIdentifier"],
                PlaidItemId = ConfigurationManager.AppSettings["PlaidItemId"],
            };

            Console.WriteLine(string.Format("Invoking plad credentials service for user identifier (email) {0}", plaidCredentials.PlaidAccessToken));

            OnPlaidCredentialsLoadClient plaidCredentialsClient = new OnPlaidCredentialsLoadClient(baseURL);

            plaidCredentialsClient.OnPlaidCredentialsLoad(plaidCredentials);

            Console.WriteLine(" Finished invoking onbarding signal service");
            Console.ReadLine();
        }
    }
}
