using fwk.Common.interfaces;
using Rialto.BusinessEntities;
using Rialto.LogicLayer;
using Rialto.ServiceLayer.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Rialto.ServiceLayer
{
    /*public class TransferService
    {

        #region Protected Attributes

        protected static string TransferServiceURL { get; set; }

        protected TradingLogic TradingLogic {get;set;}

        protected static HttpSelfHostServer Server { get; set; }

        protected ILogger Logger { get; set; }

        #endregion

        #region Constructors

        public TransferService(string pTradingCS, string pOrderCS, string pKcxURL, string pTransferServiceURL, ILogger pLogger)
        {

            TransferServiceURL = pTransferServiceURL;

            TradingLogic = new TradingLogic(pTradingCS, pOrderCS, pKcxURL, pLogger);

            Logger = pLogger;

        }

        #endregion

        #region Protected Methods

        protected string OnTransferShares(int buyShareholderId, int sellShareholderId, double tradeQuantity,
                                                    int securityId, int sellOrderId)
        {
            return TradingLogic.TransferShares(buyShareholderId, sellShareholderId, tradeQuantity, securityId, sellOrderId);
        }

        protected List<Trade> OnGetTradesToClear()
        {
            return TradingLogic.GetTradesToClear();
        }

        #endregion

        #region Public Methods

        public void Run()
        {

            HttpSelfHostConfiguration config = new HttpSelfHostConfiguration(TransferServiceURL);

            TradingController.OnTransferShares += OnTransferShares;
            TradingController.OnGetTradesToClear += OnGetTradesToClear;

            TradingController.Logger = Logger;

            config.Routes.MapHttpRoute(name: "DefaultApi",
                                       routeTemplate: "{controller}/{action}",
                                       defaults: new { id = RouteParameter.Optional });

            Server = new HttpSelfHostServer(config);
            Server.OpenAsync().Wait();
        
        }

        #endregion 
    }*/
}
