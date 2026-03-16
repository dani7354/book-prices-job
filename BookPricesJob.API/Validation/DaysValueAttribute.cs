using System.ComponentModel.DataAnnotations;

namespace BookPricesJob.API.Validation;

public class DaysValueAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is int intValue)
            return intValue > 0;

        return false;
    }
}