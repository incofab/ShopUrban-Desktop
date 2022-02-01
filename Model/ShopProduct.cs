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
    public class ShopProduct:BaseModel
    {
        public const string CREATE_TABLE = "CREATE TABLE IF NOT EXISTS `shop_products` (" +
               "id INTEGER NOT NULL," +
               "product_id INTEGER," +
               "product_unit_id INTEGER," +
               "cost_price decimal(8,2) DEFAULT 0," +
               "sell_price decimal(8,2) DEFAULT 0," +
               "stock_count INTEGER," +
               TIME_STAMPS_STR +
               ")";

        public const string table = "shop_products";
        public static string[] fillable = { "id", "product_id", "product_unit_id", "cost_price", "sell_price", "stock_count", "created_at", "updated_at" };
        public int id { get; set; }
        public int? product_id { get; set; }
        public int? product_unit_id { get; set; }
        public double cost_price { get; set; }
        public double sell_price { get; set; }
        public int? stock_count { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public bool isSelected { get; set; }
        public string name {
            get {
                return (productUnit == null ? "" : productUnit.name + " ")
                    + (product == null ? "" : product.name);
            }
        }
        public Product product { get; set; }
        [JsonProperty("product_unit")]
        public ProductUnit productUnit { get; set; }

        public static void multiCreate(List<ShopProduct> shopProducts)
        {
            if (shopProducts == null) return;

            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                foreach (var shopProduct in shopProducts)
                {
                    var insertSql = prepareInsertQuery(table, shopProduct, fillable);

                    cnn.Execute(insertSql, shopProduct);
                }
            }
        }
        public static void multiCreateOrUpdate(List<ShopProduct> shopProducts)
        {
            if (shopProducts == null) return;

            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                foreach (var shopProduct in shopProducts)
                {
                    object queryObject = new { id = shopProduct.id };

                    var query = cnn.Query<ShopProduct>($"SELECT * FROM {table} {prepareEqQuery(queryObject)}",
                        queryObject).ToList();

                    if (query.Count() < 1) //Create new
                    {
                        var insertSql = prepareInsertQuery(table, shopProduct, fillable);

                        cnn.Execute(insertSql, shopProduct);
                    }
                    else //Update
                    {
                        var first = query.First<ShopProduct>();
                        var insertSql = prepareUpdateQuery(table, shopProduct, new { id = shopProduct.id }, fillable);
                        cnn.Execute(insertSql, shopProduct);
                    }
                }
            }
        }
        public static void update(ShopProduct shopProduct)
        {
            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                var updateSql = prepareUpdateQuery(table, shopProduct, new { id = shopProduct.id });

                cnn.Execute(updateSql, shopProduct);
            }
        }

        public static List<ShopProduct> all(string searchName = null, string searchBarcode = null, 
            int searchById = -1)
        {
            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                var sql = $"SELECT * FROM shop_products INNER JOIN products ON " +
                    $"shop_products.product_id = products.id " +
                    $"INNER JOIN product_units ON shop_products.product_unit_id = product_units.id ";

                object queryObj = null;

                if(!string.IsNullOrEmpty(searchName))
                {
                    sql += $" WHERE products.name LIKE @name ";
                    queryObj = new { name = $"%{searchName}%" };
                }

                if(!string.IsNullOrEmpty(searchBarcode))
                {
                    sql += $" WHERE product_units.barcode = @barcode ";
                    queryObj = new { barcode = searchBarcode };
                }

                if(searchById != -1)
                {
                    sql += $" WHERE shop_products.id = @id ";
                    queryObj = new { id = searchById };
                }
                //Helpers.log("Shop Products, searchById = "+ searchById);
                //Helpers.log(sql);
                var query = cnn.Query<ShopProduct, Product, ProductUnit, ShopProduct>
                    (sql, (shopProduct, product, productUnit) =>
                {
                    shopProduct.product = product;
                    shopProduct.productUnit = productUnit;
                    return shopProduct;

                }, queryObj, splitOn: "id");

                return query.ToList();
            }
        }

        //public static ShopProduct get(int id)
        //{
        //    using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
        //    {
        //        object queryObject = new { id = id };

        //        var whereQuery = prepareEqQuery(queryObject);

        //        var sql = $"SELECT * FROM shop_products " +
        //            $" INNER JOIN products ON shop_products.product_id = products.id " +
        //            $" INNER JOIN product_units ON shop_products.product_unit_id = product_units.id ";

        //        var query = cnn.Query<Setting>($"SELECT * FROM {table} {whereQuery}", queryObject).ToList();

        //        return (query == null || query.Count < 1) ? null : query.First();
        //    }
        //}

        public static bool checkIfExist(IDbConnection cnn, int id)
        {
            var query = cnn.Query<ShopProduct>("SELECT id = @id", new { id = id });

            return query.First() != null;
        }

    }
}
