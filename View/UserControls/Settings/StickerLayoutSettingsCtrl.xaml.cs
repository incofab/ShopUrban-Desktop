using ShopUrban.Model;
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
    /// Interaction logic for StickerLayoutSettingsCtrl.xaml
    /// </summary>
    public partial class StickerLayoutSettingsCtrl : UserControl
    {
        //private const string LAYOUT_LANDSCAPE = "Landscape";
        //private const string LAYOUT_PORTRAIT = "Portrait";

        public StickerLayoutSettingsCtrl()
        {
            InitializeComponent();

            checkLayout();
        }
        
        private void checkLayout()
        {
            var layout = Setting.getValue(Setting.KEY_STICKER_LAYOUT);

            if (layout == null) layout = Setting.STICKER_PRINT_LAYOUT_LANDSCAPE;

            if (Setting.STICKER_PRINT_LAYOUT_LANDSCAPE.Equals(layout)) rdLandScape.IsChecked = true;
            else if (Setting.STICKER_PRINT_LAYOUT_PORTRAIT.Equals(layout)) rdPortrait.IsChecked = true;
            else rdLandScape.IsChecked = true;
        }

        private void saveLayout(string layout)
        {
            Setting.createOrUpdate(new Setting
            {
                key = Setting.KEY_STICKER_LAYOUT,
                value = layout,
                type = "string",
            });

            checkLayout();
        }

        private void rdPortrait_Click(object sender, RoutedEventArgs e)
        {
            saveLayout(Setting.STICKER_PRINT_LAYOUT_PORTRAIT);
        }

        private void rdLandScape_Click(object sender, RoutedEventArgs e)
        {
            saveLayout(Setting.STICKER_PRINT_LAYOUT_LANDSCAPE);
        }
    }
}
