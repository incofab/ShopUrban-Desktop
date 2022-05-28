using ShopUrban.Model;
using ShopUrban.Util;
using System;
using System.Collections.Generic;
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

namespace ShopUrban.View.UserControls.Settings
{
    /// <summary>
    /// Interaction logic for ReceiptBottomNote.xaml
    /// </summary>
    public partial class ReceiptBottomNote : UserControl
    {
        public ReceiptBottomNote()
        {
            InitializeComponent();

            string note = Setting.getValue(Setting.KEY_RECEIPT_BOTTOM_NOTE);

            tbNote.Text = note;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Setting.createOrUpdate(new Setting
            {
                key = Setting.KEY_RECEIPT_BOTTOM_NOTE,
                display_name = "Receipt Bottom Note",
                value = tbNote.Text.ToString(),
                type = "string"
            });

            Toast.showSuccess("Note saved successfully");
        }

    }
}
