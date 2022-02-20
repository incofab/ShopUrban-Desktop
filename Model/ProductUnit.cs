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
    public class ProductUnit : BaseModel
    {
        public const string CREATE_TABLE = "CREATE TABLE IF NOT EXISTS `product_units` (" +
            "id INTEGER NOT NULL," +
            "name VARCHAR(255)," +
            "slug VARCHAR(255)," +
            "product_id INTEGER," +
            "num_of_units INTEGER DEFAULT 0," +
            "photo VARCHAR(255)," +
            "local_photo VARCHAR(255)," +
            "barcode VARCHAR(255)," +
            TIME_STAMPS_STR +
            ")";

        public const string table = "product_units";
        public static string[] fillable = {"id", "product_id", "name",  "slug", "num_of_units", "barcode", "photo", "local_photo", "created_at", "updated_at" };
        public int id { get; set; }
        public int product_id { get; set; }
        public int? num_of_units { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public string barcode { get; set; }
        public string photo { get; set; }
        public string local_photo { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }

        public static void multiCreate(List<ProductUnit> productUnits)
        {
            if (productUnits == null) return;

            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                foreach (var productUnit in productUnits)
                {
                    var insertSql = prepareInsertQuery(table, productUnit, fillable);

                    cnn.Execute(insertSql, productUnit);
                }
            }
        }
        public static void multiCreateOrUpdate(List<ProductUnit> productUnits)
        {
            if (productUnits == null) return;

            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                foreach (var productUnit in productUnits)
                {
                    object queryObject = new { id = productUnit.id };

                    var query = cnn.Query<ProductUnit>($"SELECT * FROM {table} {prepareEqQuery(queryObject)}",
                        queryObject).ToList();

                    if (query.Count() < 1) //Create new
                    {
                        var insertSql = prepareInsertQuery(table, productUnit, fillable);

                        cnn.Execute(insertSql, productUnit);
                    }
                    else //Update
                    {
                        //var first = query.First<ProductUnit>();
                        var insertSql = prepareUpdateQuery(table, productUnit, new { id = productUnit.id }, fillable);
                        cnn.Execute(insertSql, productUnit);
                    }
                }
            }
        }

        public static List<ProductUnit> all(string condition = null)
        {
            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                var sql = $"SELECT * FROM product_units "+condition;

                var query = cnn.Query<ProductUnit>(sql);

                return query.ToList();
            }
        }

        public static List<ProductUnit> allUnsyncProductImages()
        {
            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                var sql = $"SELECT * FROM product_units WHERE local_photo IS NULL";

                var query = cnn.Query<ProductUnit>(sql);

                return query.ToList();
            }
        }

        public static void update(ProductUnit productUnit)
        {
            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                var insertSql = prepareUpdateQuery("product_units", 
                    productUnit, new { id = productUnit.id });

                cnn.Execute(insertSql, productUnit);
            }
        }

        public static bool checkIfExist(IDbConnection cnn, int id)
        {
            var query = cnn.Query<ProductUnit>("SELECT id = @id",
                    new { id = id });

            return query.First() != null;
        }

    }
}
