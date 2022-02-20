using ShopUrban.Model;
using ShopUrban.Services;
using ShopUrban.Util;
using ShopUrban.View.Dialogs;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for TopMenu.xaml
    /// </summary>
    public partial class TopMenu : UserControl
    {
        public TopMenu()
        {
            InitializeComponent();
        }

        private void logoutClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Logout now, are you sure?", "Confirm Logout", MessageBoxButton.YesNo);

            if (messageBoxResult != MessageBoxResult.Yes) return;

            LoginHelper.logout();
        }

        private void WebsiteClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Visit the website");
        }

        private void OrdersClick(object sender, RoutedEventArgs e)
        {
            //new OrderHistory().ShowDialog();
        }

        private void DraftsClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Show list of drafts");
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ContactUsClick(object sender, RoutedEventArgs e)
        {
            Helpers.openUrl(KStrings.WEBSITE);
        }

        private void FacebookClick(object sender, RoutedEventArgs e)
        {
            Helpers.openUrl(KStrings.FACEBOOK_PAGE);
        }
        private void YoutubeChannelClick(object sender, RoutedEventArgs e)
        {
            Helpers.openUrl(KStrings.YOUTUBE_PAGE);
        }
        private void UploadOrdersClick(object sender, RoutedEventArgs e)
        {
            new SyncOrdersDialog().ShowDialog();
        }
        private void SyncProductsClick(object sender, RoutedEventArgs e)
        {
            MyEventBus.post(new EventMessage(EventMessage.EVENT_OPEN_PRODUCT_SYNC, null));
        }
        private void checkUpdateClick(object sender, RoutedEventArgs e)
        {
            checkUpdate();
        }

        private async void checkUpdate()
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

    }
}
