using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Rialto.DataAccessLayer
{
    public class BaseManager
    {
        #region Protected Attributes

        protected MySqlConnection DatabaseConnection { get; set; }

        protected string ConnectionString { get; set; }

        #endregion
    }
}
