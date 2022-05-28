using ShopUrban.Model;
using ShopUrban.Util;
using ShopUrban.Util.Printing.Paginator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    /// Interaction logic for ProductBarcodeList.xaml
    /// </summary>
    public partial class ProductBarcodeList : UserControl
    {
        //public List<ShopProduct> shopProducts
        //{
        //    get { return (List<ShopProduct>)GetValue(shopProductsProperty); }
        //    set { SetValue(shopProductsProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for shopProducts.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty shopProductsProperty =
        //    DependencyProperty.Register("shopProducts", typeof(List<ShopProduct>), typeof(ProductBarcodeList), new PropertyMetadata(null));

        public ShopProduct shopProduct { get; set; }

        public ProductBarcodeList(ShopProduct shopProduct)
        {
            InitializeComponent();

            this.shopProduct = shopProduct;

            DataContext = this;

            if(shopProduct.productUnit == null || string.IsNullOrEmpty(shopProduct.productUnit.barcode))
            {
                //barcodeIcons.Visibility = Visibility.Collapsed;
                barcodeText.Visibility = Visibility.Collapsed;
            }

            //shopProducts = ShopProduct.all(null, null, -1, " LIMIT 10000 OFFSET 0 ");

            string barcodeImgFIlename = new BarcodeCreator().createBarcode(shopProduct.productUnit.barcode);

            if(!string.IsNullOrEmpty(barcodeImgFIlename) && File.Exists(barcodeImgFIlename))
            {
                barcodeImage.Source = new BitmapImage(new Uri(barcodeImgFIlename));
            }

            setLayoutMode();
        }
        
        private void setLayoutMode()
        {
            var layout = Setting.getValue(Setting.KEY_STICKER_LAYOUT);

            if (Setting.STICKER_PRINT_LAYOUT_PORTRAIT.Equals(layout))
            {
                rotate.Angle = 270;
                rotate.CenterX = 0.5;
                rotate.CenterY = 0.5;
            }
            else
            {
                rotate.Angle = 0;
                rotate.CenterX = 0;
                rotate.CenterY = 0;
            }
        }

        /*
        private void print()
        {
            btnPrint.Visibility = Visibility.Collapsed;

            PrintDialog p = new PrintDialog();

            if (p.ShowDialog() == true)
            {
                p.PrintVisual(lbProducts, $"Product Barcodes");
            }

            btnPrint.Visibility = Visibility.Visible;
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            print();
        }
        */
    }

}
