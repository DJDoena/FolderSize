using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DoenaSoft.FolderSize.View;

public sealed class ProgressBarColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var doubleValue = (double)value;

        switch (doubleValue)
        {
            case double.NaN:
                {
                    return Brushes.Gray;
                }
            case < 25:
                {
                    return Brushes.LightBlue;
                }
            case < 50:
                {
                    return Brushes.LightGreen;
                }
            case < 75:
                {
                    return Brushes.Gold;
                }
            default:
                {
                    return Brushes.Firebrick;
                }
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
