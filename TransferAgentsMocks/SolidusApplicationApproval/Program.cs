using Rialto.Common.DTO.Services.Solidus;
using Rialto.Rialto.ServiceLayer.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidusApplicationApprovalTestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string baseURL = ConfigurationManager.AppSettings["BaseURL"];
                string email = ConfigurationManager.AppSettings["Email"];

                OnApplicationApprovalDTO apprDto = new OnApplicationApprovalDTO() { Email = email };

                Console.WriteLine(string.Format("Invoking Application Approval service for user identifier (email) {0}", email));

                OnApplicationApprovalClient appApproval = new OnApplicationApprovalClient(baseURL);

                appApproval.OnApplicationApproval(apprDto);

                Console.WriteLine(" Finished invoking application approval service");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Critical error invoking application approval service:{0}", ex.Message));
            }

            Console.ReadLine();
        }
    }
}
