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
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null || (value is IEnumerable<object> collection && !collection.Cast<object>().Any())
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    public class SimulatorButtonContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isRunning)
            {
                return isRunning ? "Stop Simulator" : "Start Simulator";
            }
            return "Start Simulator"; // Default text
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RemainingTimeToProgressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan remainingTime)
            {
                TimeSpan totalTime = TimeSpan.FromDays(30); // משך זמן מלא לדוגמה
                double progress = (1 - (remainingTime.TotalSeconds / totalTime.TotalSeconds)) * 100;
                return Math.Max(0, Math.Min(100, progress)); // החזרת ערך תקין בין 0 ל-100
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
