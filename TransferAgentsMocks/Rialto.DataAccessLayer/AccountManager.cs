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
    public class AccountManager : BaseManager
    {
        #region Constructors

        public AccountManager(string pConnectionString)
        {
            ConnectionString = pConnectionString;
        }

        #endregion

        #region Private Static Querys

        private static string _SP_PERSIST_ACCOUNT = "persist_account";

        private static string _SP_GET_ACCOUNTS = "get_accounts";

        #endregion


        #region Private Methods

        private Account BuildAccount(MySqlDataReader reader)
        {
            Account account = new Account()
            {
                Id = reader.GetInt64("id"),
                AccountNumber = GetSafeString(reader, "account_number"),
                AccountName = GetSafeString(reader, "account_name"),
                AccountStatus = GetSafeBoolean(reader, "account_status"),
                SettlementInstructions = GetSafeString(reader, "settlement_instructions"),
                ClientContact = GetSafeString(reader, "client_contact"),
                Email = GetSafeString(reader, "email"),
                PhoneNumber = GetSafeString(reader, "phone_number"),
                AccountType = GetSafeString(reader, "account_type"),
                ActivationDate = GetSafeDateTime(reader, "account_activation_date"),
                AddressStreet = GetSafeString(reader, "address_street"),
                AddressCity = GetSafeString(reader, "address_city"),
                AddressState = GetSafeString(reader, "address_state"),
                AddressCountry = GetSafeString(reader, "address_country"),
                AddressZipCode = GetSafeString(reader, "address_zip_code"),
                TaxDomicile = GetSafeString(reader, "tax_domicile"),
                TaxFormType = GetSafeString(reader, "tax_form_type"),
                TaxFormStatus = GetSafeString(reader, "tax_form_status"),
                SSNTinEin = GetSafeString(reader, "ssn_tin_ein"),
                PepStatus = GetSafeString(reader, "pep_status"),
                InsiderStatus = GetSafeString(reader, "insider_status"),
                FinraRegistration = GetSafeString(reader, "finra_registration"),
                FeeMatrix = GetSafeString(reader, "fee_matrix"),
                TopAccount = GetSafeString(reader, "top_account"),
                Shareholder = new Shareholder() { Id = reader.GetInt64("firm_id") },
                UserCount = GetNullSafeInt(reader, "user_count"),
                SecFees = GetNullSafeDouble(reader, "sec_fees"),
                Commission = GetNullSafeDouble(reader, "commission"),
            };

            return account;

        }

        #endregion

        #region Public Methods

        public long PersistAccount( Account account)
        {

            //DatabaseConnection = new MySqlConnection(ConnectionString);
            MySqlCommand cmd = new MySqlCommand(_SP_PERSIST_ACCOUNT, new MySqlConnection(ConnectionString));
            cmd.CommandTimeout = 60;

            cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Parameters.AddWithValue("@_id", user.Id);
            //cmd.Parameters["@_id"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@_id", account.Id);
            cmd.Parameters["@_id"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@account_number", account.AccountNumber);
            cmd.Parameters["@account_number"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@account_name", account.AccountName);
            cmd.Parameters["@account_name"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@account_status", account.AccountStatus);
            cmd.Parameters["@account_status"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@settlement_instructions", account.SettlementInstructions);
            cmd.Parameters["@settlement_instructions"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@client_contact", account.ClientContact);
            cmd.Parameters["@client_contact"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@email", account.Email);
            cmd.Parameters["@email"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@phone_numnber", account.PhoneNumber);
            cmd.Parameters["@phone_numnber"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@account_type", account.AccountType);
            cmd.Parameters["@account_type"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@activation_date", account.ActivationDate);
            cmd.Parameters["@activation_date"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@address_street", account.AddressStreet);
            cmd.Parameters["@address_street"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@address_city", account.AddressCity);
            cmd.Parameters["@address_city"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@address_state", account.AddressState);
            cmd.Parameters["@address_state"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@address_country", account.AddressCountry);
            cmd.Parameters["@address_country"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@address_zip_zode", account.AddressZipCode);
            cmd.Parameters["@address_zip_zode"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@tax_domicile", account.TaxDomicile);
            cmd.Parameters["@tax_domicile"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@tax_form_type", account.TaxFormType);
            cmd.Parameters["@tax_form_type"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@tax_form_status", account.TaxFormStatus);
            cmd.Parameters["@tax_form_status"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@ssn_tin_ein", account.SSNTinEin);
            cmd.Parameters["@ssn_tin_ein"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@pep_status", account.PepStatus);
            cmd.Parameters["@pep_status"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@insider_status", account.InsiderStatus);
            cmd.Parameters["@insider_status"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@finra_registration", account.FinraRegistration);
            cmd.Parameters["@finra_registration"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@fee_matrix", account.FeeMatrix);
            cmd.Parameters["@fee_matrix"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@top_account", account.TopAccount);
            cmd.Parameters["@top_account"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@firm_id", account.Shareholder.Id);
            cmd.Parameters["@firm_id"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@user_count", account.UserCount);
            cmd.Parameters["@user_count"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@sec_fees", account.SecFees);
            cmd.Parameters["@sec_fees"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@commission", account.Commission);
            cmd.Parameters["@commission"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@default_user", account.DefaultUser != null ? (long?) account.DefaultUser.Id : null);
            cmd.Parameters["@default_user"].Direction = ParameterDirection.Input;


            cmd.Connection.Open();

            try
            {
                // Run Query
                object accounId = cmd.ExecuteScalar();

                return Convert.ToInt64(accounId);
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        public List<Account> GetAccounts(long firmId)
        {
            //DatabaseConnection = new MySqlConnection(ConnectionString);
            MySqlCommand cmd = new MySqlCommand(_SP_GET_ACCOUNTS, new MySqlConnection(ConnectionString));
            cmd.CommandTimeout = 60;

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@_firm_id", firmId);
            cmd.Parameters["@_firm_id"].Direction = ParameterDirection.Input;

            cmd.Connection.Open();

            // Open DB
            MySqlDataReader reader;
            List<Account> accounts = new List<Account>();

            try
            {
                // Run Query
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        accounts.Add(BuildAccount(reader));
                    }
                }
            }
            finally
            {
                cmd.Connection.Close();
            }

            return accounts;
        }

        #endregion
    }
}
