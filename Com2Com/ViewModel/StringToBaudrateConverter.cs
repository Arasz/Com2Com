using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Com2Com.ViewModel
{
    class StringToBaudrateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int baudrate;
            int defualtBaudrate = 9600;
            if (int.TryParse(value as string, out baudrate))
            {
                if (baudrate > 0)
                    return baudrate;
            }
            return defualtBaudrate;
        }
    }
}
