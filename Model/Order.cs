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
    public class Order :BaseModel
    {
        public const string CREATE_TABLE = "CREATE TABLE IF NOT EXISTS `orders` (" +
            "id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
            "cart_id INTEGER," +
            "staff_id INTEGER," +
            "user_id INTEGER," +
            "order_number VARCHAR(255)," +
            "amount decimal(8,2) DEFAULT 0," +
            "shipping_cost decimal(8,2) DEFAULT 0," +
            "status VARCHAR(30)," +
            "total_cost_price decimal(8,2) DEFAULT 0," +
            "profit decimal(8,2) DEFAULT 0," +
            "amount_to_pay decimal(8,2) DEFAULT 0," +
            "amount_paid decimal(8,2) DEFAULT 0," +
            "remaining_amount decimal(8,2) DEFAULT 0," +
            "vat_amount decimal(8,2) DEFAULT 0," +
            "customer_name VARCHAR," +
            TIME_STAMPS +
            ")";

        public static string[] fillable = {
            "cart_id", "staff_id", "user_id", "order_number", "amount", "status", "shipping_cost",
            "total_cost_price", "profit", "amount_to_pay", "amount_paid", "remaining_amount", "vat_amount",
            "customer_name",
        };

        const string table = "orders";
        public int id { get; set; }
        public int cart_id { get; set; }
        public int user_id { get; set; }
        public int staff_id { get; set; }
        public string order_number { get; set; }
        public double amount { get; set; }
        public double shipping_cost { get; set; }
        public string status { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string amountNaira { get { return Helpers.naira(amount); } }
        //public double totalAmount { get { return amount + shipping_cost; } }
        public string totalAmountNaira { get { return Helpers.naira(amount_to_pay); } }
        public double total_cost_price { get; set; }
        public double profit { get; set; }
        public double amount_to_pay { get; set; }
        public double amount_paid { get; set; }
        public double remaining_amount { get; set; }
        public double vat_amount { get; set; }
        public string amountPaidNaira { get { return Helpers.naira(amount_paid); } }
        public string remainingAmountNaira { get { return Helpers.naira(remaining_amount); } }
        public string customer_name { get; set; }
        public Cart cart { get; set; }
        public List<OrderPayment> orderPayments { get; set; }
        public Staff staff { get; set; }

        public static List<Order> all(int lastOrderId = 0, string orderBy = "ASC")
        {
            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                object queryObject = new { id = lastOrderId };

                //var sql = $" SELECT orders.*, carts.*, staff.* FROM {table} " +
                var sql = $" SELECT * FROM {table} " +
                $" LEFT JOIN carts ON orders.cart_id = carts.id " +
                $" LEFT JOIN staff ON orders.staff_id = staff.id " +
                //$" LEFT JOIN order_payments ON orders.order_number = order_payments.order_number_track " +
                $" WHERE orders.id > @id ORDER BY orders.id {orderBy}";

                //Helpers.log("Order Query = " + sql);
                var orderList = cnn.Query<Order, Cart, Staff, Order>
                    (sql, (order, cart, staff) =>
                    {
                        order.cart = cart;
                        order.staff = staff;
                        //Helpers.log("Order " + order.order_number);
                        //Helpers.log(JsonConvert.SerializeObject(order));

                        //if (order.orderPayments == null) order.orderPayments = new List<OrderPayment>();
                        //order.orderPayments.Add(orderPayment);

                        return order;

                    }, queryObject, splitOn: "id").ToList<Order>();

                //Helpers.log("OrderList Count = " + orderList.Count);
                //List<Order> orderList = query.ToList<Order>();

                if (orderList == null) return null;

                foreach (var order in orderList)
                {
                    if (order.cart == null) continue;

                    var sql2 = $"SELECT * FROM cart_items WHERE cart_id = @cart_id";
                    object queryObject2 = new { cart_id = order.cart.id };

                    order.cart.cartItems = cnn.Query<CartItem>(sql2, queryObject2).ToList();

                    var sql3 = $"SELECT * FROM order_payments WHERE order_number_track = @order_number_track";
                    object queryObject3 = new { order_number_track = order.order_number };

                    order.orderPayments = cnn.Query<OrderPayment>(sql3, queryObject3).ToList();
                }

                //var query = cnn.Query<Order>(sql);
                return orderList;
            }
        }

        public static Order create(Order order)
        {
            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                var insertSql = prepareInsertQuery(table, order, fillable);

                cnn.Execute(insertSql, order);

                var sql = $"SELECT * FROM {table} ORDER BY id DESC LIMIT 1";

                return cnn.Query<Order>(sql).First();
            }
        }

        public static string generateInvoiceNo()
        {
            Random rand = new Random();

            var orderNumber = DateTime.Now.Year.ToString().Substring(2)+"00"
                + rand.Next(100000, 999999);

            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                while (InvoiceNoExists(cnn, orderNumber))
                {
                    orderNumber = DateTime.Now.Year.ToString().Substring(2)+"00"
                        + rand.Next(100000, 999999);
                }
            }

            return orderNumber;
        }

        public static bool InvoiceNoExists(IDbConnection cnn, string orderNumber)
        {
            var queryObject = new { order_number = orderNumber };

            var query = cnn.Query<Order>("SELECT * FROM orders "+prepareEqQuery(queryObject),
                    queryObject).ToList();

            return query != null && query.Count() > 0;
        }

        public static List<Order> getUnUploadedOrders()
        {
            int.TryParse(Setting.getValue(Setting.KEY_ORDER_LAST_SYNC_ID), out int orderLastId);

            string shopId = Setting.getShopId();

            return Order.all(orderLastId);
        }
    }
}
