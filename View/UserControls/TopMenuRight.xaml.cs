using ShopUrban.Model;
using ShopUrban.Services;
using ShopUrban.Util;
using ShopUrban.Util.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for TopMenuRight.xaml
    /// </summary>
    public partial class TopMenuRight : UserControl
    {
        public TopMenuRight()
        {
            InitializeComponent();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Logout now, are you sure?", "Confirm Logout", MessageBoxButton.YesNo);

            if (messageBoxResult != MessageBoxResult.Yes) return;

            LoginHelper.logout();
        }

        private async void uploadOrders()
        {
            iconUploadOrder.Kind = MaterialDesignThemes.Wpf.PackIconKind.CloudUpload;
            
            await ProductDownloader.getInstance().uploadOrders(true);

            iconUploadOrder.Kind = MaterialDesignThemes.Wpf.PackIconKind.CloudUploadOutline;
        }

        private void btnUploadOrder_Click(object sender, RoutedEventArgs e)
        {
            uploadOrders();
        }

        private void btnSyncProducts_Click(object sender, RoutedEventArgs e)
        {
            syncProducts();
        }

        private void checkUpdate_Click(object sender, RoutedEventArgs e)
        {
            checkUpdate();
        }

        private async void checkUpdate()
        {
            try
            {
                var updateAvailable = await SquirrelHelper.getInstance().checkForUpdate();

                if (!updateAvailable)
                {
                    MessageBox.Show("Product is upto date");
                    return;
                }

                MessageBoxResult messageBoxResult = MessageBox.Show(
                    "A new version of the software is available. Click Ok to update now.\nIt will only take a few minutes",
                    "New Update Available", MessageBoxButton.OKCancel);

                if (messageBoxResult != MessageBoxResult.OK) return;

                SquirrelHelper.getInstance().update();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error checking for update: "+e.Message);
            }
        }

        private async void syncProducts()
        {
            iconSyncProducts.Kind = MaterialDesignThemes.Wpf.PackIconKind.Cloud;
            
            //await ProductSyncHelper.syncProducts();
            List<Product> products = await ProductSyncHelper.syncProducts();

            downloadImages();

            MessageBox.Show("Product Sync complete, Images will continue in background");

            iconSyncProducts.Kind = MaterialDesignThemes.Wpf.PackIconKind.CloudRefresh;

            MyEventBus.post(new EventMessage(EventMessage.EVENT_PRODUCT_SYNC_COMPLETED, null));
        }

        private void downloadImages()
        {
            ProductImagesDownloader productImagesDownloader = new ProductImagesDownloader(null, downloadImages_completed);

            List<ProductUnit> productUnits = ProductUnit.all(
                " Where photo IS NOT NULL AND local_photo IS NULL ");

            if (productUnits == null) return;

            productImagesDownloader.beginDownloadProcess(productUnits);
        }
        private void downloadImages_completed(object sender, RunWorkerCompletedEventArgs e)
        {
            TimerHelper.SetTimeout(1000, () =>
            {
                Application.Current.Dispatcher.Invoke((Action)delegate ()
                {
                    MyEventBus.post(new EventMessage(EventMessage.EVENT_PRODUCT_SYNC_COMPLETED, null));

                }, null);
            });
        }

    }
}
