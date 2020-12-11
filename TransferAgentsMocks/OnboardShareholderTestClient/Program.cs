using Rialto.Rialto.ServiceLayer.Client.KCX;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardShareholderTestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseURL = ConfigurationManager.AppSettings["BaseURL"];
            string koreChainIdToOnboard = ConfigurationManager.AppSettings["KoreChainIdToOnboard"];

            Console.WriteLine(string.Format("Invoking onboarding signal process for kore chain id {0}", koreChainIdToOnboard));

            OnKCXOnboardingApprovedServiceClient svcClient = new OnKCXOnboardingApprovedServiceClient( baseURL);

            svcClient.OnKCXOnboardingApproved(koreChainIdToOnboard);

            Console.WriteLine(" Finished invoking onbarding signal service");
            Console.ReadLine();
        }
    }
}
