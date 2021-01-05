using fwk.Common.util.encryption.common;
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

            bool sendKeyAndIV =Convert.ToBoolean( ConfigurationManager.AppSettings["SendKeyAndIV"]);
            bool sendKeyAndIVEncrypted = Convert.ToBoolean(ConfigurationManager.AppSettings["SendKeyAndIVEncrypted"]);

            if (!sendKeyAndIV)
            {

                Console.WriteLine(string.Format("Invoking onboarding signal process for kore chain id {0} without key and IV", koreChainIdToOnboard));

                OnKCXOnboardingApprovedServiceClient svcClient = new OnKCXOnboardingApprovedServiceClient(baseURL);

                svcClient.OnKCXOnboardingApproved(koreChainIdToOnboard);

            }
            else
            {
                string key, iv;

                if (sendKeyAndIVEncrypted)
                {

                    key = FileLoader.GetFileContent(ConfigurationManager.AppSettings["EncKeyPath"]);
                    iv = FileLoader.GetFileContent(ConfigurationManager.AppSettings["EncIVPath"]);
                }
                else
                {
                    key = FileLoader.GetFileContent(ConfigurationManager.AppSettings["DecKeyPath"]);
                    iv = FileLoader.GetFileContent(ConfigurationManager.AppSettings["DecIVPath"]);
                
                }


                Console.WriteLine(string.Format("Invoking onboarding signal process for kore chain id {0} with key {1} and IV {2}", koreChainIdToOnboard, key, iv));
                OnKCXOnboardingApprovedServiceClient svcClient = new OnKCXOnboardingApprovedServiceClient(baseURL);
                svcClient.OnKCXOnboardingApproved(koreChainIdToOnboard, key, iv);
            }
            

            Console.WriteLine(" Finished invoking onbarding signal service");
            Console.ReadLine();
        }
    }
}
