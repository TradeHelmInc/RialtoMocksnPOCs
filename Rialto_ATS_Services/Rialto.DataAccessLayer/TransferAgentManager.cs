using MySql.Data.MySqlClient;
using Rialto.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Data;
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
       
        private static string _SP_GET_TRANSFER_AGENT = "get_transfer_agent";
       
        #endregion


        #region Public  Methods


        public TransferAgent GetTransferAgent(string transferAgent)
        {
            MySqlCommand cmd = new MySqlCommand(_SP_GET_TRANSFER_AGENT, new MySqlConnection(ConnectionString));
            cmd.CommandTimeout = 60;

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@_transfer_agent_name", transferAgent);
            cmd.Parameters["@_transfer_agent_name"].Direction = ParameterDirection.Input;
            
            TransferAgent transfAgent = null;
            MySqlDataReader reader;


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
                cmd.Connection.Close();
            }

            return transfAgent;
        }

        #endregion

    }
}
