using System;
using System.Threading;
using System.Media;
using CyberSecurityBot;

namespace CyberSecurityBot
{
    class Program
    {
        static void Main(string[] args)
        {
            DatabaseSetup.Initialize();
            Console.Title = "Cybersecurity Awareness Bot";

            string greetingText = "Hello! Welcome to the Cybersecurity Awareness Bot. Let's help you stay safe online.";
            PlayGreetingWithText(greetingText);

            DisplayWelcome();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("\nWhat is your name? ");
            Console.ResetColor();
            string name = Console.ReadLine() ?? "Guest";

            Console.WriteLine();
            PrintWithTypingEffect($"Hello {name}, I'm your friendly cybersecurity assistant!\nAsk me anything about staying safe online, or type 'exit' to quit.\n");

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\nYou: ");
                Console.ResetColor();
                string question = (Console.ReadLine() ?? string.Empty).Trim();

                if (string.IsNullOrEmpty(question))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("⚠️ Please ask a question.");
                    Console.ResetColor();
                    continue;
                }

                if (question.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n👋 Goodbye! Stay cyber safe out there.");
                    Console.ResetColor();
                    break;
                }

                string answer = KnowledgeBase.GetAnswer(question);
                PrintWithTypingEffect(answer);
            }
        }

        static void PlayGreetingWithText(string message)
        {
            Thread audioThread = new(() =>
            {
                try
                {
                    if (OperatingSystem.IsWindows())
                    {
                        using SoundPlayer player = new SoundPlayer("greeting.wav");
                        player.PlaySync();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("⚠️ Audio playback is only supported on Windows.");
                        Console.ResetColor();
                    }
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("⚠️ Could not play greeting audio.");
                    Console.ResetColor();
                }
            });

            audioThread.Start();
            PrintWithTypingEffect(message, 35);
            audioThread.Join();
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

