using fwk.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolsShared.Logging;

namespace FerChainMock.DataAccessLayer
{
    

    public class FerChainServer : BaseDataAccessLayer
    {

        #region Protected Attributes

        protected string RESTURL { get; set; }

        #endregion

        #region Constructors


        public FerChainServer(string pRESTAdddress)
        {

            RESTURL = pRESTAdddress;
        
        }


        #endregion

        #region Protected Methods

        //protected override void LoadHistoryService()
        //{
        //    string url = RESTURL;

        //    try
        //    {

        //        DoLog(string.Format("Creating FerChain service for controller HistoryServiceController on URL {0}", url),
        //              MessageType.Information);

        //        HttpSelfHostConfiguration config = new HttpSelfHostConfiguration(url);

        //        config.Routes.MapHttpRoute(name: "DefaultApi",
        //                                   routeTemplate: "{controller}/{id}",
        //                                   defaults: new { id = RouteParameter.Optional });


        //        historyController.OnLog += DoLog;
        //        historyController.OverridenGet += historyControllerV2.Get;
        //        historyControllerV2.OnLog += DoLog;
        //        historyControllerV2.OnGetAllTrades += GetAllTrades;
        //        historyControllerV2.OnGetAllOrders += GetAllOrders;

        //        Server = new HttpSelfHostServer(config);
        //        Server.OpenAsync().Wait();
        //    }
        //    catch (Exception ex)
        //    {
        //        string error = ex.Message;
        //        Exception innerEx = ex.InnerException;

        //        while (innerEx != null)
        //        {
        //            error += "-" + innerEx.Message;
        //            innerEx = innerEx.InnerException;
        //        }


        //        DoLog(string.Format("Critical error creating history service for controller HistoryServiceController on URL {0}:{1}",
        //              url, error), MessageType.Error);
        //    }


        //}

        #endregion

        #region Public Methods

        public void Start()
        { 
        
        
        
        }


        #endregion
    }
}
