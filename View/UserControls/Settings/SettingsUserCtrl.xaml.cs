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
    /// Interaction logic for SettingsUserCtrl.xaml
    /// </summary>
    public partial class SettingsUserCtrl : UserControl
    {
        public SettingsUserCtrl()
        {
            InitializeComponent();

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.Background = Brushes.Transparent;
            }
        }

        public void refresh()
        {
            bPaperLayoutSettingsCtrl.Child = new PaperLayoutSettingsCtrl();
            //bVATSettingUserCtrl.Child = new VATSettingUserCtrl();
            bStickerLayoutSettingsCtrl.Child = new StickerLayoutSettingsCtrl();
            //bReceiptBottomNote.Child = new ReceiptBottomNote();
        }

    }
}
