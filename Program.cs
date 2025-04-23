using System;
using System.Threading;
using System.Media;

namespace CyberSecurityBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Cybersecurity Awareness Bot";
            PlayGreeting();
            DisplayWelcome();

            Console.Write("\nWhat is your name? ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            string name = Console.ReadLine();
            Console.ResetColor();

            Console.WriteLine();
            PrintWithTypingEffect($"Hello {name}, I'm your friendly cybersecurity assistant!\nAsk me anything about staying safe online, or type 'exit' to quit.\n");

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\nYou: ");
                Console.ResetColor();
                string question = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(question))
                {
                    Console.WriteLine("⚠️ Please ask a question.");
                    continue;
                }

                if (question.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("\n👋 Goodbye! Stay cyber safe out there.");
                    break;
                }

                string answer = KnowledgeBase.GetAnswer(question);
                PrintWithTypingEffect(answer);
            }
        }

        static void PlayGreeting()
        {
            try
            {
                using SoundPlayer player = new SoundPlayer("greeting.wav");
                player.PlaySync();
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("⚠️ Could not play greeting audio.");
                Console.ResetColor();
            }
        }

        static void PrintWithTypingEffect(string message, int delay = 25)
        {
            foreach (char c in message)
            {
                Console.Write(c);
                Thread.Sleep(delay);
            }
            Console.WriteLine();
        }

        static void DisplayWelcome()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            try
            {
                string banner = System.IO.File.ReadAllText("ascii_logo.txt");
                Console.WriteLine(banner);
            }
            catch
            {
                Console.WriteLine("=== Welcome to CyberSecurityBot ===");
            }
            Console.ResetColor();
        }
    }
}

