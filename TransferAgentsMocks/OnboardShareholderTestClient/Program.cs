using fwk.Common.util.encryption.common;
using Rialto.Rialto.Common.DTO.Generic;
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
            string companyKoreChainId = ConfigurationManager.AppSettings["CompanyKoreChainId"];

            bool sendKeyAndIV =Convert.ToBoolean( ConfigurationManager.AppSettings["SendKeyAndIV"]);
            bool sendKeyAndIVEncrypted = Convert.ToBoolean(ConfigurationManager.AppSettings["SendKeyAndIVEncrypted"]);

            string EncryptionMode = ConfigurationManager.AppSettings["EncryptionMode"];


            if (EncryptionMode == "1")
            {
                if (!sendKeyAndIV)
                {

                    Console.WriteLine(string.Format("Invoking onboarding signal process for kore chain id {0} without key and IV", koreChainIdToOnboard));

                    OnKCXOnboardingApprovedServiceClient svcClient = new OnKCXOnboardingApprovedServiceClient(baseURL);

                    svcClient.OnKCXOnboardingApproved(koreChainIdToOnboard, companyKoreChainId);

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
                    svcClient.OnKCXOnboardingApproved(koreChainIdToOnboard, companyKoreChainId, key, iv);
                }
            }
            else if (EncryptionMode == "2")
            {
                Console.WriteLine(string.Format("Invoking onboarding signal process for 4096 bits message (2 parts)"));
                OnKCXOnboardingApprovedServiceClient svcClient = new OnKCXOnboardingApprovedServiceClient(baseURL);

                string part1 = ConfigurationManager.AppSettings["4096Enc_Part1"];
                string part2 = ConfigurationManager.AppSettings["4096Enc_Part2"];
                TransactionResponse txResp =  svcClient.OnKCXOnboardingApproved_4096(part1, part2);

                if (txResp.Success)
                {
                    Console.WriteLine(" Finished successfully invoking onbarding signal service");
                }
                else
                    Console.WriteLine("Error invoking onboarding service:" + txResp.Error.msg);
            }
            
            Console.ReadLine();
        }
    }
}
