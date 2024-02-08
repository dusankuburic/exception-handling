using static System.Console;

namespace App3;

public class InputValidationException : Exception
{
    public InputValidationException(Exception innerException)
        : base("Input is invalid, contact support.", innerException)
    { }
}

public class NumberLessThanZeroException : Exception
{
    public NumberLessThanZeroException()
        : base("Number is less than zero.")
    { }
}

public class NumberHigherThanTenException : Exception
{
    public NumberHigherThanTenException()
        : base("Number is higher than ten.")
    { }
}

public class StringIsNullException : Exception
{
    public StringIsNullException()
        : base("String is null.")
    { }
}


public class Program
{

    static bool IsValid(int number)
    {
        if (number < 0)
            throw new NumberLessThanZeroException();

        if (number > 10)
            throw new NumberHigherThanTenException();

        return true;
    }


    static bool IsValid(string text)
    {
        if (text is null)
            throw new StringIsNullException();

        return true;
    }

    public static void ValidateDataOnCreate(int number, string text)
    {
        IsValid(number);
        IsValid(text);
    }

    public static void ProcessSomeData(int number, string text)
    {
        try
        {
            //validate input
            ValidateDataOnCreate(number, text);

            //process some data
            WriteLine($"{text} {number}");

            //validate response

            //return result
        }
        catch (NumberLessThanZeroException ex)
        {
            throw new InputValidationException(ex);
        }
        catch (NumberHigherThanTenException ex)
        {
            throw new InputValidationException(ex);
        }
        catch (StringIsNullException ex)
        {
            throw new InputValidationException(ex);
        }
        catch
        {
            throw;
        }
    }


    static void Main(string[] args)
    {
        try
        {
            ProcessSomeData(-4, "dusan");
        }
        catch (InputValidationException ex)
        {
            WriteLine(ex);
        }
        catch (Exception ex)
        {
            WriteLine(ex.Message);
        }
    }
}