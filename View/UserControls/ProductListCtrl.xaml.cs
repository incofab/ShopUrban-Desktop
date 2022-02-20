using ShopUrban.Model;
using ShopUrban.Util;
using ShopUrban.View.UserControls.Cart;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ShopUrban.View.UserControls
{
    /// <summary>
    /// Interaction logic for ProductListCtrl.xaml
    /// </summary>
    public partial class ProductListCtrl : UserControl
    {

        public static readonly DependencyProperty ShopProductsProperty =
            DependencyProperty.Register("ShopProducts", typeof(ObservableCollection<ShopProduct>),
                typeof(ProductListCtrl), new UIPropertyMetadata(null));

        public ObservableCollection<ShopProduct> ShopProducts
        {
            get { return (ObservableCollection<ShopProduct>)GetValue(ShopProductsProperty); }
            set { SetValue(ShopProductsProperty, value); }
        }

        public ProductListCtrl()
        {
            InitializeComponent();
            DataContext = this;

            borderCartSection.Child = new CartSectionCtrl();

            this.Loaded += UserControl_Loaded;

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.Background = Brushes.Transparent;
            }
            MyEventBus.subscribe(handleEvent);
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

        public void displayProducts(List<ShopProduct> s)
        {
            ShopProducts = new ObservableCollection<ShopProduct>(s);
        }

        public void loadView()
        {
            List<ShopProduct> ShopProducts = getItems();

            displayProducts(ShopProducts);
        }

        public List<ShopProduct> getItems()
        {
            List<ShopProduct> s = ShopProduct.all();

            return s;

            List<ShopProduct> shopProducts = new List<ShopProduct>();
            
            for (int i = 0; i < 0; i++)
            {
                Product product = new Product { name = $"Product {i}", id = i, };
                
                ProductUnit productUnit = new ProductUnit { name = $"Product Unit{i}", id = i, product_id = i,};
                
                ShopProduct shopProduct = new ShopProduct { 
                    id = i,
                    product_id = product.id,
                    product_unit_id = productUnit.id,
                    product = product, 
                    productUnit = productUnit, 
                    sell_price = i * 10000 + i + 0.0304,
                };

                shopProducts.Add(shopProduct);
            }

            return shopProducts;
        }

        private void tbSearch_KeyUp(object sender, KeyEventArgs e)
        {
            //string searchQuery = tbSearch.Text.Trim().ToString();

            //if (searchQuery.Length < 2)
            //{
            //    displayProducts(ShopProduct.all());

            //    return;
            //}

            //List<ShopProduct> s = ShopProduct.all(searchQuery);

            //if(s.Count == 1)
            //{
            //    var shopProduct = s[0];

            //    if(shopProduct.stock_count > 0)
            //    {
            //        MyEventBus.post(new EventMessage(EventMessage.EVENT_ADD_TO_CART, shopProduct));
            //    }
            //}

            //displayProducts(s);
        }

        private void handleEvent(EventMessage eventMessage)
        {
            switch (eventMessage.eventId)
            {
                case EventMessage.EVENT_SHOP_PRODUCT_UPDATED:
                case EventMessage.EVENT_ORDER_CREATED:
                    loadView();
                    tbSearch.Focus();
                    break;

                case EventMessage.EVENT_ADD_TO_CART:

                    tbSearch.Focus();

                    if (!string.IsNullOrEmpty(tbSearch.Text)) tbSearch.Text = "";

                    break;

                case EventMessage.EVENT_CART_CLEARED:
                    TimerHelper.SetTimeout(50, () => {
                        Application.Current.Dispatcher.Invoke(new Action(() => { 
                            tbSearch.Focus();
                        }));
                    });
                    break;
            }
        }

        private void clearSearch_Click(object sender, RoutedEventArgs e)
        {
            tbSearch.Text = "";
            //displayProducts(ShopProduct.all());
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchQuery = tbSearch.Text.Trim().ToString();

            if (searchQuery.Length < 2)
            {
                displayProducts(ShopProduct.all());

                return;
            }

            List<ShopProduct> s = ShopProduct.all(searchQuery);

            if (s.Count == 1)
            {
                var shopProduct = s[0];

                if (shopProduct.stock_count > 0)
                {
                    MyEventBus.post(new EventMessage(EventMessage.EVENT_ADD_TO_CART, shopProduct));
                    //tbSearch.Text = "";
                    return;
                }
            }

            displayProducts(s);
        }

    }
}
