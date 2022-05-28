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
    public class ProductCategory : BaseModel
    {
        public const string CREATE_TABLE = "CREATE TABLE IF NOT EXISTS `product_categories` (" +
            "id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
            "name VARCHAR, " +
            "slug VARCHAR " +
            //TIME_STAMPS +
            ")";

        public static string[] fillable = { "name", "slug" };

        const string table = "product_categories";
        public int id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        //public string created_at { get; set; }
        //public string updated_at { get; set; }

        public static void save(ProductCategory productCategory)
        {
            var insertSql = prepareInsertQuery(table, productCategory, fillable);

            DBCreator.getConn().Execute(insertSql, productCategory);
        }
        public static void update(ProductCategory productCategory)
        {
            var updateSql = prepareUpdateQuery(table, productCategory, new { id = productCategory.id }, fillable);

            DBCreator.getConn().Execute(updateSql, productCategory);
        }

        public static void createOrUpdate(ProductCategory productCategory)
        {
            if (productCategory == null) return;

            var cnn = DBCreator.getConn();

            object queryObject = new { id = productCategory.id };

            var query = cnn.Query<ProductCategory>($"SELECT * FROM {table} {prepareEqQuery(queryObject)}",
                queryObject).ToList();

            if (query.Count() < 1) //Create new
            {
                save(productCategory);
            }
            else //Update
            {
                ProductCategory productCat = query[0];

                if(!string.Equals(productCategory.name, productCat.name))
                {
                    update(productCategory);
                }
            }
        }

        public static List<ProductCategory> all()
        {
            var cnn = DBCreator.getConn();

            return cnn.Query<ProductCategory>($"SELECT * FROM {table}").ToList();
        }

        public static void saveRecords(List<ProductCategory> productCategories)
        {
            foreach (var productCategory in productCategories)
            {
                createOrUpdate(productCategory);
            }
        }

    }
}
