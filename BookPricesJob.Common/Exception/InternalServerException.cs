namespace BookPricesJob.Common.Exception;
public class InternalServerException(System.Exception innerException) :
    System.Exception("Internal Server Error", innerException) {}
