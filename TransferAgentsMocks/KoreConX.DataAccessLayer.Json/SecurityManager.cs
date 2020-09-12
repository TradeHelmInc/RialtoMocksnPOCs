using KoreConX.BusinessEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreConX.DataAccessLayer.Json
{
    public class SecurityManager
    {
        #region Constructors

        public SecurityManager()
        {

            Securities = LoadSecurities();

            //TODO:Incorporate companies
        }

        #endregion

        #region Protected Attributes

        protected Security[] Securities { get; set; }

        #endregion


        #region Private Methods

        private Security[] LoadSecurities()
        {
            string strSecurities = File.ReadAllText(@".\input\Securities.json");

            return JsonConvert.DeserializeObject<Security[]>(strSecurities);

        }

        #endregion
    }
}
