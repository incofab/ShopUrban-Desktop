using Dapper;
using ShopUrban.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace ShopUrban.Model
{
    public class OrderPayment : BaseModel
    {
        public const string CREATE_TABLE = "CREATE TABLE IF NOT EXISTS `order_payments` (" +
            "id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
            "user_id INTEGER," +
            "order_number_track VARCHAR(255)," +
            "amount decimal(8,2) DEFAULT 0," +
            "remaining_amount decimal(8,2) DEFAULT 0," +
            "payment_type VARCHAR(255)," +
            "is_confirmed INTEGER," +
            TIME_STAMPS +
            ")";

        public static string[] fillable = {
            "user_id", "order_number_track", "amount", "payment_type", "is_confirmed", "remaining_amount"
        };

        const string table = "order_payments";
        public int id { get; set; }
        public int user_id { get; set; }
        public string order_number_track { get; set; }
        public decimal amount { get; set; }
        public decimal remaining_amount { get; set; }
        public string payment_type { get; set; }
        public int? is_confirmed { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public Order order { get; set; }

        public static List<OrderPayment> all(string orderNumber, string orderBy = "ASC")
        {
            var cnn = DBCreator.getConn();

            object queryObject = new { order_number_track = orderNumber };

            var sql = $" SELECT * FROM {table} " +
            $" WHERE order_number_track = @order_number_track ORDER BY id {orderBy}";

            var query = cnn.Query<OrderPayment>(sql, queryObject);

            return query.ToList<OrderPayment>();
        }

        public static void create(OrderPayment orderPayment)
        {
            var cnn = DBCreator.getConn();

            var insertSql = prepareInsertQuery(table, orderPayment, fillable);

            cnn.Execute(insertSql, orderPayment);
        }

    }
}
