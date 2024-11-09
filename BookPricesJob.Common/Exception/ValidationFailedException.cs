namespace BookPricesJob.Common.Exception
{
    public class ValidationFailedException : System.Exception
    {
        public ValidationFailedException(string? message = null)
            : base($"Validation failed! {message}")
        {
        }
    }
}   
