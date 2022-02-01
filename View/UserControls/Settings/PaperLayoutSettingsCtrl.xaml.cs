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
    /// Interaction logic for PaperLayoutSettingsCtrl.xaml
    /// </summary>
    public partial class PaperLayoutSettingsCtrl : UserControl
    {
        public PaperLayoutSettingsCtrl()
        {
            InitializeComponent();

            checkReceiptLayout();
        }

        private void checkReceiptLayout()
        {
            var layout = Setting.getValue(Setting.KEY_RECEIPT_LAYOUT);

            if (layout == null) layout = Setting.RECEIPT_LAYOUT_88MM;

            cb58mm.IsChecked = false;
            cb88mm.IsChecked = false;
            cbA4.IsChecked = false;

            if (Setting.RECEIPT_LAYOUT_58MM.Equals(layout)) cb58mm.IsChecked = true;
            if (Setting.RECEIPT_LAYOUT_88MM.Equals(layout)) cb88mm.IsChecked = true;
            if (Setting.RECEIPT_LAYOUT_A4.Equals(layout)) cbA4.IsChecked = true;
        }

        private void saveLayout(string layout)
        {
            Setting.createOrUpdate(new Setting
            {
                key = Setting.KEY_RECEIPT_LAYOUT,
                value = layout,
                type = "string",
            });

            checkReceiptLayout();
        }

        private void cbA4_Click(object sender, RoutedEventArgs e)
        {
            saveLayout(Setting.RECEIPT_LAYOUT_A4);
        }

        private void cb88mm_Click(object sender, RoutedEventArgs e)
        {
            saveLayout(Setting.RECEIPT_LAYOUT_88MM);
        }

        private void cb58mm_Click(object sender, RoutedEventArgs e)
        {
            saveLayout(Setting.RECEIPT_LAYOUT_58MM);
        }

    }
}
