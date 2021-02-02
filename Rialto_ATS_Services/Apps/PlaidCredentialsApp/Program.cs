using System;
using System.Configuration;
using fwk.Common.enums;
using fwk.Common.interfaces;
using fwk.Common.util.encryption.common;
using fwk.Common.util.logger;
using Rialto.ServiceLayer;

namespace PlaidCredentialsLoadApp
{
    class Program: ILogger
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
                
                string plaidCredentialsLoadServiceURL = ConfigurationManager.AppSettings["PlaidCredentialsLoadServiceURL"];
                
                string RSAPublicKeyPath = ConfigurationManager.AppSettings["RSAPublicKeyPath"];
              

                logger.DoLog("Initializing Plaid Credential Load app service", MessageType.Information);
                logger.DoLog("Extracting RSA public key", MessageType.Information);
                string RSAPublicKey = FileLoader.GetFileContent(RSAPublicKeyPath);

                PlaidService plaidService = new PlaidService(tradingCS, orderCS,plaidCredentialsLoadServiceURL,RSAPublicKey , logger);

                plaidService.Run();

                logger.DoLog(string.Format("Onboarding Service successfully initialized at {0}", plaidCredentialsLoadServiceURL), MessageType.Information);

                Console.ReadKey();
            }
            catch (Exception ex)
            {
                logger.DoLog(string.Format("Critical error initializing Onboarding Service:{0}", ex.Message), MessageType.Error);
            }
        }
    }
}