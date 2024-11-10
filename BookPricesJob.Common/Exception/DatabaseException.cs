namespace BookPricesJob.Common.Exception;
    public class DatabaseException(System.Exception innerException) :
        System.Exception("Database Error", innerException) {}
