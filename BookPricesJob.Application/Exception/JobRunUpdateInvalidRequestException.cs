namespace BookPricesJob.Application.Exception;

public class JobCreateInvalidRequestException(string message) : System.Exception(message);
public class JobRunUpdateInvalidRequestException(string message) : System.Exception(message);
