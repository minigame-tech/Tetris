using Tetris.Core;

try
{
    if (args.Length > 0 && (args[0] == "--test" || args[0] == "-t" || args[0] == "--self-test"))
    {
        int exitCode = TestRunner.RunAllTests();
        Environment.Exit(exitCode);
    }

    Console.WriteLine("========================================");
    Console.WriteLine("             TETRIS NEO                 ");
    Console.WriteLine("    High-Performance Arcade Edition     ");
    Console.WriteLine("========================================");

    using var game = new Game();
    game.Run();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\n[ERROR] Fatal exception in game execution: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
    Console.ResetColor();
}
