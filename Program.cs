using System;
using System.Threading;
using System.Media;
using System.Collections.Generic;
using CyberSecurityBot;

namespace CyberSecurityBot
{
    class Program
    {
        static void Main(string[] args)
        {
            DatabaseSetup.Initialize();
            Console.Title = "Cybersecurity Awareness Bot";

            string greetingText = "Hello! Welcome to the Cybersecurity Awareness Bot! I am here to help you stay informed, and safe online.";
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
                string input = (Console.ReadLine() ?? string.Empty).Trim();

                if (string.IsNullOrEmpty(input))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("⚠️ Please ask a question.");
                    Console.ResetColor();
                    continue;
                }

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n👋 Goodbye! Stay cyber safe out there.");
                    Console.ResetColor();
                    break;
                }

                if (input.Equals("/list", StringComparison.OrdinalIgnoreCase))
                {
                    PrintDivider("🧠 I can help with these topics:");
                    var topics = KnowledgeBase.GetAllQuestions();
                    foreach (var topic in topics)
                    {
                        Console.WriteLine($"• {topic}");
                    }
                    PrintDivider();
                    continue;
                }

                if (input.Equals("/add", StringComparison.OrdinalIgnoreCase))
                {
                    PrintDivider("📝 Let's add a new topic!");

                    Console.Write("Enter your question keyword: ");
                    string newQuestion = Console.ReadLine()?.Trim() ?? "";

                    Console.Write("Enter the answer: ");
                    string newAnswer = Console.ReadLine()?.Trim() ?? "";

                    bool success = KnowledgeBase.InsertEntry(newQuestion, newAnswer);
                    Console.ForegroundColor = success ? ConsoleColor.Green : ConsoleColor.Red;
                    Console.WriteLine(success ? "✅ Added successfully!" : "❌ Failed to add the entry.");
                    Console.ResetColor();
                    PrintDivider();
                    continue;
                }

                string answer = KnowledgeBase.GetAnswer(input);
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

        static void PrintDivider(string? title = null)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n" + new string('═', 50));
            if (!string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine($"  {title}");
                Console.WriteLine(new string('═', 50));
            }
            Console.ResetColor();
        }
    }
}
