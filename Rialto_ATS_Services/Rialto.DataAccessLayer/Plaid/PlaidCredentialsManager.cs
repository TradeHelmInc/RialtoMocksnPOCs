using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Rialto.BusinessEntities;
using Rialto.BusinessEntities.Plaid;

namespace Rialto.DataAccessLayer.Plaid
{
    public class PlaidCredentialsManager: BaseManager
    {
        #region Constructors

        public PlaidCredentialsManager(string pConnectionString)
        {
            ConnectionString = pConnectionString;
        }

        #endregion

        #region Private Static Querys

        private static string _SP_PERSIST_PLAID_CREDENTIALS = "persist_plaid_credentials";
        
        private static string _SP_GET_PLAID_CREDENTIALS = "get_plaid_credentials";

        #endregion
        
          #region Private Methods

        private PlaidCredential BuildPlaidCredential(MySqlDataReader reader)
        {
            PlaidCredential cred = new PlaidCredential()
            {
                User = new User()
                {
                    Id = reader.GetInt32("jhi_user_id")
                },
                AccessToken = GetSafeString(reader, "plaid_access_token"),
                UserIdentifier = GetSafeString(reader, "user_identifier"),
                PlaidItemId = GetSafeString(reader, "plaid_item_id"),
                Secret = GetSafeString(reader, "plaid_secret"),
                ClientId = GetSafeString(reader, "plaid_client")
            };

            return cred;
        
        }

        #endregion
        
        #region Public Methods
        
        
        public PlaidCredential GetPlaidCredentials(string email)
        {
            //DatabaseConnection = new MySqlConnection(ConnectionString);
            MySqlCommand cmd = new MySqlCommand(_SP_GET_PLAID_CREDENTIALS, new MySqlConnection(ConnectionString));
            cmd.CommandTimeout = 60;

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@_email", email);
            cmd.Parameters["@_email"].Direction = ParameterDirection.Input;
            
            cmd.Connection.Open();

            // Open DB
            MySqlDataReader reader;
            PlaidCredential cred = null;

            try
            {
                // Run Query
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        cred=BuildPlaidCredential(reader);
                    }
                }
            }
            finally
            {
                cmd.Connection.Close();
            }

            return cred;
        }
        
        public void PersistPlaidCredentials(PlaidCredential cred)
        {

            //DatabaseConnection = new MySqlConnection(ConnectionString);
            MySqlCommand cmd = new MySqlCommand(_SP_PERSIST_PLAID_CREDENTIALS, new MySqlConnection(ConnectionString));
            cmd.CommandTimeout = 60;

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@_jhi_user_id", cred.User.Id);
            cmd.Parameters["@_jhi_user_id"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@_user_identifier", cred.UserIdentifier);
            cmd.Parameters["@_user_identifier"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@_plaid_access_token", cred.AccessToken);
            cmd.Parameters["@_plaid_access_token"].Direction = ParameterDirection.Input;
            
            cmd.Parameters.AddWithValue("@_plaid_item_id", cred.PlaidItemId);
            cmd.Parameters["@_plaid_item_id"].Direction = ParameterDirection.Input;

            cmd.Connection.Open();
            try
            {
                // Run Query
                cmd.ExecuteScalar();
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        
        #endregion
    }
}