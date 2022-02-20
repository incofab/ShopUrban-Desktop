using MaterialDesignThemes.Wpf;
using ShopUrban.Model;
using ShopUrban.Model.Others;
using ShopUrban.Util;
using ShopUrban.Util.Network;
using ShopUrban.View;
using ShopUrban.View.Dialogs;
using ShopUrban.View.UserControls;
using ShopUrban.View.UserControls.Cart;
using ShopUrban.View.UserControls.Draft;
using ShopUrban.View.UserControls.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ShopUrban
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ProductListCtrl productListCtrl;
        public static Staff staffDetail;
        public List<SideMenu> menuItems { get; set; }
        private Dictionary<string, UserControl> pages = new Dictionary<string, UserControl>();

        public MainWindow(Staff staff)
        {
            DateTime.Now.ToString(KStrings.TIME_FORMAT);

            staffDetail = staff;

            InitializeComponent();

            productListCtrl = new ProductListCtrl();

            //menuItemFrame.Navigate(productListCtrl);
            //borderProductList.Child = productListCtrl;
            //borderCartSection.Child = new CartSectionCtrl();
            boxEmpty.Children.Add(new ProductSyncCtrl(this));

            handleDisplay();

            menuItems = getMenuItems();
            lbMenuItems.ItemsSource = menuItems;

            lbMenuItems.SelectedIndex = 0;
            //lbMenuItems.SelectedItem = ((List<SideMenu>)lbMenuItems.ItemsSource)[0];
            //lbMenuItems_SelectionChanged(lbMenuItems, null);

            MyEventBus.subscribe(handleEvent);

            AutoSyncOrders.getInstance().start();

            productListCtrl.tbSearch.Focus();

            this.Title = "ShopUrban - v"+Helpers.getVersionNo();
        }

        protected override void OnClosed(EventArgs e)
        {
            MyEventBus.unSubscribe(handleEvent);
            base.OnClosed(e);
        }

        private List<SideMenu> getMenuItems()
        {
            List<SideMenu> l = new List<SideMenu>();

            l.Add(new SideMenu { index = 1, title = "Stock", icon = PackIconKind.CartOutline, view = productListCtrl });
            l.Add(new SideMenu { index = 2, title = "Orders", icon = PackIconKind.BasketOutline, view = null });
            l.Add(new SideMenu { index = 3, title = "Draft", icon = PackIconKind.BriefcaseOutline, view = null });
            l.Add(new SideMenu { index = 4, title = "Settings", icon = PackIconKind.GearOutline, view = new SettingsUserCtrl() });
            //l.Add(new SideMenu { index = 4, title = "Product Sync", icon = PackIconKind.Reload, view = new ProductSyncCtrl(this) });

            return l;
        }

        public void handleDisplay()
        {
            productListCtrl.loadView();

            var productLastSync = Setting.getValue(Setting.KEY_PRODUCT_LAST_SYNC);
            var imagesSynced = Setting.getValue(Setting.KEY_IMAGES_SYNCED);

            if (productLastSync == null || imagesSynced != "true")
            {
                boxEmpty.Visibility = Visibility.Visible;
            }
            else
            {
                boxEmpty.Visibility = Visibility.Collapsed;
            }
            //boxEmpty.Visibility = Visibility.Collapsed;
        }

        private void handleEvent(EventMessage eventMessage)
        {
            switch (eventMessage.eventId)
            {
                case EventMessage.EVENT_LOGOUT:
                    new LoginWindow().Show();
                    this.Close();
                    staffDetail = null;
                    break;
                case EventMessage.EVENT_OPEN_PRODUCT_SYNC:
                    new ProductSyncDialog(this).ShowDialog();
                    break;
                case EventMessage.EVENT_PRODUCT_SYNC_COMPLETED:
                    handleDisplay();
                    break;
                case EventMessage.EVENT_DRAFT_EDIT:
                    lbMenuItems.SelectedItem = ((List<SideMenu>)lbMenuItems.ItemsSource)[0];
                    lbMenuItems_SelectionChanged(lbMenuItems, null);
                    break;
                default:
                    break;
            }
        }

        private SideMenu currentSideMenu = null;

        private void lbMenuItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SideMenu currentSideMenu = (SideMenu)((ListBox)sender).SelectedItem;

            if (this.currentSideMenu != null) this.currentSideMenu.selected = false;

            this.currentSideMenu = currentSideMenu;
            this.currentSideMenu.selected = true;

            borderProductList.Child = getSideMenuView(currentSideMenu);
        }

        private UserControl getSideMenuView(SideMenu sideMenu)
        {
            var view = sideMenu.view;

            switch (sideMenu.title)
            {
                case "Orders":
                    if (view == null) view = new OrderHistory();
                    ((OrderHistory)view).refresh();
                    break;

                case "Draft":
                    if (view == null) view = new DraftUserCtrl();
                    ((DraftUserCtrl)view).refresh();
                    break;
            }

            return view;
        }


    }
}
