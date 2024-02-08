using static System.Console;

namespace App1;

public class Program
{
    static void DoSomethingA()
    {
        try
        {
            DoSomethingB();
            throw new Exception("A procedure failed, contact support");
        }
        catch (Exception ex)
        {
            WriteLine($"Log Exception: {ex}");
            throw;
        }
    }

    static void DoSomethingB()
    {
        try
        {
            DoSomethingC();
            throw new Exception("B procedure failed, contact support");
        }
        catch (Exception ex)
        {
            WriteLine($"Log Exception: {ex}");
            throw;
        }
    }

    static void DoSomethingC()
    {
        try
        {
            throw new Exception("C procedure failed, contact support");
        }
        catch (Exception ex)
        {
            WriteLine($"Log Exception: {ex}");
            throw;
        }
    }

    static void Main(string[] args)
    {
        try
        {
            DoSomethingA();
        }
        catch (Exception ex)
        {
            WriteLine($"MESSAGE: {ex.Message}");
        }
    }
}