using ShopUrban.Model;
using ShopUrban.Services;
using ShopUrban.Util;
using ShopUrban.View;
using ShopUrban.View.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ShopUrban
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        private Staff currentLoggedInStaff;
        const int DELAY = 1;
        public SplashScreen()
        {
            DBCreator.getInstance().init();

            InitializeComponent();

            MyEventBus.subscribe(handleEvent);

            Shop shopDetail = Setting.getShopDetail();
            currentLoggedInStaff = Helpers.getLoggedInStaffFromDB();
            MainWindow.staffDetail = currentLoggedInStaff;

            if (shopDetail != null && currentLoggedInStaff != null)
            {
                MainWindow.shopDetail = shopDetail;

                syncProducts();

                return;
            }

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(DELAY) };

            timer.Start();
            timer.Tick += (sender, args) =>
            {
                timer.Stop();
                gotoNextPage();
            };
        }

        private void gotoNextPage()
        {
            string rememberLoginValue = Setting.getValue(Setting.KEY_REMEMBER_LOGIN);

            if (!"true".Equals(rememberLoginValue) || currentLoggedInStaff == null)
            {
                new LoginWindow().Show();
            }
            else
            {
                new MainWindow(currentLoggedInStaff).Show();
            }

            this.Close();
        }

        private void syncProducts()
        {

            var obj = new SynchronizeShopProduct(
                (List<ShopProduct> shopProducts) => {

                    if (syncFailed)
                    {
                        syncFailed = false;
                        return;
                    }

                    gotoNextPage();
                }
            );

            obj.syncProductsAndImages();

            //ProductSyncHelper.syncProductsAndImages((bool isDownloadingImages) =>{

            //    if (syncFailed)
            //    {
            //        syncFailed = false;
            //        return;
            //    }
            //    gotoNextPage();

            //});
        }

        private bool syncFailed = false;
        private void onProductSyncFailed()
        {
            syncFailed = true;

            string message = "Product sync not successful, connect your internet and try again or " +
                "click continue to move on without synchronization";

            CustomDialog customDialog = new CustomDialog(message, "Try Again", "Continue", "Product sync failed");
            customDialog.ShowDialog();

            if (customDialog.selection.Equals(CustomDialog.SELECTION_POSITIVE))
            {
                //try again
                syncProducts();
            }
            else
            {
                gotoNextPage();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            MyEventBus.unSubscribe(handleEvent);
            base.OnClosed(e);
        }

        private void handleEvent(EventMessage eventMessage)
        {
            switch (eventMessage.eventId)
            {
                case EventMessage.EVENT_PRODUCT_SYNC_FAILED:
                    
                    onProductSyncFailed();

                    break;
            }
        }

    }
}
