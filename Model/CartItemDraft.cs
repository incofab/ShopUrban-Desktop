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
    public class CartItemDraft : CartItem
    {
        public new const string CREATE_TABLE = "CREATE TABLE IF NOT EXISTS `cart_item_drafts` (" +
            "id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
            "cart_id INTEGER," +
            "shop_product_id INTEGER," +
            "quantity INT(6)," +
            "price decimal(8,2) DEFAULT 0," +
            "discount decimal(8,2) DEFAULT 0," +
            "status VARCHAR(30)," +
            "content TEXT," +
            TIME_STAMPS +
            ")";

        public new static string[] fillable = {"cart_id", "shop_product_id", "quantity", "price", "discount", "status", "content" };

        public const string table = "cart_item_drafts";
        public CartItemDraft()
        {

        }
        public CartItemDraft(CartItem cartItem)
        {
            this.id = cartItem.id;
            this.cart_id = cartItem.cart_id;
            this.content = cartItem.content;
            this.cost_price = cartItem.cost_price;
            this.created_at = cartItem.created_at;
            this.discount = cartItem.discount;
            this.price = cartItem.price;
            this.quantity = cartItem.quantity;
            this.shop_product_id = cartItem.shop_product_id;
        }

        public static void save(CartItemDraft cartItem)
        {
            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                var insertSql = prepareInsertQuery(table, cartItem, fillable);

                cnn.Execute(insertSql, cartItem);
            }
        }

        public static void multiCreate(List<CartItemDraft> cartItems, int? cartId = null)
        {
            if (cartItems == null) return;

            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                foreach (var cartItem in cartItems)
                {
                    if (cartId != null) cartItem.cart_id = (int)cartId;

                    var insertSql = prepareInsertQuery(table, cartItem, fillable);

                    cnn.Execute(insertSql, cartItem);
                }
            }
        }

    }
}
