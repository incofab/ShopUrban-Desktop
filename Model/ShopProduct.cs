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
               "restock_alert INTEGER," +
               "expired_date VARCHAR," +
               TIME_STAMPS_STR +
               ")";

        public const string table = "shop_products";
        public static string[] fillable = {
            "id", "product_id", "product_unit_id", "cost_price", "sell_price", "stock_count",
            "restock_alert", "expired_date", "created_at", "updated_at"
        };
        public int id { get; set; }
        public int? product_id { get; set; }
        public int? product_unit_id { get; set; }
        public decimal cost_price { get; set; }
        public decimal sell_price { get; set; }
        public int? stock_count { get; set; }
        public int restock_alert { get; set; }
        public string expired_date { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public bool isSelected { get; set; }
        public string name {
            get {
                return (product == null ? "" : product.name + " ") + (productUnit == null ? "" : productUnit.name);
            }
        }
        public Product product { get; set; }
        [JsonProperty("product_unit")]
        public ProductUnit productUnit { get; set; }

        public static void multiCreate(List<ShopProduct> shopProducts)
        {
            if (shopProducts == null) return;

            var cnn = DBCreator.getConn();

            foreach (var shopProduct in shopProducts)
            {
                var insertSql = prepareInsertQuery(table, shopProduct, fillable);

                cnn.Execute(insertSql, shopProduct);
            }
        }

        public static void multiCreateOrUpdate(List<ShopProduct> shopProducts)
        {
            if (shopProducts == null) return;

            var cnn = DBCreator.getConn();

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
        public static void createOrUpdate(ShopProduct shopProduct)
        {
            if (shopProduct == null) return;

            var cnn = DBCreator.getConn();

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

        public static void update(int id, object shopProduct)
        {
            DateTime.Now.ToString(KStrings.TIME_FORMAT);
            var cnn = DBCreator.getConn();

            var updateSql = prepareUpdateQuery(table, shopProduct, new { id = id }, fillable);

            cnn.Execute(updateSql, shopProduct);
        }

        public static List<ShopProduct> all(string searchName = null, string searchBarcode = null, 
            int searchById = -1, int productCategoryId = 0, string limit = " LIMIT 100 OFFSET 0 ")
        {
            IDbConnection cnn = DBCreator.getConn();

            var sql = $"SELECT * FROM shop_products INNER JOIN products ON " +
                $"shop_products.product_id = products.id " +
                $"INNER JOIN product_units ON shop_products.product_unit_id = product_units.id ";

            object queryObj = null;

            var prefix = " WHERE ";
            if (!string.IsNullOrEmpty(searchName))
            {
                sql += $" WHERE products.name LIKE @name OR product_units.name LIKE @name " +
                    $" OR product_units.barcode = @barcode ";
                queryObj = new { name = $"%{searchName}%", barcode = searchName };
                prefix = " AND ";
            }

            if(!string.IsNullOrEmpty(searchBarcode))
            {
                sql += $" WHERE product_units.barcode = @barcode ";
                queryObj = new { barcode = searchBarcode };
                prefix = " AND ";
            }

            if(searchById != -1)
            {
                sql += $" WHERE shop_products.id = @id ";
                queryObj = new { id = searchById };
                prefix = " AND ";
            }

            if(productCategoryId != 0)
            {
                sql += $" {prefix} products.product_category_id = @product_category_id ";
                queryObj = new { product_category_id = productCategoryId };
            }

            sql += limit;

            var query = cnn.Query<ShopProduct, Product, ProductUnit, ShopProduct>
                (sql, (shopProduct, product, productUnit) =>
            {
                shopProduct.product = product;
                shopProduct.productUnit = productUnit;
                return shopProduct;

            }, queryObj, splitOn: "id");

            var endQuery = DateTime.Now.Millisecond;

            return query.ToList();
        }

        public static bool checkIfExist(IDbConnection cnn, int id)
        {
            var query = cnn.Query<ShopProduct>("SELECT id = @id", new { id = id });

            return query.First() != null;
        }

    }
}
