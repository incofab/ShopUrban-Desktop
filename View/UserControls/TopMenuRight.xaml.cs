using ShopUrban.Model;
using ShopUrban.Services;
using ShopUrban.Util;
using ShopUrban.Util.Network;
using Squirrel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        private void uploadOrders()
        {
            if (iconUploadOrder.Kind == MaterialDesignThemes.Wpf.PackIconKind.CloudUpload) return;

            iconUploadOrder.Kind = MaterialDesignThemes.Wpf.PackIconKind.CloudUpload;

            OrderHelper.uploadPendingOrders((BaseResponse baseResponse) => {

                if (baseResponse != null) Toast.showSuccess("Orders uploaded successfully");

                iconUploadOrder.Kind = MaterialDesignThemes.Wpf.PackIconKind.CloudUploadOutline;
            },
            true);

            //await ProductDownloader.getInstance().uploadOrders(true);
            //iconUploadOrder.Kind = MaterialDesignThemes.Wpf.PackIconKind.CloudUploadOutline;
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
                var squirrelHelper = SquirrelHelper.getInstance();
                var updateAvailable = await squirrelHelper.checkForUpdate();

                if (!updateAvailable)
                {
                    squirrelHelper.dispose();
                    MessageBox.Show("Product is upto date");
                    return;
                }

                MessageBoxResult messageBoxResult = MessageBox.Show(
                    "A new version of the software is available. Click Ok to update now.\nIt will only take a few minutes",
                    "New Update Available", MessageBoxButton.OKCancel);

                if (messageBoxResult != MessageBoxResult.OK) return;

                ReleaseEntry releaseEntry = await squirrelHelper.update();

                if(releaseEntry != null)
                {
                    MessageBox.Show("Update complete, You can relaunch your application now to use the new update");
                }

                squirrelHelper.dispose();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error checking for update: "+e.Message);
            }
        }

        private void syncProducts()
        {
            iconSyncProducts.Kind = MaterialDesignThemes.Wpf.PackIconKind.Cloud;

            var obj = new SynchronizeShopProduct(
                (List<ShopProduct> shopProducts) => {

                    bool isDownloadingImages = shopProducts != null && shopProducts.Count > 0;

                    if (isDownloadingImages) 
                        Toast.showSuccess("Product Sync complete, Images will continue in background");
                    else 
                        Toast.showWarning("Products of this shop are upto date");

                    iconSyncProducts.Kind = MaterialDesignThemes.Wpf.PackIconKind.CloudRefresh;
                }
            );

            obj.syncProductsAndImages();

            DashboardSyncHelper.syncDashboard();
        }
    }
}
