using ShopUrban.Model;
using ShopUrban.Util;
using ShopUrban.View.Dialogs;
using ShopUrban.View.Receipt;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShopUrban.View.UserControls.Cart
{
    /// <summary>
    /// Interaction logic for CartSection.xaml
    /// </summary>
    public partial class CartSectionCtrl : UserControl
    {
        public Dictionary<int, CartItemUserCtrl> cartItemCtrls { get; set; }

        public string cartTotalPrice
        {
            get { return (string)GetValue(cartTotalPriceProperty); }
            set { SetValue(cartTotalPriceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for cartTotalPriceProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty cartTotalPriceProperty =
            DependencyProperty.Register("cartTotalPrice", typeof(string), 
                typeof(CartSectionCtrl), new PropertyMetadata(""));

        public double shippingCost { get; set; }
        public Staff staff { get; set; }

        public CartSectionCtrl()
        {
            InitializeComponent();

            DataContext = this;

            this.Loaded += UserControl_Loaded;

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.Background = Brushes.Transparent;
            }

            clearCart();

            MyEventBus.subscribe(handleEvent);

            staff = Helpers.getLoggedInStaff();
        }

        void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            window.Closing += window_Closing;
        }

        void window_Closing(object sender, global::System.ComponentModel.CancelEventArgs e)
        {
            //do something before the window is closed...
            MyEventBus.unSubscribe(handleEvent);
        }

        private void clearCart()
        {
            if (cartItemCtrls == null) cartItemCtrls = new Dictionary<int, CartItemUserCtrl>();
            //if (cartItemCtrls == null) cartItemCtrls = new ObservableCollection<CartItemCtrl>();

            foreach (var cartItemCtrl in cartItemCtrls)
            {
                cartItemCtrl.Value.cartItem.shopProduct.isSelected = false;

                //Don't modify dictionary inside a loop
                //cartItemCtrls.Remove(cartItemCtrl.Key);
            }
            
            cartItemCtrls.Clear();

            updateUI();

            MyEventBus.post(new EventMessage(EventMessage.EVENT_CART_CLEARED));
        }

        private void updateUI()
        {
            cartItemContainer.Children.Clear();

            if(cartItemCtrls == null || cartItemCtrls.Count < 1)
            {
                cartBox.Visibility = Visibility.Collapsed;
                boxCartEmpty.Visibility = Visibility.Visible;
                return;
            }

            cartBox.Visibility = Visibility.Visible;
            boxCartEmpty.Visibility = Visibility.Collapsed;

            foreach (var cartItemCtrl in cartItemCtrls)
            {
                cartItemContainer.Children.Add(cartItemCtrl.Value);
            }

            updateCartTotalPrice();
        }

        public void updateCartTotalPrice()
        {
            decimal total = 0;
            decimal totalQuantity = 0;
            foreach (var cartItemCtrl in cartItemCtrls)
            {
                total += cartItemCtrl.Value.cartItem.totalPrice;
                totalQuantity += cartItemCtrl.Value.cartItem.quantity;
            }

            tbItemCount.Text = totalQuantity + " items";
            cartTotalPrice = Helpers.naira(total);
            tbTotalPrice.Text = Helpers.naira(total);
        }

        private void addCartItem(ShopProduct shopProduct)
        {
            if (cartItemCtrls.ContainsKey(shopProduct.id))
            {
                cartItemCtrls[shopProduct.id].increaseQuantity();
                //cartItemCtrls. GetValueOrDefault(shopProduct.id).increaseQuantity();
                return;
            }

            var cartItem = new CartItem
            {
                shopProduct = shopProduct,
                shop_product_id = shopProduct.id,
                quantity = 1,
                status = "active",
                price = shopProduct.sell_price,
                cost_price = shopProduct.cost_price
            };

            cartItemCtrls.Add(shopProduct.id, new CartItemUserCtrl(this, cartItem));
        }

        private void addCartDraft(CartDraft cartDraft)
        {
            //Trace.WriteLine(cartDraft.ToString());
            foreach (CartItem cartItemDraft in cartDraft.cartItems)
            {
                List<ShopProduct> shopProducts = ShopProduct.all(null, null, cartItemDraft.shop_product_id);

                if (shopProducts.Count != 1) continue;

                cartItemDraft.shopProduct = shopProducts[0];

                //Trace.WriteLine(cartItemDraft.ToString());
                //Trace.WriteLine("cartItemDraft.shopProduct.id = "+ cartItemDraft.shopProduct.id);

                cartItemCtrls.Add(cartItemDraft.shopProduct.id, new CartItemUserCtrl(this, cartItemDraft));
            }
        }

        public void removeCartItem(CartItemUserCtrl cartItemCtrl)
        {
            cartItemCtrl.cartItem.shopProduct.isSelected = false;
            cartItemCtrls.Remove(cartItemCtrl.cartItem.shopProduct.id);
            updateUI();
        }

        private void handleEvent(EventMessage eventMessage)
        {
            switch (eventMessage.eventId)
            {
                case EventMessage.EVENT_ADD_TO_CART:
                    addCartItem((ShopProduct)eventMessage.data);
                    updateUI();
                    break;

                case EventMessage.EVENT_ORDER_CREATED:
                    clearCart();
                    updateUI();
                    AutoSyncOrders.getInstance().start();
                    break;

                case EventMessage.EVENT_DRAFT_CREATED:
                    clearCart();
                    updateUI();
                    break;
                case EventMessage.EVENT_DRAFT_EDIT:
                    clearCart();
                    addCartDraft((CartDraft)eventMessage.data);
                    updateUI();
                    break;

                default:
                    break;
            }
        }

        private void btnCheckout_Click(object sender, RoutedEventArgs e)
        {
            List<CartItem> cartItemsArr = new List<CartItem>();

            foreach (var cartItemCtrl in cartItemCtrls)
            {
                cartItemsArr.Add(cartItemCtrl.Value.cartItem);
            }

            new OrderSummaryDialog(cartItemsArr).ShowDialog();

            /*
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Confirm Order", MessageBoxButton.YesNo);
            
            if (messageBoxResult != MessageBoxResult.Yes) return;
        
            var order = Model.Cart.saveRecords(cartItemsArr);

            //new Thermal88mm(order).ShowDialog();
            Helpers.showReceipt(order);
            //new OrderSummaryDialog(new ObservableCollection<CartItem>(cartItemsArr)).ShowDialog();
            */
        }

        private void btnDraft_Click(object sender, RoutedEventArgs e)
        {
            List<CartItemDraft> cartItemsArr = new List<CartItemDraft>();

            foreach (var cartItemCtrl in cartItemCtrls)
            {
                cartItemsArr.Add(new CartItemDraft(cartItemCtrl.Value.cartItem));
            }

            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Save as draft", MessageBoxButton.YesNo);

            if (messageBoxResult != MessageBoxResult.Yes) return;

            CartDraft.saveRecords(cartItemsArr);
        }

        private void btnClearCart_Click(object sender, RoutedEventArgs e)
        {
            clearCart();
        }
    }
}
