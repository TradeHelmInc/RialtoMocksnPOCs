﻿using Newtonsoft.Json;
using Rialto.KoreConX.Common.DTO.Generic;
using Rialto.KoreConX.Common.DTO.Shareholders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.KoreConX.ServiceLayer.Client
{
    public class PersonsServiceClient: KCXBaseRESTClient
    {
        #region Constructors

        public PersonsServiceClient(string pBaseURL, string pUser, string pPassword)
        {

            BaseURL = pBaseURL;

            User = pUser;

            Password = pPassword;

        }

        #endregion

        #region Protected Static Consts

        protected static string _PERSON_SHOW = "/person/show/";

        #endregion

        #region Protected Methods

        protected PersonResponse ProcessDataResponse(BaseGetResponse resp)
        {
            if (resp.Resp != null)
            {
                try
                {
                    PersonResponse dataResp = JsonConvert.DeserializeObject<PersonResponse>(resp.Resp);
                    return dataResp;
                }
                catch (Exception ex)
                {
                    return new PersonResponse() { Resp = resp.Resp, message = resp.message };

                }
            }
            else
                return new PersonResponse() { Resp = resp.Resp, message = resp.message };

        }

        #endregion

        #region Public Methods

        public PersonResponse RequestPerson(string shareholderId, string company_id, string requestor_id)
        {
            string url = BaseURL + _PERSON_SHOW + shareholderId;;

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("company_id", company_id);
            param.Add("requestor_id", requestor_id);


            return ProcessDataResponse(DoGetJson(url, param));
        }

        #endregion
    }
}
