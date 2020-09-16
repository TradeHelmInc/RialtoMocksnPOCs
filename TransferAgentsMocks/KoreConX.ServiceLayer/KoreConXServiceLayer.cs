using fwk.Common.enums;
using fwk.ServiceLayer;
using KoreConX.Common.DTO.Generic;
using Mocks.KoreConX.Common.DTO.Generic;
using Mocks.KoreConX.Common.DTO.Holdings;
using Mocks.KoreConX.LogicLayer;
using Mocks.KoreConX.ServiceLayer.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace Mocks.KoreConX.ServiceLayer
{
    public class KoreConXServiceLayer : BaseServiceLayer
    {
        #region Protected Attributes

        protected string RESTURL { get; set; }

        protected HttpSelfHostServer Server { get; set; }

        protected HoldingsLogicLayer HoldingsLogicLayer { get; set; }

        #endregion

        #region Constructors


        public KoreConXServiceLayer(string pRESTAdddress)
        {
            RESTURL = pRESTAdddress;

            HoldingsLogicLayer = new HoldingsLogicLayer();

            LoadDocsService();
        }

        #endregion

        #region Events

        protected ValidationResponse OnAvailableShares(string koreShareholderId, string koreSecurityId, int qty, string koreATSId)
        {
            bool resp = HoldingsLogicLayer.HasEnoughShares(koreShareholderId, koreSecurityId, qty);

            return new ValidationResponse() { data = new ExistsEntity() { exists = resp } };
        }


        protected TransactionResponse OnReleaseShares(ReleaseSharesDTO releaseDto)
        {

            string txId = HoldingsLogicLayer.ReleaseShares(releaseDto);

            return new TransactionResponse() { data = new IdEntity() { id = txId } };
        
        }


        protected TransactionResponse OnHoldShares(HoldSharesDTO holdDto)
        {
            string txId = HoldingsLogicLayer.HoldShares(holdDto);

            return new TransactionResponse() { data = new IdEntity() { id = txId } };
        }

        #endregion

        #region Protected Methods

        protected  void LoadDocsService()
        {
            string url = RESTURL;

            try
            {

                DoLog(string.Format("Creating KoreConX Service Layer  on URL {0}", url),MessageType.Information);

                HttpSelfHostConfiguration config = new HttpSelfHostConfiguration(url);

                config.Routes.MapHttpRoute(name: "DefaultApi",
                                           routeTemplate: "{controller}/{action}",
                                           defaults: new { id = RouteParameter.Optional });


             
                holdingsController.OnLog += DoLog;
                holdingsController.OnAvailableShares += OnAvailableShares;
                holdingsController.OnHoldShares += OnHoldShares;
                holdingsController.OnReleaseShares += OnReleaseShares;
               

                Server = new HttpSelfHostServer(config);
                Server.OpenAsync().Wait();
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                Exception innerEx = ex.InnerException;

                while (innerEx != null)
                {
                    error += "-" + innerEx.Message;
                    innerEx = innerEx.InnerException;
                }


                DoLog(string.Format("Critical error creating Docs service for controller DocsServiceController on URL {0}:{1}",
                      url, error), MessageType.Error);
            }


        }

        #endregion

        #region Public Methods

        public void Start()
        {



        }


        #endregion
    }
}
