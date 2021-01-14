﻿using fwk.Common.enums;
using fwk.Common.interfaces;
using fwk.Common.util.encryption.common;
using Rialto.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fwk.Common.util.logger;

//using ToolsShared.Logging;

namespace OnboardingApp
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
                string tradingCS = ConfigurationManager.AppSettings["TradingDBConnectionString"];
                string orderCS = ConfigurationManager.AppSettings["OrdersDBConnectionString"];
                string kcxURL = ConfigurationManager.AppSettings["KCXURL"];
                string solidusURL = ConfigurationManager.AppSettings["SolidusURL"];
                string onboardingServiceURL = ConfigurationManager.AppSettings["OnboardingServiceURL"];
                //string kcxKeyAndIVPath = ConfigurationManager.AppSettings["KCXKeyAndIVPath"];
                
                string kcxEncryptedKeyAndIVPath = ConfigurationManager.AppSettings["KCXEncryptedKeyAndIVPath"];
                string kcxDeccryptedKeyAndIVPath = ConfigurationManager.AppSettings["KCXDecryptedKeyAndIVPath"];

                string kcxEncryptedRSAPrivateKeyPath = ConfigurationManager.AppSettings["KCXEncryptedRSAPrivateKeyPath"];
                string kcxDeccryptedRSAPrivateKeyPath = ConfigurationManager.AppSettings["KCXDeccryptedRSAPrivateKeyPath"];

                bool AESKeyEncrypted = Convert.ToBoolean(ConfigurationManager.AppSettings["AESSKeyEncrypted"]);
                bool RSAKeyEncrypted = Convert.ToBoolean(ConfigurationManager.AppSettings["RSAPrivateKeyEncrypted"]);

                logger.DoLog("Initializing Onboarding app service", MessageType.Information);
                logger.DoLog("Extracting AES key", MessageType.Information);
                string kcxKeyAndIV = AESKeyEncrypted ? FileLoader.GetFileContent(kcxEncryptedKeyAndIVPath) : FileLoader.GetFileContent(kcxDeccryptedKeyAndIVPath);

                string kcxRSAPrivateKeyPath = RSAKeyEncrypted ? kcxEncryptedRSAPrivateKeyPath : kcxDeccryptedRSAPrivateKeyPath;
                
                //string kcxKeyAndIV = FileLoader.GetFileContent(kcxKeyAndIVPath);

                ManagementService transService = new ManagementService(tradingCS, orderCS, onboardingServiceURL, kcxURL, kcxKeyAndIV, AESKeyEncrypted,
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