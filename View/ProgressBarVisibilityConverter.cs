using System.Globalization;
using System.Windows.Data;

namespace DoenaSoft.FolderSize.View;

public sealed class ProgressBarVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var doubleValue = (double)value;

        switch (doubleValue)
        {
            case double.NaN:
            case < 5:
                {
                    return System.Windows.Visibility.Collapsed;
                }
            default:
                {
                    return System.Windows.Visibility.Visible;
                }
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
