namespace BookPricesJob.Common.Exception;

public class NotFoundException(string id) :
    System.Exception($"Resource with id {id} was not found")
{
}
