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
    public class DateTimeConverter : IValueConverter
    {
        public DateTimeConverter()
        {

        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (string.IsNullOrEmpty(value.ToString())) return "";

                return DateTime.Parse(value.ToString()).ToString(KStrings.TIME_FORMAT);
            }
            catch (Exception)
            {
                return value + "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

    }
}
