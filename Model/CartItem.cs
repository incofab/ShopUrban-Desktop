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
    public class CartItem :BaseModel
    {
        public const string CREATE_TABLE = "CREATE TABLE IF NOT EXISTS `cart_items` (" +
            "id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
            "cart_id INTEGER," +
            "shop_product_id INTEGER," +
            "quantity INT(6)," +
            "price decimal(8,2) DEFAULT 0," +
            "discount decimal(8,2) DEFAULT 0," +
            "cost_price decimal(8,2) DEFAULT 0," +
            "status VARCHAR(30)," +
            "content TEXT," +
            TIME_STAMPS +
            ")";

        public static string[] fillable = {
            "cart_id", "shop_product_id", "quantity", "price", "discount", "status", "content", "cost_price"
        };

        const string table = "cart_items";
        public int id { get; set; }
        public int cart_id { get; set; }
        public int shop_product_id { get; set; }
        private double _price { get; set; }
        public double price { get { return _price; } 
            set {
                _price = value;
                //cost_price = _quantity * _price;
                NotifyPropertyChanged("price"); 
            } 
        }
        public double discount { get; set; }
        public double cost_price { get; set; }
        private int _quantity { get; set; }
        public int quantity { 
            get { return _quantity; } 
            set { 
                _quantity = value;
                //cost_price = _quantity * _price;
                NotifyPropertyChanged("quantity");

                //if(shopProduct != null){
                //    price = value * shopProduct.sell_price;
                //}
            } 
        }
        public string status { get; set; }
        public string content { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string priceNaira { get { return Helpers.naira(price); } }
        public double totalPrice { get { return quantity * price; } }
        public string totalPriceNaira { get { return Helpers.naira(totalPrice); } }
        public ShopProduct shopProduct { get; set; }

        public static void save(CartItem cartItem)
        {
            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                var insertSql = prepareInsertQuery(table, cartItem, fillable);

                cnn.Execute(insertSql, cartItem);
            }
        }

        public static void multiCreate(List<CartItem> cartItems, int? cartId = null)
        {
            if (cartItems == null) return;

            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                foreach (var cartItem in cartItems)
                {
                    if (cartId != null) cartItem.cart_id = (int)cartId;

                    var insertSql = prepareInsertQuery("cart_items", cartItem, fillable);

                    cnn.Execute(insertSql, cartItem);

                    if (cartItem.shopProduct == null) continue;

                    //Update the shop product, for stock_count
                    cartItem.shopProduct.stock_count -= cartItem.quantity;

                    var updateSql = prepareUpdateQuery(ShopProduct.table, 
                        cartItem.shopProduct, new { id = cartItem.shopProduct.id }, 
                        ShopProduct.fillable);

                    cnn.Execute(updateSql, cartItem.shopProduct);
                }
            }
        }

    }
}
