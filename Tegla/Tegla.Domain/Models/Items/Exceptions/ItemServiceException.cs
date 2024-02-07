namespace Tegla.Domain.Models.Items.Exceptions;

public class ItemServiceException : Exception
{
    public ItemServiceException(Exception innerException)
        : base("Item service error, contact support.", innerException)
    { }
}
