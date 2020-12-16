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

        private static string _GET_SHAREHOLDER_QUERY = @"SELECT id as id,firm_legal_name as name  
                                                      FROM secondarytrading.firm f
                                                      WHERE f.id={0} and f.enabled=1";

        private static string _GET_KORECONX_SHAREHOLDER_ID_QUERY = @"SELECT kcx_ids.share_holder_chain_id as kore_shareholder_id
                                                                  FROM secondarytrading.share_holder kcx_ids
                                                                  INNER JOIN secondarytrading.transfer_agent ta ON ta.id=kcx_ids.transfer_agent_id
                                                                  INNER JOIN secondarytrading.firm f  on f.id=kcx_ids.firm_id
                                                                  WHERE  f.id={1} and ta.transfer_agent_name='{0}' ";


      

        private static string _SP_GET_FIRMS = "get_firms";

        private static string _SP_PERSIST_FIRMS = "persist_firms";

       
        #endregion

        #region Protected Methods

        private Shareholder BuildShareholder(MySqlDataReader reader)
        {
        
            Shareholder sh = new Shareholder()
            {
                Id=reader.GetInt32("id"),
                Name=GetSafeString(reader,"firm_legal_name"),
                Address = GetSafeString(reader,"firm_address"),
                FirmLimit = GetSafeDouble(reader, "firm_limit"),
                Phone = GetSafeString(reader,"phone"),
                Email = GetSafeString(reader,"email"),
                Enabled = reader.GetBoolean("enabled"),
                FirmAccountNumber = GetSafeString(reader,"firm_account_number"),
                Principal = GetSafeString(reader,"principal"),
                PepStatus = GetSafeString(reader,"pep_status"),
                InsiderStatus = GetSafeString(reader,"insider_status"),
                FeeMatrix = GetSafeString(reader,"fee_matrix"),
                IssuerFirmCheckbox = GetSafeString(reader,"issuer_firm_checkbox"),
                LargeTraderId = GetSafeString(reader,"large_trader_id"),
                UniqueId = GetSafeString(reader,"unique_id"),
                Status = GetSafeString(reader,"status"),//PROCESAR
                LargeTraderFlag = GetSafeString(reader,"large_trader_flag"),
            };

            if(!string.IsNullOrEmpty(GetSafeString(reader,"kore_chain_id")))
            {
                KoreConXShareholderId kcxShareholderId = new KoreConXShareholderId()
                {
                    KoreShareholderId=GetSafeString(reader,"kore_chain_id")

                };

                sh.KoreConXShareholderId=kcxShareholderId;
            }

            return sh;
        }

        #endregion

        #region Public Methods

        public long PersistShareholder(Shareholder shareholder)
        {

            //DatabaseConnection = new MySqlConnection(ConnectionString);
            MySqlCommand cmd = new MySqlCommand(_SP_PERSIST_FIRMS, new MySqlConnection(ConnectionString));
            cmd.CommandTimeout = 60;

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", shareholder.Id);
            cmd.Parameters["@id"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@firm_legal_name", shareholder.Name);
            cmd.Parameters["@firm_legal_name"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@firm_address", shareholder.Address);
            cmd.Parameters["@firm_address"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@firm_limit", shareholder.FirmLimit);
            cmd.Parameters["@firm_limit"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@phone", shareholder.Phone);
            cmd.Parameters["@phone"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@email", shareholder.Email);
            cmd.Parameters["@email"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@enabled", shareholder.Enabled);
            cmd.Parameters["@enabled"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@firm_account_number", shareholder.FirmAccountNumber);
            cmd.Parameters["@firm_account_number"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@principal", shareholder.Principal);
            cmd.Parameters["@principal"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@firm_tax_id", shareholder.FirmTaxId);
            cmd.Parameters["@firm_tax_id"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@pep_status", shareholder.PepStatus);
            cmd.Parameters["@pep_status"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@insider_status", shareholder.InsiderStatus);
            cmd.Parameters["@insider_status"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@fee_matrix", shareholder.FeeMatrix);
            cmd.Parameters["@fee_matrix"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@issuer_firm_checkbox", shareholder.IssuerFirmCheckbox);
            cmd.Parameters["@issuer_firm_checkbox"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@large_trader_id", shareholder.LargeTraderId);
            cmd.Parameters["@large_trader_id"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@unique_id", shareholder.UniqueId);
            cmd.Parameters["@unique_id"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@firm_status", shareholder.Status);
            cmd.Parameters["@firm_status"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@large_trader_flag", shareholder.LargeTraderFlag);
            cmd.Parameters["@large_trader_flag"].Direction = ParameterDirection.Input;


            MySqlDataReader reader;
            List<Shareholder> shareholders = new List<Shareholder>();

            try
            {
                // Run Query
                object shareholderId = cmd.ExecuteScalar();

                return Convert.ToInt64(shareholderId);
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        public List<Shareholder> GetShareholders(string koreChainId,string firmTaxId)
        {
        
            //DatabaseConnection = new MySqlConnection(ConnectionString);
            MySqlCommand cmd = new MySqlCommand(_SP_GET_FIRMS,new MySqlConnection(ConnectionString));
            cmd.CommandTimeout = 60;

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@kore_chain_id", koreChainId);
            cmd.Parameters["@kore_chain_id"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@firm_tax_id", firmTaxId);
            cmd.Parameters["@firm_tax_id"].Direction = ParameterDirection.Input;

            cmd.Connection.Open();

            int count = 0;
            // Open DB

            MySqlDataReader reader;
            List<Shareholder> shareholders = new List<Shareholder>();

            try
            {
                // Run Query
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        shareholders.Add(BuildShareholder(reader));
                    }
                }
            }
            finally
            {
                cmd.Connection.Close();
            }

            return shareholders;
        }

        public KoreConXShareholderId GetKoreShareholderId(long shareholderId)
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
                        koreShrId = new KoreConXShareholderId() { KoreShareholderId = GetSafeString(reader,"kore_shareholder_id") };
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
                        shr = new Shareholder() { Id = reader.GetInt32("id"), Name = GetSafeString(reader,"name") };
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
