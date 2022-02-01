using ShopUrban.Model;
using ShopUrban.Util;
using ShopUrban.View.UserControls;
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
using System.Windows.Shapes;

namespace ShopUrban.View.Dialogs
{
    /// <summary>
    /// Interaction logic for ProductSyncDialog.xaml
    /// </summary>
    public partial class ProductSyncDialog : Window
    {
        public ProductSyncDialog(MainWindow mainWindow)
        {
            InitializeComponent();

            baseContainer.Children.Add(new ProductSyncCtrl(mainWindow));

            MyEventBus.subscribe(handleEvent);
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
                case EventMessage.EVENT_PRODUCT_SYNC_COMPLETED:
                    DialogResult = true;
                    break;
                default:
                    break;
            }
        }

    }
}
