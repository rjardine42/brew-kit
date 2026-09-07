using Spectre.Console;

public static class AnsiConsoleExtensions
{
    extension(AnsiConsole)
    {
        public static void WriteWarning(string message, bool newline = true)
        {
            AnsiConsole.MarkupLine($"[yellow]{message}[/]{(newline ? "\n" : "")}");
        }

        public static void WriteError(string message, bool newline = true)
        {
            AnsiConsole.MarkupLine($"[red]{message}[/]{(newline ? "\n" : "")}");
        }

        public static void WriteSuccess(string message, bool newline = true)
        {
            AnsiConsole.MarkupLine($"[green]{message}[/]{(newline ? "\n" : "")}");
        }
    }
}
