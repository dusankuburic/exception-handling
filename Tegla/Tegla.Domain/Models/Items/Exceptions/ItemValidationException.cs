namespace Tegla.Domain.Models.Items.Exceptions;

public class ItemValidationException : Exception
{
    public ItemValidationException(Exception innerException)
        : base("Invalid input, contact support.", innerException)
    { }
}

