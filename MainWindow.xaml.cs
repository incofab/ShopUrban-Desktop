using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using ShopUrban.Model;
using ShopUrban.Model.Others;
using ShopUrban.Services;
using ShopUrban.Util;
using ShopUrban.Util.Network;
using ShopUrban.Util.Network.Responses;
using ShopUrban.View;
using ShopUrban.View.Dialogs;
using ShopUrban.View.UserControls;
using ShopUrban.View.UserControls.Draft;
using ShopUrban.View.UserControls.Settings;
using ShopUrban.View.UserControls.ToastCtrl;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ShopUrban
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Staff staffDetail;
        public static Shop shopDetail;
        private ProductListCtrl productListCtrl;
        public List<SideMenu> menuItems { get; set; }
        private Dictionary<string, UserControl> pages = new Dictionary<string, UserControl>();

        public MainWindow(Staff staff)
        {
            DateTime.Now.ToString(KStrings.TIME_FORMAT);

            staffDetail = staff;
            shopDetail = Setting.getShopDetail();

            InitializeComponent();

            productListCtrl = new ProductListCtrl();

            boxEmpty.Children.Add(new ProductSyncCtrl(this));

            handleDisplay();

            menuItems = getMenuItems();
            lbMenuItems.ItemsSource = menuItems;

            lbMenuItems.SelectedIndex = 0;

            AutoSyncOrders.getInstance().start();
            DashboardSyncHelper.syncDashboard();

            productListCtrl.tbSearch.Focus();

            this.Title = "ShopUrban - v"+Helpers.getVersionNo();

            toastContainer.Child = new ToastPageCtrl();

            MyEventBus.subscribe(handleEvent);
        }

        protected override void OnClosed(EventArgs e)
        {
            MyEventBus.unSubscribe(handleEvent);
            base.OnClosed(e);
            DBCreator.getConn().Close();
        }

        private List<SideMenu> getMenuItems()
        {
            List<SideMenu> l = new List<SideMenu>();

            l.Add(new SideMenu { index = 1, title = "Stock", icon = PackIconKind.CartOutline, view = productListCtrl });
            l.Add(new SideMenu { index = 2, title = "Orders", icon = PackIconKind.BasketOutline, view = null });
            l.Add(new SideMenu { index = 3, title = "Draft", icon = PackIconKind.BriefcaseOutline, view = null });
            l.Add(new SideMenu { index = 4, title = "Settings", icon = PackIconKind.GearOutline, view = null });
            //l.Add(new SideMenu { index = 5, title = "Barcodes", icon = PackIconKind.Barcode, view = new ProductBarcodeList() });

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
                case "Stock":
                    TimerHelper.SetTimeout(1000, () => {

                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            if (productListCtrl == null) return;

                            productListCtrl.tbSearch.Focus();

                        }));
                    });
                    break;

                case "Orders":
                    if (view == null) view = new OrderHistory();
                    ((OrderHistory)view).refresh();
                    break;

                case "Draft":
                    if (view == null) view = new DraftUserCtrl();
                    ((DraftUserCtrl)view).refresh();
                    break;

                case "Settings":
                    if (view == null) view = new SettingsUserCtrl();
                    ((SettingsUserCtrl)view).refresh();
                    break;
            }

            return view;
        }

        private async void indexData()
        {
            //    string desktop_index_last_sync = Setting.getValue(Setting.KEY_DESKTOP_INDEX_LAST_SYNC);

            //    List<KeyValuePair<string, string>> l = new List<KeyValuePair<string, string>>();

            //    //l.Add(new KeyValuePair<string, string>("shop_id", shopId));
            //    l.Add(new KeyValuePair<string, string>("desktop_index_last_sync", desktop_index_last_sync));

            //    BaseResponse baseResponse = await BaseResponse.callEndpoint(KStrings.URL_DESKTOP_DASHBOARD_API, l);

            //    if (baseResponse == null || baseResponse.status != "success")
            //    {
            //        Helpers.log((baseResponse != null) ? baseResponse.message : $"Login(): Error connecting to server");
            //        return;
            //    }

            //    DashboardIndexResponse dashboardIndexResponse =
            //        JsonConvert.DeserializeObject<DashboardIndexResponse>(baseResponse.data.ToString());

            //    ShopCustomer.multiCreateOrUpdate(dashboardIndexResponse.shop_customers);
            //    Setting.multiCreateOrUpdate(dashboardIndexResponse.shop_settings);

            //    DateTime.Now.ToString(KStrings.TIME_FORMAT);
            //    desktop_index_last_sync = DateTime.Now.ToString(KStrings.TIME_FORMAT);

            //    Setting.createOrUpdate(new Setting()
            //    {
            //        key = Setting.KEY_DESKTOP_INDEX_LAST_SYNC,
            //        value = desktop_index_last_sync,
            //    });

            //    Helpers.log("Desktop index completed");
        }
    }
}
