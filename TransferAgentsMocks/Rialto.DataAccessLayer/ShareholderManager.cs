﻿using MySql.Data.MySqlClient;
using Rialto.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.DataAccessLayer
{
    public class ShareholderManager : BaseManager
    {
        #region Constructors

        public ShareholderManager(string pConnectionString)
        {
            ConnectionString = pConnectionString;
        }

        #endregion

        #region Private Static Querys

        private static string _KCX_ID = "KoreConX";

        private static string _GET_SHAREHOLDER_QUERY = @"SELECT id as id,firm_legal_name as name  FROM secondarytrading.firm f
                                                      
                                                      WHERE f.id={0} and f.enabled=1";

        private static string _GET_KORECONX_SHAREHOLDER_ID_QUERY = @"SELECT kcx_ids.share_holder_chain_id as kore_shareholder_id
                                                                  FROM secondarytrading.share_holder kcx_ids
                                                                  INNER JOIN secondarytrading.transfer_agent ta ON ta.id=kcx_ids.transfer_agent_id
                                                                  INNER JOIN secondarytrading.firm f  on f.id=kcx_ids.firm_id
                                                                  WHERE  f.id={1} and ta.transfer_agent_name='{0}' ";

       
        #endregion

        #region Public Methods

        public KoreConXShareholderId GetKoreShareholderId(int shareholderId)
        {
            DatabaseConnection = new MySqlConnection(ConnectionString);
            MySqlCommand commandDatabase = new MySqlCommand(string.Format(_GET_KORECONX_SHAREHOLDER_ID_QUERY, _KCX_ID, shareholderId), DatabaseConnection);
            commandDatabase.CommandTimeout = 60;
            MySqlDataReader reader;
            KoreConXShareholderId koreShrId = null;

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
                        koreShrId = new KoreConXShareholderId() { KoreShareholderId = reader.GetString("kore_shareholder_id") };
                        count++;
                    }
                }
            }
            finally
            {
                DatabaseConnection.Close();
            }

            if (count > 1)
                throw new Exception(string.Format("Inconsistent DB state detected: There are more than 1 Kore Shareholder Ids for security {0}", koreShrId));

            return koreShrId;
        
        }

        public Shareholder GetShareholder(int Id)
        {
            DatabaseConnection = new MySqlConnection(ConnectionString);
            MySqlCommand commandDatabase = new MySqlCommand(string.Format(_GET_SHAREHOLDER_QUERY, Id), DatabaseConnection);
            commandDatabase.CommandTimeout = 60;
            MySqlDataReader reader;
            Shareholder shr = null;


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
                        shr = new Shareholder() { Id = reader.GetInt32("id"), Name = reader.GetString("name") };
                    }
                }
            }
            finally
            {
                DatabaseConnection.Close();
            }

            return shr;
        }

        #endregion
    }
}
