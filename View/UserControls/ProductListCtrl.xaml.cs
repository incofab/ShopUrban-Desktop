using ShopUrban.Model;
using ShopUrban.Util;
using ShopUrban.View.UserControls.Cart;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
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
            DependencyProperty.Register("ShopProducts", typeof(List<ShopProduct>),
                typeof(ProductListCtrl), new UIPropertyMetadata(null));

        public List<ShopProduct> ShopProducts
        {
            get { return (List<ShopProduct>)GetValue(ShopProductsProperty); }

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

            refreshProductCategoryButtons();
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

        private int productsPerPage = 1000;
        public void displayProducts(List<ShopProduct> s)
        {
            //Application.Current.Dispatcher.Invoke(new Action(() => {
            var len = s.Count;
            ShopProducts = s.GetRange(0, len > productsPerPage ? productsPerPage : len);

            if(ShopProducts != null && ShopProducts.Count > 0)
            {
                UniformGridPanel uniformGridPanel = FindVisualChild<UniformGridPanel>(lbProducts);

                if (uniformGridPanel != null) uniformGridPanel.ScrollOwner.ScrollToTop();
            }
            //}));
        }

        public void loadView()
        {
            List<ShopProduct> ShopProducts = getItems();

            displayProducts(ShopProducts);
        }

        public List<ShopProduct> getItems()
        {
            return ShopProduct.all();

            #region dummy ShopProducts
            /*
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
            */
            #endregion
        }

        private void handleEvent(EventMessage eventMessage)
        {
            switch (eventMessage.eventId)
            {
                case EventMessage.EVENT_PRODUCT_SYNC_COMPLETED:
                case EventMessage.EVENT_SHOP_PRODUCT_UPDATED:
                    refreshProductCategoryButtons();
                    loadView();
                    tbSearch.Focus();
                    break;

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

        private string prevSearchQuery = "";
        private void clearSearch_Click(object sender, RoutedEventArgs e)
        {
            tbSearch.Text = "";
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchQuery = tbSearch.Text.Trim().ToString();

            if (searchQuery.Length < 2)
            {
                if (searchQuery.Length == 2 || prevSearchQuery.Length == 0)
                {
                    prevSearchQuery = searchQuery;
                    return;
                }

                //TimerHelper.SetTimeout(1000, () => { 
                    displayProducts(getItems());
                //});

                prevSearchQuery = searchQuery;

                return;
            }

            List<ShopProduct> s = ShopProduct.all(searchQuery);

            if (s.Count == 1 && isBarcode(searchQuery))
            {
                var shopProduct = s[0];

                if (shopProduct.stock_count > 0 && shopProduct.sell_price > 0)
                {
                    MyEventBus.post(new EventMessage(EventMessage.EVENT_ADD_TO_CART, shopProduct));

                    prevSearchQuery = searchQuery;

                    return;
                }
                else
                {
                    var errorMessage = "";
                    if (shopProduct.stock_count < 1) errorMessage += "Product out of stock\n";
                    if (shopProduct.sell_price < 1) errorMessage += "Please, update product price";

                    Helpers.playErrorSound();
                    Toast.showError(errorMessage);
                }
            }

            prevSearchQuery = searchQuery;

            displayProducts(s);
        }

        private bool isBarcode(string str)
        {
            return str.Length > 9 && Helpers.IsDigits(str);
        }

        #region Handling Product Category
        private void refreshProductCategoryButtons()
        {
            spCategory.Children.Clear();

            //List<Button> buttons = new List<Button>();
            List<ProductCategory> pcs = new List<ProductCategory>();
            pcs.Add(new ProductCategory { id = 0, name = "All", slug = "" });
            pcs.AddRange(ProductCategory.all());

            List<string> check = new List<string>();
            foreach (var productCategory in pcs)
            {
                if(check.Contains(productCategory.name)){
                    continue;
                }
                
                check.Add(productCategory.name);

                Button b = new Button();
                b.Content = productCategory.name;
                b.Click += category_button_Click;
                b.Name = "btn_" + Helpers.toAlphaNum(productCategory.slug);
                b.Style = Application.Current.Resources["MaterialDesignOutlinedButton"] as Style;
                b.Margin = new Thickness(5, 0, 5, 0);
                b.Tag = productCategory.id;
                
                if(productCategory.id == 0) b.Background = Brushes.LightBlue;

                spCategory.Children.Add(b);
            }
        }

        private string selectedCategory = "All";
        private void category_button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            selectedCategory = button.Content.ToString();

            List<ShopProduct> s = ShopProduct.all(null, null, -1, (int)button.Tag);
            displayProducts(s);

            foreach (var item in spCategory.Children)
            {
                var b = item as Button;

                if(string.Equals(b.Content.ToString(), selectedCategory))
                {
                    b.Background = Brushes.LightBlue;
                }
                else
                {
                    b.Background = Brushes.Transparent;
                }
            }
        }
        #endregion

        public TChildItem FindVisualChild<TChildItem>(DependencyObject obj) where TChildItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                
                //Helpers.log("Child " + i + " = " + child.ToString());

                if (child != null && child is TChildItem)
                    return (TChildItem)child;

                var childOfChild = FindVisualChild<TChildItem>(child);

                if (childOfChild != null)
                    return childOfChild;
            }

            return null;
        }

    }
}
