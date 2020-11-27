using Rialto.BusinessEntities;
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
    public class HoldingsService
    {
        #region Protected Attributes

        protected static string HoldingsServiceURL { get; set; }

        protected TradingService TradingService {get;set;}

        protected static HttpSelfHostServer Server { get; set; }

        #endregion

        #region Constructors

        public HoldingsService(string pTradingCS, string pOrderCS, string pKcxURL, string pHoldingsServiceURL)
        {

            HoldingsServiceURL = pHoldingsServiceURL;

            TradingService = new TradingService(pTradingCS, pOrderCS, pKcxURL);

        }

        #endregion

        #region Protected Methods

        protected string OnSell(int sellShareholderId, int securityId, double orderQty)
        {
            return TradingService.OnSell(sellShareholderId, securityId, orderQty);
        }

        protected string OnBuy(int sellShareholderId, int securityId, double orderQty)
        {
            return TradingService.OnSell(sellShareholderId, securityId, orderQty);
        }

        protected string OnOrderCancelledOrExpired(int sellOrderId, double releaseQty)
        {
            return TradingService.OnOrderCancelledOrExpired(sellOrderId, releaseQty);
        }

        #endregion

        #region Public Methods

        public void Run()
        {

            HttpSelfHostConfiguration config = new HttpSelfHostConfiguration(HoldingsServiceURL);

            HoldingController.OnSell += OnSell;
            HoldingController.OnBuy += OnBuy;
            HoldingController.OnOrderCancelledOrExpired += OnOrderCancelledOrExpired;
   

            config.Routes.MapHttpRoute(name: "DefaultApi",
                                       routeTemplate: "{controller}/{action}",
                                       defaults: new { id = RouteParameter.Optional });

            Server = new HttpSelfHostServer(config);
            Server.OpenAsync().Wait();
        
        }

        #endregion 
    }
}
