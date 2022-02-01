using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ShopUrban.Converters
{
    class GenericConverter : IValueConverter
    {
        private Func<object, Type, object, CultureInfo, object> convertMethod;

        public GenericConverter(Func<object, Type, object, CultureInfo, object> convertMethod)
        {
            this.convertMethod = convertMethod;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Console.WriteLine("Parameter = " + parameter.ToString());
            //return value + " ("+getNumOfQuestions("English", value.ToString())+" Questions)";
            return convertMethod(value, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

    }
}
