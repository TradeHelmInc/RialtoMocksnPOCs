using MySql.Data.MySqlClient;
using Rialto.BusinessEntities;
using Rialto.BusinessEntities.KoreConX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.DataAccessLayer
{
    public class OrderManager : BaseManager
    {
        #region Constructors

        public OrderManager(string pOrderConnectionString,string pTradingConnectionString)
        {
            OrderConnectionString = pOrderConnectionString;

            TradingConnectionString = pTradingConnectionString;
        }

        #endregion

        #region Protected Attributes

        protected string OrderConnectionString { get; set; }

        protected string TradingConnectionString { get; set; }

        #endregion
      
        #region Private Static Querys

        private static string _GET_ORDER_QUERY = @"SELECT id,symbol,qty,side,limit_price,state,leaves_qty,executed_qty,trade_price,datetime_recieved as datetime_received,
                                                             firm_id as shareholder_id,execution_datetime,matching_id 
                                                   FROM secondaryorder.jhi_order WHERE id={0}";

        private static string _GET_ORDER_TO_CEAR_QUERY = @"SELECT id,symbol,qty,side,limit_price,state,leaves_qty,executed_qty,trade_price,datetime_recieved as datetime_received,
                                                             firm_id as shareholder_id,execution_datetime,matching_id 
                                                            FROM secondaryorder.jhi_order WHERE state IN ('{0}','{1}') AND matching_id is not NULL";

        private static string _GET_KCX_TRANSACTION_QUERY = @"SELECT id,state,transaction_id,order_id,number_of_shares,
                                                                    koresecurities_id as kore_security_id,securities_holder_id as kore_shareholder_id,
                                                                    ats_id
                                                            FROM secondaryorder.jhi_holding
                                                            WHERE order_id={0}";


        private static string _KCX_PERSIST_TRANSACTION_QUERY = @"INSERT INTO secondaryorder.jhi_holding
                                                                    (state,
                                                                    transaction_id,
                                                                    order_id,
                                                                    number_of_shares,
                                                                    koresecurities_id,
                                                                    securities_holder_id,
                                                                    ats_id,
                                                                    host)
                                                                    VALUES
                                                                    ('{0}',
                                                                    '{1}',
                                                                     {2},
                                                                     {3},
                                                                    '{4}',
                                                                    '{5}',
                                                                    '{6}',
                                                                    '{7}');";

   

        #endregion

        #region Private Methods

        private Order BuildOrder(MySqlDataReader reader)
        {
            Order order = new Order()
            {
                Id = reader.GetInt32("id"),
                Symbol = reader.GetString("symbol"),
                Qty = reader.GetInt32("qty"),
                Side = Order.GetSideFromStr(reader.GetString("side")),
                Price = !reader.IsDBNull(reader.GetOrdinal("limit_price")) ? (double?)reader.GetDouble("limit_price") : null,
                State = Order.GetStateFromStr(reader.GetString("state")),
                LeavesQty = !reader.IsDBNull(reader.GetOrdinal("leaves_qty")) ? (double?)reader.GetDouble("leaves_qty") : null,
                ExecutedQty = !reader.IsDBNull(reader.GetOrdinal("executed_qty")) ? (double?)reader.GetDouble("executed_qty") : null,
                TradePrice = !reader.IsDBNull(reader.GetOrdinal("trade_price")) ? (double?)reader.GetDouble("trade_price") : null,
                DateTimeReceived = reader.GetDateTime("datetime_received"),
                ShareholderId = reader.GetInt32("shareholder_id"),
                ExecutionTime = !reader.IsDBNull(reader.GetOrdinal("execution_datetime")) ? (DateTime?)reader.GetDateTime("execution_datetime") : null,
                MatchingId = !reader.IsDBNull(reader.GetOrdinal("matching_id")) ? reader.GetString("matching_id") : null,
            };

            return order;
        
        }

        private KoreConXTransaction BuildKoreConXTransaction(MySqlDataReader reader)
        {

            KoreConXTransaction tx = new KoreConXTransaction()
            {
                Id = reader.GetInt32("id"),
                State = KoreConXTransaction.GetKoreConXTransactionStateFromStr(reader.GetString("state")),
                TransactionId = reader.GetString("transaction_id"),
                NumberOfShares = reader.GetInt32("number_of_shares"),
                KoreSecurityId = reader.GetString("kore_security_id"),
                KoreShareholderId = reader.GetString("kore_shareholder_id"),
                ATSId = reader.GetString("ats_id"),
                OrderId = reader.GetInt32("order_id")

            };

            return tx;
        }

        #endregion


        #region Public Methods

        public void PersistTransaction(KoreConXTransaction tx)
        {


            DatabaseConnection = new MySqlConnection(OrderConnectionString);
            string query=string.Format(_KCX_PERSIST_TRANSACTION_QUERY,

                                                                           tx.GetKoreConXTransactionStateToStr(),
                                                                           tx.TransactionId,
                                                                           tx.OrderId,
                                                                           tx.NumberOfShares,
                                                                           tx.KoreSecurityId,
                                                                           tx.KoreShareholderId,
                                                                           tx.ATSId,
                                                                           "");

            MySqlCommand commandDatabase = new MySqlCommand(query, DatabaseConnection);
            commandDatabase.CommandTimeout = 60;
            MySqlDataReader reader;
            List<KoreConXTransaction> transactions = new List<KoreConXTransaction>();


            // Open DB
            DatabaseConnection.Open();

            try
            {
                // Run Query
                commandDatabase.ExecuteNonQuery();

               
            }
            finally
            {
                DatabaseConnection.Close();
            }
        }

        public List<KoreConXTransaction> GetKoreConXTransaction(int orderId)
        {


            DatabaseConnection = new MySqlConnection(OrderConnectionString);
            MySqlCommand commandDatabase = new MySqlCommand(string.Format(_GET_KCX_TRANSACTION_QUERY, orderId), DatabaseConnection);
            commandDatabase.CommandTimeout = 60;
            MySqlDataReader reader;
            List<KoreConXTransaction> transactions = new List<KoreConXTransaction>();


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
                        transactions.Add(BuildKoreConXTransaction(reader));
                    }
                }
            }
            finally
            {
                DatabaseConnection.Close();
            }

            return transactions;
        
        }

        public List<Order> GetOrdersToCear()
        {
            DatabaseConnection = new MySqlConnection(OrderConnectionString);
            MySqlCommand commandDatabase = new MySqlCommand(string.Format(_GET_ORDER_TO_CEAR_QUERY, Order._STATE_EXECUTED, Order._STATE_PARTIAL), DatabaseConnection);
            commandDatabase.CommandTimeout = 60;
            MySqlDataReader reader;
            List<Order> orders = new List<Order>();


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
                        orders.Add(BuildOrder(reader));
                    }
                }
            }
            finally
            {
                DatabaseConnection.Close();
            }

            return orders;
        
        
        }

        public Order GetOrder(int Id)
        {
            DatabaseConnection = new MySqlConnection(OrderConnectionString);
            MySqlCommand commandDatabase = new MySqlCommand(string.Format(_GET_ORDER_QUERY, Id), DatabaseConnection);
            commandDatabase.CommandTimeout = 60;
            MySqlDataReader reader;
            Order order = null;


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
                        order = BuildOrder(reader);
                    }
                }
            }
            finally
            {
                DatabaseConnection.Close();
            }

            return order;
        }


        #endregion
    }
}
