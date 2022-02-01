using ShopUrban.Model;
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

namespace ShopUrban.View.UserControls.Settings
{
    /// <summary>
    /// Interaction logic for VATSettingUserCtrl.xaml
    /// </summary>
    public partial class VATSettingUserCtrl : UserControl
    {
        float vat = 0;

        public VATSettingUserCtrl()
        {
            InitializeComponent();

            float.TryParse(Setting.getValue(Setting.KEY_VAT_PERCENT), out vat);

            tbVAT.Text = vat + "";
        }

        private void tbVAT_LostFocus(object sender, RoutedEventArgs e)
        {
            float vat = 0;
            try
            {
                vat = float.Parse(tbVAT.Text.ToString());

                Setting.createOrUpdate(new Setting
                {
                    key = Setting.KEY_VAT_PERCENT,
                    display_name = "VAT percent",
                    value = vat+"",
                    type = "Float"
                });

                this.vat = vat;
            }
            catch (Exception)
            {
                MessageBox.Show("Please enter a valid number");
                throw;
            }
        }

    }
}
