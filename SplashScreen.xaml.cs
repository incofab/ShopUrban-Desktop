using ShopUrban.Model;
using ShopUrban.Util;
using ShopUrban.View;
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
using System.Windows.Threading;

namespace ShopUrban
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        const int DELAY = 1;
        public SplashScreen()
        {
            DBCreator.getInstance().init();

            InitializeComponent();

            //Setting.execute($"DELETE FROM settings WHERE key = @key", new { key = Setting.KEY_LOGGED_IN_STAFF_ID });
            //Setting.execute($"DELETE FROM settings WHERE key = @key", new { key = Setting.KEY_REMEMBER_LOGIN });

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
            List<Staff> allStaff = Staff.all();
            Setting rememberLogin = Setting.get(Setting.KEY_REMEMBER_LOGIN);

            if(allStaff == null || allStaff.Count < 1)
            {
                new LoginWindow().Show();
            }
            else if(allStaff.Count == 1 && rememberLogin != null && "true".Equals(rememberLogin.value))
            {
                new MainWindow(allStaff[0]).Show();
            }
            else
            {
                new LoginWindow().Show();
            }

            this.Close();
        }


    }
}
