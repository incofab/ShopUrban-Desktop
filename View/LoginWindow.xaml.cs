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
            setLoginLoading(true);

            string phone = tbPhone.Text.Trim().Replace(" ", "");
            string pin = pbPassword.Password.Trim();
            string shopId = tbShopId.Text.Trim();

            localLogin(phone, pin, shopId);
        }

        private void localLogin(string phone, string pin, string shopId)
        {
            Staff staff = Staff.login(phone, Helpers.md5(pin));

            if (staff == null)
            {
                login(phone, pin, shopId);
                return;
            }
            
            setLoginLoading(false);

            gotoMainPage(staff);
        }

        private void gotoMainPage(Staff staff)
        {
            DateTime.Now.ToString(KStrings.TIME_FORMAT);

            if (cbRememberLogin.IsChecked == true)
            {
                Setting.createOrUpdate(new Setting
                {
                    key = Setting.KEY_REMEMBER_LOGIN,
                    display_name = "Remeber Login",
                    value = "true",
                    type = "boolean"
                });
            }

            Setting.createOrUpdate(new Setting
            {
                key = Setting.KEY_LOGGED_IN_STAFF_ID,
                display_name = "Logged in Staff ID",
                value = staff.id+"",
                type = "integer"
            });

            new MainWindow(staff).Show();
            
            Close();
        }

        public async void login(string phone, string pin, string shopId)
        {
            List<KeyValuePair<string, string>> l = new List<KeyValuePair<string, string>>();
            
            l.Add(new KeyValuePair<string, string>("verification_code", pin));
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

            var pinMd5 = Helpers.md5(pin);

            var staff = new Staff()
            {
                name = $"{user.firstname} {user.surname}",
                user_id = user.id,
                phone = user.contact_no,
                username = user.contact_no,
                pin = pinMd5,
                password = pinMd5,
                token = loginResponse.token,
            };

            if(Staff.eqQuery(new { user_id = staff.user_id }) != null)
            {
                Staff.updateByUserId(staff);
            }
            else
            {
                Staff.save(staff);
            }
            
            Setting.create(new 
            {
                key = Setting.KEY_SHOP_DETAILS,
                value = JsonConvert.SerializeObject(loginResponse.shop),
                type = "string",
            });

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
