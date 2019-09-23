using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SpaceAnalyzer.Converters
{
    public class DriveComboBoxConverter : IValueConverter
    {
        public object Convert(object value1, Type targetType, object parameter, CultureInfo culture)
        {
            return value1 != null ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}