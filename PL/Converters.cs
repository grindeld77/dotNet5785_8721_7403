using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PL
{
    public class Converters : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is Visibility visibility && visibility == Visibility.Visible);
        }
    }
    public class CallStatusToIsActiveConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BO.CallStatus status)
            {
                return (status == BO.CallStatus.Open || status == BO.CallStatus.OpenAtRisk || status == BO.CallStatus.InProgressAtRisk, status == BO.CallStatus.InProgress);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

