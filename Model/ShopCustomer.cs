using Dapper;
using Newtonsoft.Json;
using ShopUrban.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace ShopUrban.Model
{
    public class ShopCustomer:BaseModel
    {
        public const string CREATE_TABLE = "CREATE TABLE IF NOT EXISTS `shop_customers` (" +
               "id INTEGER NOT NULL," +
               "user_id INTEGER," +
               "phone VARCHAR," +
               "name VARCHAR" +
               ")";

        public const string table = "shop_customers";

        public static string[] fillable = {
            "id", "user_id", "phone", "name"
        };
        public int id { get; set; }
        public int? user_id { get; set; }
        public string phone { get; set; }
        public string name { get; set; }

        public static void multiCreateOrUpdate(List<ShopCustomer> shopCustomers)
        {
            if (shopCustomers == null || shopCustomers.Count < 1) return;

            var cnn = DBCreator.getConn();

            foreach (var shopCustomer in shopCustomers)
            {
                object queryObject = new { user_id = shopCustomer.user_id };

                var query = cnn.Query<ShopCustomer>($"SELECT * FROM {table} {prepareEqQuery(queryObject)}",
                    queryObject).ToList();

                if (query.Count() < 1) //Create new
                {
                    var insertSql = prepareInsertQuery(table, shopCustomer, fillable);

                    cnn.Execute(insertSql, shopCustomer);
                }
                else //Update
                {
                    var first = query.First<ShopCustomer>();
                    var updateSql = prepareUpdateQuery(table, shopCustomer, new { id = shopCustomer.id }, fillable);
                    cnn.Execute(updateSql, shopCustomer);
                }
            }
        }

        public static List<ShopCustomer> all(string searchQuery = null)
        {
            IDbConnection cnn = DBCreator.getConn();

            var sql = $"SELECT * FROM shop_customers";

            object queryObj = null;
            
            if (searchQuery != null)
            {
                queryObj = new { phone = $"%{searchQuery}%", name = $"%{searchQuery}%" };

                sql += " WHERE phone LIKE @phone OR name LIKE @name";
            }

            var query = cnn.Query<ShopCustomer>(sql, queryObj).ToList();

            return query;
        }

    }
}
