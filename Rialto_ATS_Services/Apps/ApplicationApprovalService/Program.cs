using System;
using System.Configuration;
using System.Diagnostics;
using fwk.Common.enums;
using fwk.Common.util.encryption.common;
using fwk.Common.util.logger;
using Microsoft.Owin.Logging;
using Rialto.ServiceLayer;

namespace ApplicationApprovalService
{
    class Program: fwk.Common.interfaces.ILogger
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
                
                string ApplicationApprovalServiceURL = ConfigurationManager.AppSettings["ApplicationApprovalServiceURL"];
                
                logger.DoLog("Initializing Application Approval Service", MessageType.Information);

                ManagementService transService = new ManagementService(tradingCS, orderCS, ApplicationApprovalServiceURL, logger);
                
                transService.Run();

                logger.DoLog(string.Format("Application Approval Service successfully initialized at {0}", ApplicationApprovalServiceURL), MessageType.Information);

                Console.ReadKey();
            }
            catch (Exception ex)
            {
                logger.DoLog(string.Format("Critical error initializing Application Approval Service:{0}", ex.Message), MessageType.Error);
            }
        }

    }
}