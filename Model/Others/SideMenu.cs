using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace ShopUrban.Model.Others
{
    public class SideMenu
    {

        public int index { get;  set; }
        public bool selected { get;  set; }
        public PackIconKind icon { get;  set; }
        public string title { get;  set; }
        public UserControl view { get;  set; }


    }

}
