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
    public class Product :BaseModel
    {
        public const string CREATE_TABLE = "CREATE TABLE IF NOT EXISTS `products` (" +
            "id INTEGER NOT NULL," +
            "brand_id INTEGER," +
            "name VARCHAR(255)," +
            "barcode VARCHAR(255)," +
            "slug VARCHAR(255)," +
            "photo VARCHAR(255)," +
            TIME_STAMPS_STR +
            ")";

        public const string table = "products";

        public static string[] fillable = {"id", "name", "slug", "brand_id", "photo", "barcode", "created_at", "updated_at" };
        public int id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public int brand_id { get; set; }
        public string barcode { get; set; }
        public string photo { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        [JsonProperty("product_units")]
        public List<ProductUnit> productUnits { get; set; }

        [JsonProperty("shop_products")]
        public List<ShopProduct> shopProducts { get; set; }

        public static void create(Product product)
        {
            var insertSql = prepareInsertQuery(table, product, fillable);

            execute(insertSql, product);
        }

        public static void createOrUpdate(Product product)
        {
            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                object queryObject = new { id = product.id };

                var query = cnn.Query<Product>($"SELECT * FROM {table} {prepareEqQuery(queryObject)}",
                    queryObject).ToList();

                if (query.Count() < 1) //Create new
                {
                    var insertSql = prepareInsertQuery(table, product, fillable);

                    cnn.Execute(insertSql, product);
                }
                else //Update
                {
                    //var first = query.First<Product>();
                    var insertSql = prepareUpdateQuery(table, product, new { id = product.id }, fillable);
                    cnn.Execute(insertSql, product);
                }
            }
        }

        public override string ToString()
        {
            Helpers.log(
                "name = " + name
                + ", slug = " + slug
                + ", brand_id = " + brand_id
                + ", barcode = " + barcode
                + ", photo = " + photo
                + ", created_at = " + created_at
                + ", updated_at = " + updated_at
                );

            return base.ToString();
        }

    }
}
