using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ShopUrban.Util
{
    public static class KStrings
    {
        public readonly static string BASE_FOLDER = Environment.GetFolderPath(
            Environment.SpecialFolder.CommonApplicationData) + $"/{APP_NAME}/";
        // Local 77777779, 1234, 4399595348343
        //Staging: En-En-1104, 0503, +2348032485925 ... Rotate and Shine En-Ti-1228
        //Production: En-En-1014|En-En-0107, 0102, +2348032485925
        private static BrushConverter converter = new BrushConverter();
        //var brush = (Brush)converter.ConvertFromString("#FFFFFF90");
        public const string APP_NAME = "ShopUrban";
        
        public const bool DEV = true;
        public const int VERSION = 1;
        public const int APP_PRICE = 1500;
        public const string SITE_EMAIL = "support@shopurban.co";
        public const string WEBSITE = "https://shopurban.co";
        public const string SMS_PHONE_NO = CUSTOMER_CARE_NUMBER;
        public const string CUSTOMER_CARE_NUMBER = "09034302172";
        public const string FACEBOOK_PAGE = "https://shopurban.co/facebook";
        public const string YOUTUBE_PAGE = "https://shopurban.co/youtube";
        public const string TIME_FORMAT = "yyyy-M-d H:m:s.s";

        public const string NAIRA_SIGN = "₦"; // ((char)8358).ToString();
        
        public const string URL_BASE = (DEV 
            //? "https://testapi.shopurban.co/"
            ? "http://192.168.56.1:8000/"
            : "https://api.shopurban.co/");
        public const string URL_BASE_API = URL_BASE+"api/desktop/";
        public const string URL_LOGIN_API = URL_BASE_API + "shop/user/login";
        public const string URL_GET_PRODUCTS_API = URL_BASE_API + "shop/product/index";
        public const string URL_UPLOAD_ORDERS_API = URL_BASE_API + "shop/order/upload";
        public const string URL_SHOP_PRODUCT_UPDATE = URL_BASE_API + "shop/product/update";
        public const string URL_SHOP_PRODUCT_STOCK_ACTION = URL_BASE_API + "shop/product/stock-action";

        public const string URL_FEEDBACK_API = URL_BASE_API + "feedback/store";
        public const string URL_GET_ANNOUNCEMENT_API = URL_BASE_API + "announcements";

        public const string URL_PIN_ACTIVATE_PAGE = URL_BASE + "offline-app/activate";
        public const string URL_BUY_LICENSE = URL_BASE + "offline-app/buy-license";
        public const string URL_OFFLINE_BASE = URL_BASE + "offline-app/index";

        public static SolidColorBrush SELECTED_OPTION = (SolidColorBrush)converter.ConvertFromString("#ddf7e2");

        public static SolidColorBrush CORRECT_ANSWER_COLOR = (SolidColorBrush)converter.ConvertFromString("#ddf7e2");
        public static SolidColorBrush WRONG_ANSWER_COLOR = (SolidColorBrush)converter.ConvertFromString("#ffeaea");

        //public static SolidColorBrush COLOR_PRIMARY = (SolidColorBrush)converter.ConvertFromString("#009688");
        //public static SolidColorBrush COLOR_PRIMARY_DARK = (SolidColorBrush)converter.ConvertFromString("#09524b");

        //public const string FA_CARET_LEFT = "&#xf0d9;";
        //public const string FA_CARET_RIGHT = "&#xf0da;";
        //public const string FA_PAPER_PLANE = "&#xf1d8;";
        //public const string FA_CALCULATOR = "&#xf1ec;";
    }
}
