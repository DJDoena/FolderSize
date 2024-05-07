namespace DoenaSoft.FolderSize.Model;

internal static class ULongExtensions
{
    internal static ulong Sum(this IEnumerable<ulong> source)
    {
        if (source == null)
        {
            return 0UL;
        }

        var sum = 0UL;

        foreach (var value in source)
        {
            sum += value;
        }

        return sum;
    }
}