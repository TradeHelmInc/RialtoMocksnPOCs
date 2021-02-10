using System;
using System.Configuration;
using fwk.Common.enums;
using fwk.Common.interfaces;
using fwk.Common.util.logger;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Rialto.ServiceLayer;
using Rialto.ServiceLayer.config;

namespace OnboardingApp2
{
    class Program : ILogger
    {
        #region Protected Attributes
        protected Logger Logger { get; set; }

        #endregion

        #region Public  Methods
        public void DoLog(string msg, MessageType type)
        {
            Logger.DoLog(msg,type);
        }

        #endregion

        
        static void Main(string[] args)
        {
            Program logger = new Program() {Logger = new Logger(ConfigurationManager.AppSettings["DebugLevel"])};

            try
            {
                string appName = ConfigurationManager.AppSettings["ApplicationName"];
                
                string tradingCS = ConfigurationManager.AppSettings["TradingDBConnectionString"];
                string orderCS = ConfigurationManager.AppSettings["OrdersDBConnectionString"];
                string kcxName = ConfigurationManager.AppSettings["KCXName"];
                string solidusURL = ConfigurationManager.AppSettings["SolidusURL"];
                string onboardingServiceURL = ConfigurationManager.AppSettings["OnboardingServiceURL"];
                //string kcxKeyAndIVPath = ConfigurationManager.AppSettings["KCXKeyAndIVPath"];
                
                // string kcxEncryptedKeyAndIVPath = ConfigurationManager.AppSettings["KCXEncryptedKeyAndIVPath"];
                // string kcxDeccryptedKeyAndIVPath = ConfigurationManager.AppSettings["KCXDecryptedKeyAndIVPath"];
                // bool AESKeyEncrypted = Convert.ToBoolean(ConfigurationManager.AppSettings["AESSKeyEncrypted"]);
                // string kcxKeyAndIV = AESKeyEncrypted ? FileLoader.GetFileContent(kcxEncryptedKeyAndIVPath) : FileLoader.GetFileContent(kcxDeccryptedKeyAndIVPath);

                string kcxEncryptedRSAPrivateKeyPath = ConfigurationManager.AppSettings["KCXEncryptedRSAPrivateKeyPath"];
                string kcxDeccryptedRSAPrivateKeyPath = ConfigurationManager.AppSettings["KCXDeccryptedRSAPrivateKeyPath"];
                string kcxPublicKeyPath = ConfigurationManager.AppSettings["KCXPublicKeyPath"];

                
                bool RSAKeyEncrypted = Convert.ToBoolean(ConfigurationManager.AppSettings["RSAPrivateKeyEncrypted"]);

                logger.DoLog("Initializing Onboarding app service", MessageType.Information);
                
                //logger.DoLog("Extracting AES key", MessageType.Information);
                string kcxRSAPrivateKeyPath = RSAKeyEncrypted ? kcxEncryptedRSAPrivateKeyPath : kcxDeccryptedRSAPrivateKeyPath;
                //string kcxKeyAndIV = FileLoader.GetFileContent(kcxKeyAndIVPath);

                ManagementService transService = new ManagementService(appName,tradingCS, orderCS, onboardingServiceURL, kcxName,kcxPublicKeyPath,
                                                                        kcxRSAPrivateKeyPath, RSAKeyEncrypted, solidusURL, logger);

                transService.Run();

                logger.DoLog(string.Format("Onboarding Service successfully initialized at {0}", onboardingServiceURL), MessageType.Information);

                Console.ReadKey();
            }
            catch (Exception ex)
            {
                logger.DoLog(string.Format("Critical error initializing Onboarding Service:{0}", ex.Message), MessageType.Error);
            }
        }
    }
}