namespace LoGiC.NET.v2;

public static class Terminal
{
    public static void Initialize()
    {
        Console.ResetColor();
        Console.Clear();
    }

    public static void Info(string text)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write('[');
        Console.Write(text);
        Console.Write(']');
        Console.WriteLine();
        Console.ResetColor();
    }

    public static void Warn(string text)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write('[');
        Console.Write(text);
        Console.Write(']');
        Console.WriteLine();
        Console.ResetColor();
    }

    public static void Error(string text)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write('[');
        Console.Write(text);
        Console.Write(']');
        Console.WriteLine();
        Console.ResetColor();
        Environment.Exit(1);
    }
}