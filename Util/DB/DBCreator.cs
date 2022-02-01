using Dapper;
using ShopUrban.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopUrban.Util
{
    class DBCreator
    {
        public static string dbPath = KStrings.BASE_FOLDER + "DB";
        public static string dbFilePath = dbPath + "\\shopUrbanDb.db";
        public static string dbConnectionString = string.Format("Data Source={0};", dbFilePath);

        private static DBCreator instance;

        public static DBCreator getInstance()
        {
            if (instance == null)
            {
                instance = new DBCreator();
            }

            return instance;
        }

        public void init()
        {
            string initializationComplete = null;

            if (!Directory.Exists(dbPath))
            {
                Directory.CreateDirectory(dbPath);
            }

            //dbFilePath = dbPath + "\\shopUrbanDb.db";

            if (!System.IO.File.Exists(dbFilePath))
            {
                SQLiteConnection.CreateFile(dbFilePath);
            }
            else
            {
                initializationComplete = Setting.getValue(Setting.KEY_INIT_COMPLETE);
                
                if(initializationComplete == null)
                {
                    File.Delete(dbFilePath);

                    SQLiteConnection.CreateFile(dbFilePath);
                }
            }

            if(initializationComplete == null)
            {
                createTables();
            }
        }

        public void createTables()
        {
            //string strCon = string.Format("Data Source={0};", dbConnectionString);

            string[] arr = { Staff.CREATE_TABLE, Cart.CREATE_TABLE, CartItem.CREATE_TABLE,
                Order.CREATE_TABLE, Product.CREATE_TABLE, ProductUnit.CREATE_TABLE,
                Setting.CREATE_TABLE, ShopProduct.CREATE_TABLE, CartDraft.CREATE_TABLE,
                CartItemDraft.CREATE_TABLE, OrderPayment.CREATE_TABLE
            };

            using (SQLiteConnection sqlite_conn = new SQLiteConnection(dbConnectionString))
            {
                sqlite_conn.Open();

                var SqliteCmd = new SQLiteCommand();
                SqliteCmd = sqlite_conn.CreateCommand();

                foreach (var sql in arr)
                {
                    SqliteCmd.CommandText = sql;

                    var res = SqliteCmd.ExecuteNonQuery();

                    if(KStrings.DEV) Trace.WriteLine($"Executed = {sql}, Rows = {res}");
                }

                sqlite_conn.Close();

                Setting.create(new Setting {
                    key = Setting.KEY_INIT_COMPLETE,
                    value = "true",
                    type = "bool"
                });
            }

        }


    }
}
