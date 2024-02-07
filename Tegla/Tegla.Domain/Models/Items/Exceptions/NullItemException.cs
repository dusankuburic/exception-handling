namespace Tegla.Domain.Models.Items.Exceptions;

public class NullItemException : Exception
{
    public NullItemException()
        : base("Item is null.")
    { }
}
