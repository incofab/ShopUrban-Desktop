using Newtonsoft.Json.Linq;
using ShopUrban.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ShopUrban.Converters
{
    public class NairaConverter : IValueConverter
    {
        public NairaConverter()
        {

        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Helpers.log("NairaConverter Value = " + value);
            decimal.TryParse(value.ToString(), out decimal amount);

            return KStrings.NAIRA_SIGN + Helpers.numberFormat(amount);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

    }
}
