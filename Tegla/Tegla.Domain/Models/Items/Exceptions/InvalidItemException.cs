namespace Tegla.Domain.Models.Items.Exceptions;

public class InvalidItemException : Exception
{
    public InvalidItemException(string parameterName, object parameterValue)
        : base($"Invalid Employee, " +
              $"ParameterName : {parameterName}, " +
              $"ParameterValue: {parameterValue}.")
    { }
}
