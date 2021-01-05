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

        #region Protected Methods


        protected string GetSafeString(MySqlDataReader rdr, string col)
        {
            var ordinal = rdr.GetOrdinal(col);
            if (!rdr.IsDBNull(ordinal))
                return rdr.GetString(col);
            else

                return null;
        
        }

        protected DateTime? GetSafeDateTime(MySqlDataReader rdr, string col)
        {
            var ordinal = rdr.GetOrdinal(col);
            if (!rdr.IsDBNull(ordinal))
                return rdr.GetDateTime(col);
            else

                return null;

        }


        protected double? GetNullSafeDouble(MySqlDataReader rdr, string col)
        {
            var ordinal = rdr.GetOrdinal(col);
            if (!rdr.IsDBNull(ordinal))
                return rdr.GetDouble(col);
            else

                return null;

        }

        protected int? GetNullSafeInt(MySqlDataReader rdr, string col)
        {
            var ordinal = rdr.GetOrdinal(col);
            if (!rdr.IsDBNull(ordinal))
                return rdr.GetInt32(col);
            else

                return null;

        }



        protected long? GetNullSafeLong(MySqlDataReader rdr, string col)
        {
            var ordinal = rdr.GetOrdinal(col);
            if (!rdr.IsDBNull(ordinal))
                return rdr.GetInt64(col);
            else

                return null;

        }


        protected double GetSafeDouble(MySqlDataReader rdr, string col)
        {
            var ordinal = rdr.GetOrdinal(col);
            if (!rdr.IsDBNull(ordinal))
                return rdr.GetDouble(col);
            else

                return 0;

        }

        protected decimal GetSafeDecimal(MySqlDataReader rdr, string col)
        {
            var ordinal = rdr.GetOrdinal(col);
            if (!rdr.IsDBNull(ordinal))
                return rdr.GetDecimal(col);
            else

                return 0;

        }


        protected bool GetSafeBoolean(MySqlDataReader rdr, string col)
        {
            var ordinal = rdr.GetOrdinal(col);
            if (!rdr.IsDBNull(ordinal))
                return rdr.GetBoolean(col);
            else

                return false;

        }

        #endregion
    }
}
