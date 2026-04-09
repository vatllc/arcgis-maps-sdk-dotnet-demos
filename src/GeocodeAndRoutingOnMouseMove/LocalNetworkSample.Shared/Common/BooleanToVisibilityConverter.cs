using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
#if WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
#else
using System.Windows.Data;
using System.Windows;
#endif

namespace LocalNetworkSample.Common
{
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
#if WINUI
        public object Convert(object value, Type targetType, object parameter, string language)
#else
       public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#endif
		 {
            if (parameter as string == "reverse")
                return (value is bool && (bool)value) ? Visibility.Collapsed : Visibility.Visible;
            else
                return (value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }
#if WINUI
        public object ConvertBack(object value, Type targetType, object parameter, string language)
#else
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#endif
        {
            return value is Visibility && (Visibility)value == Visibility.Visible;
        }
	}
}
