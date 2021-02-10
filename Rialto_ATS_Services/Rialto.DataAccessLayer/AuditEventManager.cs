using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Rialto.BusinessEntities;

namespace Rialto.DataAccessLayer
{
    public class AuditEventManager:BaseManager
    {
        #region Constructors

        public AuditEventManager(string pConnectionString)
        {
            ConnectionString = pConnectionString;
        }

        #endregion

        #region Private Static Querys

        private static string _SP_INSERT_AUDIT_EVENT = "insert_audit_event";

        #endregion

 

        #region Public Methods

        public long InsertAudit(string principal, DateTime eventDate,string eventType,String exectionType,String message,
                                string idName=null,string idValue=null)
        {

            //DatabaseConnection = new MySqlConnection(ConnectionString);
            MySqlCommand cmd = new MySqlCommand(_SP_INSERT_AUDIT_EVENT, new MySqlConnection(ConnectionString));
            cmd.CommandTimeout = 60;

            cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Parameters.AddWithValue("@_id", user.Id);
            //cmd.Parameters["@_id"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@principal",principal);
            cmd.Parameters["@principal"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@event_date", eventDate);
            cmd.Parameters["@event_date"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("@event_type",eventType);
            cmd.Parameters["@event_type"].Direction = ParameterDirection.Input;
            
            cmd.Parameters.AddWithValue("@exception_type", exectionType);
            cmd.Parameters["@exception_type"].Direction = ParameterDirection.Input;
            
            cmd.Parameters.AddWithValue("@event_value",message);
            cmd.Parameters["@event_value"].Direction = ParameterDirection.Input;
            
            cmd.Parameters.AddWithValue("@id_name",idName);
            cmd.Parameters["@id_name"].Direction = ParameterDirection.Input;
            
            cmd.Parameters.AddWithValue("@id_value",idValue);
            cmd.Parameters["@id_value"].Direction = ParameterDirection.Input;

            MySqlDataReader reader;

            cmd.Connection.Open();

            try
            {
                // Run Query
                object evId = cmd.ExecuteScalar();

                return Convert.ToInt64(evId);
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        
        #endregion
    }
}