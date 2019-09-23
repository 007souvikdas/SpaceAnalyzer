using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SpaceAnalyzer.Converters
{
    public class PageVisibilityConverter : IValueConverter
    {
        public object Convert(object value1, Type targetType, object parameter, CultureInfo culture)
        {
            if (value1 != null)
            {
                bool visibilityValue = (Boolean)value1;
                return visibilityValue ? Visibility.Visible : Visibility.Hidden;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
           return null;
        }
    }
}