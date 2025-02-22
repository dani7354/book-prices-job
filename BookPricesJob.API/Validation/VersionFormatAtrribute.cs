using System.ComponentModel.DataAnnotations;

namespace BookPricesJob.API.Validation;

public class VersionFormatAttribute : ValidationAttribute
{
    private const int GuidLength = 36;
    
    public override bool IsValid(object? value)
    {
        var stringValue = value as string;
        if (!string.IsNullOrEmpty(stringValue))
        {
            if (stringValue.Length != GuidLength)
                return false;
            try
            {
                _ = Guid.Parse(stringValue);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        return true;    
    }
}