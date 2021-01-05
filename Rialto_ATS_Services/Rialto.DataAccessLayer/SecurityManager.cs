using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rialto.BusinessEntities;
using MySql.Data.MySqlClient;

namespace Rialto.DataAccessLayer
{
    public class SecurityManager : BaseManager
    {
        #region Constructors

        public SecurityManager(string pConnectionString)
        {
            ConnectionString = pConnectionString;
        }

        #endregion


        #region Private Static Consts

        private static string _KCX_ID = "KoreConX";

        private static string _HARCODED_COMPANY_ID = "0ceed471e79b07a937c8ca1d769469748dbc2a7958886b31b547cdbd93908538";

        #endregion

        #region Private Static Querys

        private static string _GET_SECURITY_BY_ID_QUERY = @"SELECT id,name,symbol,cusip FROM secondarytrading.fund_security WHERE id={0}";

        private static string _GET_SECURITY_BY_SYMBOL_QUERY = @"SELECT id,name,symbol,cusip FROM secondarytrading.fund_security WHERE symbol='{0}'";

        private static string _GET_KORECONX_SECURITY_ID_QUERY = @"SELECT kcx_ids.chain_id as kore_security_id
                                                                  FROM secondarytrading.security_chain kcx_ids
                                                                  INNER JOIN secondarytrading.transfer_agent ta ON ta.id=kcx_ids.transfer_agent_id
                                                                  INNER JOIN secondarytrading.fund_security sec on sec.security_chain_id=kcx_ids.id
                                                                  WHERE ta.transfer_agent_name='{0}' 
                                                                  AND sec.id={1} and kcx_ids.enabled=1";

        #endregion

        #region Public Methods

        public Security GetSecurity(int Id)
        {
            DatabaseConnection = new MySqlConnection(ConnectionString);
            MySqlCommand commandDatabase = new MySqlCommand(string.Format(_GET_SECURITY_BY_ID_QUERY, Id), DatabaseConnection);
            commandDatabase.CommandTimeout = 60;
            MySqlDataReader reader;
            Security sec = null;

           
            // Open DB
            DatabaseConnection.Open();

            try
            {
                // Run Query
                reader = commandDatabase.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        // En nuestra base de datos, el array contiene:  ID 0, FIRST_NAME 1,LAST_NAME 2, ADDRESS 3
                        // Hacer algo con cada fila obtenida
                        sec = new Security() { Id = reader.GetInt32(0), Name = reader.GetString("name"), CUSIP = reader.GetString("cusip"), Symbol = reader.GetString("symbol") };
                    }
                }
            }
            finally
            {
                DatabaseConnection.Close();
            }

            return sec;
        }

        public Security GetSecurity(string symbol)
        {
            DatabaseConnection = new MySqlConnection(ConnectionString);
            MySqlCommand commandDatabase = new MySqlCommand(string.Format(_GET_SECURITY_BY_SYMBOL_QUERY, symbol), DatabaseConnection);
            commandDatabase.CommandTimeout = 60;
            MySqlDataReader reader;
            Security sec = null;


            // Open DB
            DatabaseConnection.Open();

            try
            {
                // Run Query
                reader = commandDatabase.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        // En nuestra base de datos, el array contiene:  ID 0, FIRST_NAME 1,LAST_NAME 2, ADDRESS 3
                        // Hacer algo con cada fila obtenida
                        sec = new Security() { Id = reader.GetInt32(0), Name = reader.GetString("name"), CUSIP = reader.GetString("cusip"), Symbol = reader.GetString("symbol") };
                    }
                }
            }
            finally
            {
                DatabaseConnection.Close();
            }

            return sec;
        }


        public KoreConXSecurityId GetKoreConXSecurityId(int securityId)
        {
            DatabaseConnection = new MySqlConnection(ConnectionString);
            MySqlCommand commandDatabase = new MySqlCommand(string.Format(_GET_KORECONX_SECURITY_ID_QUERY, _KCX_ID, securityId), DatabaseConnection);
            commandDatabase.CommandTimeout = 60;
            MySqlDataReader reader;
            KoreConXSecurityId kcxId = null;

            int count = 0;
            // Open DB
            DatabaseConnection.Open();

            try
            {
                // Run Query
                reader = commandDatabase.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        kcxId = new KoreConXSecurityId() { CompanyId = _HARCODED_COMPANY_ID, KoreSecurityId = reader.GetString("kore_security_id") };
                        count++;
                    }
                }
            }
            finally
            {
                DatabaseConnection.Close();
            }

            if (count > 1)
                throw new Exception(string.Format("Inconsistent DB state detected: There are more than 1 Kore Security Ids for security {0}", securityId));

            return kcxId;
        }

        #endregion

    }
}
