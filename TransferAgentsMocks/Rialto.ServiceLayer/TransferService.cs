﻿using Rialto.BusinessEntities;
using Rialto.LogicLayer;
using Rialto.ServiceLayer.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace Rialto.ServiceLayer
{
    public class TransferService
    {

        #region Protected Attributes

        protected static string TransferServiceURL { get; set; }

        protected TradingService TradingService {get;set;}

        protected static HttpSelfHostServer Server { get; set; }

        #endregion

        #region Constructors

        public TransferService(string pTradingCS, string pOrderCS, string pKcxURL, string pTransferServiceURL)
        {

            TransferServiceURL = pTransferServiceURL;

            TradingService = new TradingService(pTradingCS, pOrderCS, pKcxURL);

        }

        #endregion

        #region Protected Methods

        protected string OnTransferShares(int buyShareholderId, int sellShareholderId, double tradeQuantity,
                                                    int securityId, int sellOrderId)
        {
            return TradingService.TransferShares(buyShareholderId, sellShareholderId, tradeQuantity, securityId, sellOrderId);
        }

        protected List<Trade> OnGetTradesToClear()
        {
            return TradingService.GetTradesToClear();
        }

        #endregion

        #region Public Methods

        public void Run()
        {

            HttpSelfHostConfiguration config = new HttpSelfHostConfiguration(TransferServiceURL);

            TradingController.OnTransferShares += OnTransferShares;
            TradingController.OnGetTradesToClear += OnGetTradesToClear;

            config.Routes.MapHttpRoute(name: "DefaultApi",
                                       routeTemplate: "{controller}/{action}",
                                       defaults: new { id = RouteParameter.Optional });

            Server = new HttpSelfHostServer(config);
            Server.OpenAsync().Wait();
        
        }

        #endregion 
    }
}
