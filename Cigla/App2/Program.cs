using static System.Console;

namespace App2;

public class Program
{
    public class InvalidPixelColorException : Exception
    {
        public InvalidPixelColorException(string color)
            : base($"Invalid color {color}")
        { }
    }

    public class UnknownPixelColorException : Exception
    {
        public UnknownPixelColorException(string color)
            : base($"Unknown color {color}")
        { }
    }

    public static void RenderPixel()
    {
        try
        {
            throw new UnknownPixelColorException("yellow");
            throw new InvalidPixelColorException("blue");
        }
        catch (InvalidPixelColorException ex) when (ex.Message.Contains("red"))
        {
            WriteLine($"\nLOG ERROR RED\n{ex.Message}");
        }
        catch (InvalidPixelColorException ex) when (ex.Message.Contains("green"))
        {
            WriteLine($"\nLOG ERROR GREEN\n{ex.Message}");
        }
        catch (InvalidPixelColorException ex) when (ex.Message.Contains("blue"))
        {
            WriteLine($"\nLOG ERROR BLUE\n{ex.Message}");
        }
        catch (InvalidPixelColorException ex) when (ex.Message.Contains("yellow"))
        {
            throw new UnknownPixelColorException("yellow");
        }
    }

    static void Main(string[] args)
    {
        try
        {
            RenderPixel();
        }
        catch (UnknownPixelColorException e)
        {
            WriteLine(e.Message);
        }
        catch (InvalidPixelColorException e)
        {
            WriteLine(e.Message);

        }
        catch (Exception e)
        {
            WriteLine(e.Message);
        }
    }
}