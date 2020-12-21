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
    public class UserManager : BaseManager
    {
        
        #region Constructors

        public UserManager(string pConnectionString)
        {
            ConnectionString = pConnectionString;
        }

        #endregion

        #region Private Static Querys

        private static string _SP_GET_USERS = "get_users";

        private static string _SP_PERSIST_USER = "persist_user";

        #endregion

        #region Private Methods

        private User BuildUser(MySqlDataReader reader)
        {
            User user = new User()
            {
                Id = reader.GetInt32("id"),
                Login = GetSafeString(reader,"login"),
                PasswordHash = GetSafeString(reader,"password_hash"),
                FirstName = GetSafeString(reader,"first_name"),
                LastName = GetSafeString(reader,"last_name"),
                Email = GetSafeString(reader,"email"),
                Activated = GetSafeBoolean(reader,"activated"),
                LangKey = GetSafeString(reader,"lang_key"),
                ActivationKey = GetSafeString(reader,"activation_key"),
                ResetKey = GetSafeString(reader,"reset_key"),
                CreatedBy = GetSafeString(reader,"created_by"),
                CreatedDate = reader.GetDateTime("created_date"),
                ResetDate = GetSafeDateTime(reader,"reset_date"),
                LastModifiedBy = GetSafeString(reader,"last_modified_by"),
                LastModifiedDate =GetSafeDateTime(reader,"last_modified_date"),
                PasswordModifyDate = GetSafeDateTime(reader,"password_modify_date"),
                LastActive = GetSafeDateTime(reader,"last_active"),

                BuyingPower = GetSafeDouble(reader, "buying_power"),
                UsedLimit = GetSafeDouble(reader, "used_limit"),
                Phone = GetSafeString(reader,"phone"),
                Enabled = GetSafeBoolean(reader,"enabled"),
                Disclaimer = GetSafeBoolean(reader,"disclaimer"),
                TradeLimit = GetSafeDouble(reader, "trade_limit"),
                RegulatoryFlag = GetSafeBoolean(reader,"regulatory_flag"),
                FirmId = reader.GetInt32("firm_id"),
                POCFirmId = GetNullSafeLong(reader, "poc_firm_id"),
                Capacity = GetSafeString(reader,"capacity"),
                CashOnHand = GetSafeDouble(reader, "cash_on_hand"),
                PledgedFund = GetSafeDouble(reader, "pledged_fund"),
            };

            return user;
        
        }

        #endregion


        #region Public Methods

        public long PersistUser(long shareholderId,User user)
        {

            //DatabaseConnection = new MySqlConnection(ConnectionString);
            MySqlCommand cmd = new MySqlCommand(_SP_PERSIST_USER, new MySqlConnection(ConnectionString));
            cmd.CommandTimeout = 60;

            cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Parameters.AddWithValue("@_id", user.Id);
            //cmd.Parameters["@_id"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@_login", user.Login);
            cmd.Parameters["@_login"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@password_hash", user.PasswordHash);
            cmd.Parameters["@password_hash"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@first_name", user.FirstName);
            cmd.Parameters["@first_name"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@last_name", user.LastName);
            cmd.Parameters["@last_name"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters["@email"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@image_url", null);
            cmd.Parameters["@image_url"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@activated", user.Activated);
            cmd.Parameters["@activated"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@lang_key", user.LangKey);
            cmd.Parameters["@lang_key"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@activation_key", user.ActivationKey);
            cmd.Parameters["@activation_key"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@reset_key", user.ResetKey);
            cmd.Parameters["@reset_key"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@created_by", user.CreatedBy);
            cmd.Parameters["@created_by"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@created_date", user.CreatedDate);
            cmd.Parameters["@created_date"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@reset_date", user.ResetDate);
            cmd.Parameters["@reset_date"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@last_modified_by", user.LastModifiedBy);
            cmd.Parameters["@last_modified_by"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@last_modified_date", user.LastModifiedDate);
            cmd.Parameters["@last_modified_date"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@password_modify_date", user.PasswordModifyDate);
            cmd.Parameters["@password_modify_date"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@last_active", user.LastActive);
            cmd.Parameters["@last_active"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@buying_power", user.BuyingPower);
            cmd.Parameters["@buying_power"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@used_limit", user.UsedLimit);
            cmd.Parameters["@used_limit"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@phone", user.Phone);
            cmd.Parameters["@phone"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@enabled", user.Enabled);
            cmd.Parameters["@enabled"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@disclaimer", user.Disclaimer);
            cmd.Parameters["@disclaimer"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@trade_limit", user.TradeLimit);
            cmd.Parameters["@trade_limit"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@regulatory_flag", user.RegulatoryFlag);
            cmd.Parameters["@regulatory_flag"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@firm_id", shareholderId);
            cmd.Parameters["@firm_id"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@poc_firm_id", user.POCFirmId);
            cmd.Parameters["@poc_firm_id"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@capacity", user.Capacity);
            cmd.Parameters["@capacity"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@cash_on_hand", user.CashOnHand);
            cmd.Parameters["@cash_on_hand"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@pledged_fund", user.PledgedFund);

            MySqlDataReader reader;

            cmd.Connection.Open();

            try
            {
                // Run Query
                object userId = cmd.ExecuteScalar();

                return Convert.ToInt64(userId);
            }
            finally
            {
                cmd.Connection.Close();
            }
        }


        public List<User> GetUsers(long firmId)
        {
            //DatabaseConnection = new MySqlConnection(ConnectionString);
            MySqlCommand cmd = new MySqlCommand(_SP_GET_USERS, new MySqlConnection(ConnectionString));
            cmd.CommandTimeout = 60;

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@firm_id", firmId);
            cmd.Parameters["@firm_id"].Direction = ParameterDirection.Input;

            cmd.Connection.Open();

            // Open DB
            MySqlDataReader reader;
            List<User> users = new List<User>();

            try
            {
                // Run Query
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        users.Add(BuildUser(reader));
                    }
                }
            }
            finally
            {
                cmd.Connection.Close();
            }

            return users;
        }


        #endregion
    }
}
