using MySql.Data.MySqlClient;
using Rialto.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.DataAccessLayer
{
    public class TransferAgentManager : BaseManager
    {
        #region Constructors

        public TransferAgentManager(string pConnectionString)
        {
            ConnectionString = pConnectionString;
        }

        #endregion

        #region Private Static Querys

        public static string _KCX_ID = "KoreConX";

        private static string _GET_TRANSFER_AGENT_QUERY = @"SELECT id,transfer_agent_name,transfer_agent_chain_id as transfer_agent_ats_id  
                                                            FROM secondarytrading.transfer_agent ta
                                                      
                                                          WHERE ta.transfer_agent_name='{0}' and ta.enabled=1";

       

       
        #endregion


        #region Public  Methods


        public TransferAgent GetTransferAgent(string transferAgent)
        {
            DatabaseConnection = new MySqlConnection(ConnectionString);
            MySqlCommand commandDatabase = new MySqlCommand(string.Format(_GET_TRANSFER_AGENT_QUERY, transferAgent), DatabaseConnection);
            commandDatabase.CommandTimeout = 60;
            MySqlDataReader reader;
            TransferAgent transfAgent = null;


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
                        transfAgent = new TransferAgent()
                        {
                            Id = reader.GetInt32("id"),
                            TransferAgentName = reader.GetString("transfer_agent_name"),
                            TransferAgentATSId = reader.GetString("transfer_agent_ats_id")
                        };
                    }
                }
            }
            finally
            {
                DatabaseConnection.Close();
            }

            return transfAgent;
        }

        #endregion

    }
}
