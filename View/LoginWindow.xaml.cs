using Newtonsoft.Json;
using ShopUrban.Model;
using ShopUrban.Util;
using ShopUrban.Util.Network;
using ShopUrban.Util.Network.Responses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ShopUrban.View
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            Shop shopDetail = Setting.getShopDetail();

            if(shopDetail == null)
            {
                tbShopId.Visibility = Visibility.Visible;
                tbShopIdText.Visibility = Visibility.Visible;
            }
            else
            {
                tbShopId.Visibility = Visibility.Collapsed;
                tbShopIdText.Visibility = Visibility.Collapsed;
            }
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            if (isLoginLoading) return;

            string phone = tbPhone.Text.Trim().Replace(" ", "");
            string password = pbPassword.Password.Trim();
            string shopId = tbShopId.Text.Trim();

            phone = Helpers.formatPhone(phone);

            if (string.IsNullOrEmpty(shopId))
            {
                var shopDetail = Setting.getShopDetail();

                if(shopDetail != null) shopId = shopDetail.shop_id;
            }

            if (string.IsNullOrEmpty(shopId))
            {
                MessageBox.Show("Shop Id is required");
                return;
            }

            setLoginLoading(true);

            localLogin(phone, password, shopId);
        }

        private void localLogin(string phone, string password, string shopId)
        {
            Staff staff = Staff.login(phone, Helpers.md5(password));

            if (staff == null)
            {
                login(phone, password, shopId);
                return;
            }
            
            setLoginLoading(false);

            gotoMainPage(staff);
        }

        private void gotoMainPage(Staff staff)
        {
            DateTime.Now.ToString(KStrings.TIME_FORMAT);

            Setting.createOrUpdate(new Setting
            {
                key = Setting.KEY_REMEMBER_LOGIN,
                display_name = "Remeber Login",
                value = cbRememberLogin.IsChecked == true ? "true" : "",
                type = "boolean"
            });

            Setting.createOrUpdate(new Setting
            {
                key = Setting.KEY_LOGGED_IN_STAFF_ID,
                display_name = "Logged in Staff ID",
                value = staff.id+"",
                type = "integer"
            });

            MainWindow.staffDetail = staff;
            new MainWindow(staff).Show();
            
            Close();
        }

        public async void login(string phone, string password, string shopId)
        {
            List<KeyValuePair<string, string>> l = new List<KeyValuePair<string, string>>();
            
            //l.Add(new KeyValuePair<string, string>("verification_code", pin));
            l.Add(new KeyValuePair<string, string>("password", password));
            l.Add(new KeyValuePair<string, string>("contact_no", phone));
            l.Add(new KeyValuePair<string, string>("shop_id", shopId));
            
            BaseResponse baseResponse = await BaseResponse.callEndpoint(KStrings.URL_LOGIN_API, l);

            if (baseResponse == null || baseResponse.status != "success")
            {
                MessageBox.Show((baseResponse != null)
                    ? baseResponse.message : $"Login(): Error connecting to server");
                setLoginLoading(false);
                return;
            }

            LoginResponse loginResponse = 
                JsonConvert.DeserializeObject<LoginResponse>(baseResponse.data.ToString());

            User user = loginResponse.user;

            var passwordMd5 = Helpers.md5(password);

            var staff = Staff.eqQuery(new { user_id = user.id }) ?? new Staff();

            staff.name = $"{user.firstname} {user.surname}";
            staff.user_id = user.id;
            staff.phone = user.contact_no;
            staff.username = user.contact_no;
            staff.pin = passwordMd5;
            staff.password = passwordMd5;
            staff.token = loginResponse.token;

            if(staff.id > 0)
            {
                Staff.updateByUserId(staff);
            }
            else
            {
                staff = Staff.save(staff);
            }
            
            Setting.createOrUpdate(new Setting
            {
                key = Setting.KEY_SHOP_DETAILS,
                value = JsonConvert.SerializeObject(loginResponse.shop),
                type = "string",
            });
            
            MainWindow.shopDetail = loginResponse.shop;
            MainWindow.staffDetail = staff;

            MessageBox.Show("Login successful "+loginResponse.user.surname);

            setLoginLoading(false);

            gotoMainPage(staff);
        }

        private bool isLoginLoading;
        private void setLoginLoading(bool isLoading)
        {
            isLoginLoading = isLoading;

            btnLogin.Content = isLoading ? "Loading..." : "Login";
        }


    }
}
