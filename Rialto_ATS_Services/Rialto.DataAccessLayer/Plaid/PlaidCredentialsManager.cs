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

        #endregion
        
        #region Public Methods
        
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