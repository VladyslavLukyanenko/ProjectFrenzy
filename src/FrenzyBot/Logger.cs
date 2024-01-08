using FrenzyBot.Structures.User;
using System;

namespace FrenzyBot
{
    public static class Logger
    {
        public static void PrintLogo()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Red;
            centerMessage("                                                            ");
            centerMessage(" ██████╗ ██████╗  ██████╗      ██╗███████╗ ██████╗████████╗ ");
            centerMessage(" ██╔══██╗██╔══██╗██╔═══██╗     ██║██╔════╝██╔════╝╚══██╔══╝ ");
            centerMessage(" ██████╔╝██████╔╝██║   ██║     ██║█████╗  ██║        ██║    ");
            centerMessage(" ██╔═══╝ ██╔══██╗██║   ██║██   ██║██╔══╝  ██║        ██║    ");
            centerMessage(" ██║     ██║  ██║╚██████╔╝╚█████╔╝███████╗╚██████╗   ██║    ");
            centerMessage(" ╚═╝     ╚═╝  ╚═╝ ╚═════╝  ╚════╝ ╚══════╝ ╚═════╝   ╚═╝    ");
            centerMessage("                                                            ");
            centerMessage("    ███████╗██████╗ ███████╗███╗   ██╗███████╗██╗   ██╗     ");
            centerMessage("    ██╔════╝██╔══██╗██╔════╝████╗  ██║╚══███╔╝╚██╗ ██╔╝     ");
            centerMessage("    █████╗  ██████╔╝█████╗  ██╔██╗ ██║  ███╔╝  ╚████╔╝      ");
            centerMessage("    ██╔══╝  ██╔══██╗██╔══╝  ██║╚██╗██║ ███╔╝    ╚██╔╝       ");
            centerMessage("    ██║     ██║  ██║███████╗██║ ╚████║███████╗   ██║        ");
            centerMessage("    ╚═╝     ╚═╝  ╚═╝╚══════╝╚═╝  ╚═══╝╚══════╝   ╚═╝        ");
            centerMessage($"v{Versions.program_version}                                                     ");
            Console.ResetColor();
        }

        public static void Log(string Message, ConsoleColor Color)
        {
            Console.ForegroundColor = Color;
            centerMessage($"{DateTime.Now} - {Message}");
            Console.ResetColor();
        }

        public static void Print(string Message, ConsoleColor Color)
        {
            Console.ForegroundColor = Color;
            Console.Write(Message);
            Console.ResetColor();
        }

        public static void printAndExit(string Message, ConsoleColor Color)
        {
            Console.Clear();
            PrintLogo();
            Console.ForegroundColor = Color;
            Console.Write(Message + " (Press enter to quit)");
            Console.ResetColor();
            Console.Read();
            Environment.Exit(0);
        }

        public static void PrintLine(string Message, ConsoleColor Color)
        {
            Console.ForegroundColor = Color;
            centerMessage(Message);
            Console.ResetColor();
        }
        private static void centerMessage(string Message)
        {
            try
            {
                Console.SetCursorPosition((Console.WindowWidth - Message.Length) / 2, Console.CursorTop);
            }
            catch { }
            Console.WriteLine(Message);

        }
    }
}