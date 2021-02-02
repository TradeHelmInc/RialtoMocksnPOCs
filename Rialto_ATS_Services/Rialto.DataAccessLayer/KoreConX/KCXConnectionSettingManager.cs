using System.Data;
using MySql.Data.MySqlClient;
using Rialto.BusinessEntities.KoreConX;

namespace Rialto.DataAccessLayer.KoreConX
{
    public class KCXConnectionSettingManager: BaseManager
    {
        #region Constructors

        public KCXConnectionSettingManager(string pConnectionString)
        {
            ConnectionString = pConnectionString;
        }

        #endregion

        #region Private Static Querys

        private static string _SP_GET_KCX_CONNECTION_SETTING = "get_kcx_connection_setting";

        #endregion
        
        #region Public Methods
        
        public KoreConXConnectionSetting GetKCXConnectionSettings(int transfAgentId)
        {
            
            //DatabaseConnection = new MySqlConnection(ConnectionString);
            MySqlCommand cmd = new MySqlCommand(_SP_GET_KCX_CONNECTION_SETTING, new MySqlConnection(ConnectionString));
            cmd.CommandTimeout = 60;

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@_transfer_agent_id", transfAgentId);
            cmd.Parameters["@_transfer_agent_id"].Direction = ParameterDirection.Input;
            
            MySqlDataReader reader;
            KoreConXConnectionSetting kcxSettings = null;


            // Open DB
            cmd.Connection.Open();

            try
            {
                // Run Query
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        // En nuestra base de datos, el array contiene:  ID 0, FIRST_NAME 1,LAST_NAME 2, ADDRESS 3
                        // Hacer algo con cada fila obtenida
                        kcxSettings = new KoreConXConnectionSetting()
                        {
                            TransferAgentId = reader.GetInt32("transfer_agent_id"),
                            ATSId = reader.GetString("ats_id"),
                            URL = reader.GetString("url"),
                            User = reader.GetString("user"),
                            Password = reader.GetString("password")
                        };
                    }
                }
            }
            finally
            {
                cmd.Connection.Close();
            }

            return kcxSettings;
        }
        
        #endregion
    }
}