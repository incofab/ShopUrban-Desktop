using Dapper;
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
    public class CartDraft : Cart
    {
        public new const string CREATE_TABLE = "CREATE TABLE IF NOT EXISTS `cart_drafts` (" +
            "id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
            "quantity INT(6)," +
            "amount decimal(8,2) DEFAULT 0," +
            "status VARCHAR(30)," +
            TIME_STAMPS +
            ")";

        public new static string[] fillable = { "quantity", "amount", "status" };

        const string table = "cart_drafts";

        public new List<CartItemDraft> cartItems { get; set; }

        public static List<CartDraft> all(string orderBy = "ASC")
        {
            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                var cartDrafts = cnn.Query<CartDraft>($"SELECT * FROM {table}").ToList<CartDraft>();
                
                foreach (var cartDraft in cartDrafts)
                {
                    var sql = $" SELECT * FROM {CartItemDraft.table} " +
                    $" LEFT JOIN shop_products ON cart_item_drafts.shop_product_id = shop_products.id " +
                    $" WHERE {CartItemDraft.table}.cart_id = @cart_id";

                    var obj = new { cart_id = cartDraft.id };
                    //Helpers.log("SQL Query = " + sql);
                    var cartItemDrafts = cnn.Query<CartItemDraft, ShopProduct, CartItemDraft>
                        (sql, (cartItemDraft, shopProduct) =>
                        {
                            cartItemDraft.shopProduct = shopProduct;

                            return cartItemDraft;

                        }, obj, splitOn: "id").ToList<CartItemDraft>();

                    cartDraft.cartItems = cartItemDrafts;
                }

                return cartDrafts;
            }
        }
        public static void save(CartDraft cart)
        {
            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                var insertSql = prepareInsertQuery(table, cart, fillable);

                cnn.Execute(insertSql, cart);
            }
        }
        public static CartDraft createAndReturn(CartDraft cart)
        {
            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                var insertSql = prepareInsertQuery(table, cart, fillable);

                cnn.Execute(insertSql, cart);

                var sql = $"SELECT * FROM {table} ORDER BY id DESC LIMIT 1";

                return cnn.Query<CartDraft>(sql).First();
            }
        }

        public static void saveRecords(List<CartItemDraft> cartItems)
        {
            double amount = 0;
            int quantity = 0;
            foreach (var cartItem in cartItems)
            {
                amount += cartItem.totalPrice;
                quantity += cartItem.quantity;
            }

            var cart = CartDraft.createAndReturn(new CartDraft()
            {
                amount = amount,
                quantity = quantity,
                status = "1"
            });

            cart.cartItems = cartItems;

            CartItemDraft.multiCreate(cartItems, cart.id);

            Helpers.log("Draft records successfully");

            MyEventBus.post(new EventMessage(EventMessage.EVENT_DRAFT_CREATED, null));

            return;
        }

        public static void delete(CartDraft cart)
        {
            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                var deleteObj = new { id = cart.id };
                var deleteObj2 = new { cart_id = cart.id };

                var deleteSql = deleteQuery(CartDraft.table, deleteObj);

                //Trace.WriteLine("deleteSql = " + deleteSql);

                cnn.Execute(deleteSql, deleteObj);

                var deleteCartItemDraftSql = deleteQuery(CartItemDraft.table, deleteObj2);

                //Trace.WriteLine("deleteCartItemDraftSql = " + deleteCartItemDraftSql);

                cnn.Execute(deleteCartItemDraftSql, deleteObj2);
            }
        }

    }
}
