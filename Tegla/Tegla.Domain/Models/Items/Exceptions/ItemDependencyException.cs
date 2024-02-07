
namespace Tegla.Domain.Models.Items.Exceptions;

public class ItemDependencyException : Exception
{
	public ItemDependencyException(Exception innerException)
		: base("Service dependency error occurred, contact support", innerException)
	{}
}
