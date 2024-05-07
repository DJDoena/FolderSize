using System.Globalization;

namespace DoenaSoft.FolderSize.Model;

internal static class FileSizeHelper
{
    public static string FormatFileSize(ulong size)
    {
        string unit;
        if (CheckOrderOfMagnitude(size, 40, 1, out var rounded))
        {
            unit = "TiB";
        }
        else if (CheckOrderOfMagnitude(size, 30, 1, out rounded))
        {
            unit = "GiB";
        }
        else if (CheckOrderOfMagnitude(size, 20, 0, out rounded))
        {
            unit = "MiB";
        }
        else if (CheckOrderOfMagnitude(size, 10, 0, out rounded))
        {
            unit = "KiB";
        }
        else
        {
            rounded = size;

            unit = "Byte";
        }

        var formatted = $"{rounded.ToString(CultureInfo.CurrentCulture)} {unit}";

        return formatted;
    }

    private static bool CheckOrderOfMagnitude(ulong size, double exponent, int decimals, out decimal rounded)
    {
        var pow = (decimal)Math.Pow(2, exponent);

        var quotient = size / pow;

        rounded = Math.Round(quotient, decimals, MidpointRounding.AwayFromZero);

        return rounded >= 1;
    }
}