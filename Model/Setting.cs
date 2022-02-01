using Dapper;
using Newtonsoft.Json;
using ShopUrban.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ShopUrban.Model
{
    public class Setting :BaseModel
    {
        public const string CREATE_TABLE = "CREATE TABLE IF NOT EXISTS `settings` (" +
            "id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
            "key VARCHAR(100) UNIQUE," +
            "value TEXT," +
            "display_name VARCHAR(255)," +
            "type VARCHAR(30)," +
            TIME_STAMPS +
            ")";

        public static string[] fillable = { "key", "value", "display_name", "type" };

        public const string KEY_REMEMBER_LOGIN = "remember_login";
        public const string KEY_LOGGED_IN_STAFF_ID = "logged_in_staff_id";
        public const string KEY_SHOP_DETAILS = "shop_details";
        public const string KEY_INIT_COMPLETE = "initialization_complete";
        public const string KEY_PRODUCT_LAST_SYNC = "product_last_sync";
        public const string KEY_PRODUCT_LAST_SYNC_SERVER = "product_last_sync_server";
        public const string KEY_IMAGES_SYNCED = "is_images_sync";
        public const string KEY_ORDER_LAST_SYNC_ID = "order_last_sync_id";
        public const string KEY_ORDER_LAST_SYNC_TIME = "order_last_sync_time";
        public const string KEY_RECEIPT_LAYOUT = "receipt_layout";
        public const string KEY_VAT_PERCENT = "vat";

        public const string RECEIPT_LAYOUT_88MM = "Receipt Layout 88mm";
        public const string RECEIPT_LAYOUT_58MM = "Receipt Layout 58mm";
        public const string RECEIPT_LAYOUT_A4 = "Receipt Layout A4";

        const string table = "settings";
        public int id { get; set; }
        public string key { get; set; }
        public string value { get; set; }
        public string type { get; set; }
        public string display_name { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }

        public static void create(object setting)
        {
            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                var insertSql = prepareInsertQuery(table, setting, fillable);

                cnn.Execute(insertSql, setting);
            }
        }
        public static void createOrUpdate(Setting setting)
        {
            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                object queryObject = new { key = setting.key };

                var query = cnn.Query<Setting>($"SELECT * FROM {table} {prepareEqQuery(queryObject)}", 
                    queryObject).ToList();

                if(query.Count() < 1) //Create new
                {
                    var insertSql = prepareInsertQuery(table, setting, fillable);

                    cnn.Execute(insertSql, setting);
                }
                else //Update
                {
                    var insertSql = prepareUpdateQuery(table, setting, new { key = setting.key}, fillable);
                    cnn.Execute(insertSql, setting);
                    //Reset the shop object
                    if (setting.key == KEY_SHOP_DETAILS) shop = null;
                }
            }
        }

        public static void update(object setting, string key)
        {
            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                var updateSql = prepareUpdateQuery(table, setting, new { key = key }, fillable);

                cnn.Execute(updateSql, setting);
            }
        }

        public static Setting get(string key)
        {
            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                object queryObject = new { key = key };

                var whereQuery = prepareEqQuery(queryObject);

                var query = cnn.Query<Setting>($"SELECT * FROM {table} {whereQuery}", queryObject).ToList();

                return (query == null || query.Count < 1) ? null : query.First();
            }
        }

        public static string getValue(string key)
        {
            var setting = get(key);

            return setting == null ? null : setting.value;
        }

        private static Shop shop;
        public static Shop getShopDetail()
        {
            if (shop != null) return shop;

            var shopDetail = getValue(KEY_SHOP_DETAILS);

            if (shopDetail == null) return null;

            shop = JsonConvert.DeserializeObject<Shop>(shopDetail);

            return shop;
        }
        public static string getShopId()
        {
            var shop = getShopDetail();
            return shop == null ? null : shop.id + "";
        }


    }
}
