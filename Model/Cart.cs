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
    public class Cart :BaseModel
    {
        public const string CREATE_TABLE = "CREATE TABLE IF NOT EXISTS `carts` (" +
            "id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
            "quantity INT(6)," +
            "amount decimal(8,2) DEFAULT 0," +
            "status VARCHAR(30)," +
            TIME_STAMPS +
            ")";

        public static string[] fillable = { "quantity", "amount", "status"};

        const string table = "carts";
        public int id { get; set; }
        public int quantity {get; set; }
        public double amount { get; set; }
        public string status { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string amountNaira { get { return Helpers.naira(amount); } }
        public List<CartItem> cartItems { get; set; }

        public static void save(Cart cart)
        {
            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                var insertSql = prepareInsertQuery(table, cart, fillable);

                cnn.Execute(insertSql, cart);
            }
        }
        public static Cart createAndReturn(Cart cart)
        {
            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                var insertSql = prepareInsertQuery(table, cart, fillable);

                cnn.Execute(insertSql, cart);

                var sql = $"SELECT * FROM {table} ORDER BY id DESC LIMIT 1";

                return cnn.Query<Cart>(sql).First();
            }
        }

        public static Order saveRecords(List<CartItem> cartItems, Order evaluatedOrder, 
            Cart evaluatedCart, string paymentType)
        {
            var cart = Cart.createAndReturn(evaluatedCart);

            cart.cartItems = cartItems;

            CartItem.multiCreate(cartItems, cart.id);

            var staff = Helpers.getLoggedInStaff();

            evaluatedOrder.cart_id = cart.id;
            evaluatedOrder.order_number = Order.generateInvoiceNo();
            evaluatedOrder.status = (evaluatedOrder.amount > evaluatedOrder.amount_paid) ? "incomplete payment" : "paid";

            // Create the Order
            var order = Order.create(evaluatedOrder);

            OrderPayment.create(new OrderPayment { 
                order_number_track = order.order_number,
                amount = order.amount_paid,
                remaining_amount = order.remaining_amount,
                payment_type = paymentType,
                user_id = order.user_id,
                is_confirmed = 1
            });

            order.cart = cart;
            if(order.orderPayments == null || order.orderPayments.Count <= 0)
            {
                order.orderPayments = new List<OrderPayment>();
            }

            order.orderPayments = order.orderPayments;

            Helpers.log("Order recorded successfully, You can now print the receipt");

            MyEventBus.post(new EventMessage(EventMessage.EVENT_ORDER_CREATED, null));

            return order;
        }

        public override string ToString()
        {
            Helpers.log(
                "id = " + id
                + "amount = " + amount
                + ", quantity = " + quantity
                + ", status = " + status
                + ", updated_at = " + updated_at
                + ", created_at = " + created_at
                );

            return base.ToString();
        }
    }
}
