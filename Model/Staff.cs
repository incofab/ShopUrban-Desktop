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
    public class Staff : BaseModel
    {
        public const string CREATE_TABLE = "CREATE TABLE IF NOT EXISTS `staff` (" +
            "id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
            "name VARCHAR(255)," +
            "user_id INTEGER UNIQUE," +
            "username VARCHAR(100) UNIQUE," +
            "phone VARCHAR(20)," +
            "pin VARCHAR(255)," +
            "password VARCHAR(255)," +
            "token VARCHAR(255)," +
            TIME_STAMPS +
            ")";

        public static string[] fillable = { "user_id", "name", "username", "phone", "pin", "password", "token"};

        const string table = "staff";
        public int id { get; set; }
        public int user_id { get; set; }
        public string name { get; set; }
        public string username { get; set; }
        public string phone { get; set; }
        public string pin { get; set; }
        public string password { get; set; }
        public string token { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }

        public static List<Staff> all()
        {
            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                var query = cnn.Query<Staff>($"SELECT * FROM staff");
                
                return query.ToList<Staff>();
            }
        }

        public static Staff login(string phone, string pin)
        {
            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                object queryObject = new { phone = phone, pin = pin, };

                var whereQuery = prepareEqQuery(queryObject);

                var query = cnn.Query<Staff>($"SELECT * FROM staff {whereQuery}", queryObject).ToList();

                return query.Count > 0 ? query.First() : null;
            }
        }
        public static Staff findById(int id)
        {
            return eqQuery(new { id = id });
        }

        public static Staff eqQuery(object queryObject)
        {
            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                var whereQuery = prepareEqQuery(queryObject);

                var query = cnn.Query<Staff>($"SELECT * FROM staff {whereQuery}", queryObject).ToList();

                return query.Count > 0 ? query.First() : null;
            }
        }

        public static void save(Staff staff)
        {
            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                var insertSql = prepareInsertQuery(table, staff, fillable);

                cnn.Execute(insertSql, staff);
            }
        }

        public static void updateByUserId(Staff staff)
        {
            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                var updateSql = prepareUpdateQuery(table, staff, new { user_id = staff.user_id});

                cnn.Execute(updateSql, staff);
            }
        }

    }
}
