using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MyApps.Utilities
{
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is true)
            {
                return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility.Collapsed)
            {
                return true;
            }

            return false;
        }
    }
}
