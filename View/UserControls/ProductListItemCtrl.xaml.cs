using ShopUrban.Model;
using ShopUrban.Util;
using ShopUrban.View.Dialogs;
using ShopUrban.View.Receipt;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for ProductListItemCtrl.xaml
    /// </summary>
    public partial class ProductListItemCtrl : UserControl
    {
        public static readonly DependencyProperty shopProductProperty =
            DependencyProperty.Register("shopProduct", typeof(ShopProduct), 
                typeof(ProductListItemCtrl), new UIPropertyMetadata(null));

        public ShopProduct shopProduct
        {
            get { return (ShopProduct)GetValue(shopProductProperty); }
            set { SetValue(shopProductProperty, value); }
        }

        //public ShopProduct shopProduct { get; set; }

        public ProductListItemCtrl()
        {
            InitializeComponent();
        }

        private void updateUI()
        {
            tbProductName.Text = shopProduct.name;
            tbStockCount.Text = (shopProduct.stock_count == null ? 0 : shopProduct.stock_count) + " left";

            tbPrice.Text = Helpers.naira(shopProduct.sell_price);

            var imgSrc = File.Exists(shopProduct.productUnit.local_photo)
                ? shopProduct.productUnit.local_photo 
                : "pack://application:,,," + @"/logo.ico";

            try
            {
                imgProductImage.ImageSource = new BitmapImage(new Uri(imgSrc));
            }
            catch (Exception e)
            {
                imgProductImage.ImageSource = new BitmapImage(new Uri("pack://application:,,," + @"/logo.ico"));
                //imgProductImage.ImageSource = new BitmapImage(new Uri(Environment.CurrentDirectory + @"/logo.ico"));
                Helpers.log($"Errorl displaying image {imgSrc}, Error: {e.Message}");
            }
            //imgProductImage.Source = new BitmapImage(new Uri(imgSrc));

            if (shopProduct.productUnit == null || string.IsNullOrEmpty(shopProduct.productUnit.barcode))
            {
                btnPrintSticker.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnPrintSticker.Visibility = Visibility.Visible;
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if ("shopProduct".Equals(e.Property.ToString()))
            {
                updateUI();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(shopProduct.stock_count < 1) {
                Helpers.playErrorSound();
                //MessageBox.Show("Out of stock");
                Toast.showError($"Out of stock \nProduct: {shopProduct.name} ");
                return;
            }

            if(shopProduct.sell_price < 1) {
                Helpers.playErrorSound();
                Toast.showError($"Please, update price for {shopProduct.name}");
                //MessageBox.Show("Please, update product price");
                return;
            }

            MyEventBus.post(new EventMessage(EventMessage.EVENT_ADD_TO_CART, shopProduct));
        }

        private void btnEditShopProduct_Click(object sender, RoutedEventArgs e)
        {
            new EditShopProductDialog(shopProduct).ShowDialog();

            updateUI();
        }

        private void btnMenu_Click(object sender, RoutedEventArgs e)
        {
            menuContainer.Visibility = menuContainer.Visibility == Visibility.Visible
                ? Visibility.Collapsed : Visibility.Visible;
        }

        private void btnPrintSticker_Click(object sender, RoutedEventArgs e)
        {
            if(shopProduct.productUnit == null || string.IsNullOrEmpty(shopProduct.productUnit.barcode))
            {
                Toast.showError("This product does not have a barcode");

                return;
            }

            PrintDialog p = new PrintDialog();

            if (p.ShowDialog() == true)
            {
                p.PrintVisual(new ProductBarcodeList(shopProduct), $"Product Barcodes");
            }
        }

    }
}

