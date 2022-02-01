
using System;
using System.Reflection;

namespace ShopUrban.Util
{
    class Dummy
    {
        public static void dummy()
        {
            object car = new { wheel = "red", size = 20, weight = "50kg" };

            foreach (PropertyInfo prop in car.GetType().GetProperties())
            {
                var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                Console.WriteLine("******* Start ********");
                Console.WriteLine("Type = "+type);
                Console.WriteLine("Value = "+prop.GetValue(car, null).ToString());
                Console.WriteLine("Name = "+prop.Name);

            }
        }









    }
}
