namespace BookPricesJob.API.Extension;

public static class StringExtensions
{
    public static T? SafelyConvertToEnum<T>(this string value) where T : struct
    {
        return Enum.TryParse<T>(value, ignoreCase: true, out var result) ? result : null;
    }
}