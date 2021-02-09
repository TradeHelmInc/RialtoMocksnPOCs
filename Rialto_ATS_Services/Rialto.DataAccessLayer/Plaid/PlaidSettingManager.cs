using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Rialto.BusinessEntities;
using Rialto.BusinessEntities.Plaid;

namespace Rialto.DataAccessLayer.Plaid
{
    public class PlaidSettingManager: BaseManager
    {
        #region Constructors

        public PlaidSettingManager(string pConnectionString)
        {
            ConnectionString = pConnectionString;
        }

        #endregion

        #region Private Static Querys

        private static string _SP_GET_PLAID_SETTINGS = "get_plaid_settings";

        #endregion
        
          #region Private Methods

        private PlaidSetting BuildPlaidSetting(MySqlDataReader reader)
        {
            PlaidSetting setting = new PlaidSetting()
            {
             
                EnvName = GetSafeString(reader, "env_name"),
                URL = GetSafeString(reader, "url"),
                ClientId = GetSafeString(reader, "client_id"),
                Secret = GetSafeString(reader, "secret"),
                Enabled = GetSafeBoolean(reader, "enabled")
            };

            return setting;
        }

        #endregion
        
        #region Public Methods
        
        
        public PlaidSetting GetEnabledPlaidSetting()
        {
            //DatabaseConnection = new MySqlConnection(ConnectionString);
            MySqlCommand cmd = new MySqlCommand(_SP_GET_PLAID_SETTINGS, new MySqlConnection(ConnectionString));
            cmd.CommandTimeout = 60;

            cmd.CommandType = CommandType.StoredProcedure;

            
            cmd.Connection.Open();

            // Open DB
            MySqlDataReader reader;
            PlaidSetting setting = null;

            try
            {
                // Run Query
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        setting=BuildPlaidSetting(reader);
                    }
                }
            }
            finally
            {
                cmd.Connection.Close();
            }

            return setting;
        }
        
        
        #endregion
    }
}