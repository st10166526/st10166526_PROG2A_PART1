using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Threading;
using CyberSecurityBot;

namespace CyberSecurityBot
{
    class Program
    {
        // You can tweak this to slow down or speed up the greeting text
        private const int GREETING_TYPING_DELAY = 55;

        // Conversation memory
        static List<string> conversationHistory = new();
        static string lastTopic = string.Empty;

        static void Main(string[] args)
        {
            DatabaseSetup.Initialize();
            Console.Title = "Cybersecurity Awareness Bot";

            // 1. Audio + Typed Greeting
            string greetingText = "Hello! Welcome to the Cybersecurity Awareness Bot. I'm here to help you stay informed, and safe online.";
            PlayGreetingWithText(greetingText, GREETING_TYPING_DELAY);

            // 2. ASCII Banner
            DisplayWelcome();

            // 3. Name & Personalized Welcome
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("\nWhat is your name? ");
            Console.ResetColor();
            string name = Console.ReadLine()?.Trim() ?? "Guest";

            Console.WriteLine();
            PrintWithTypingEffect(
                $"Hello {name}, I'm your friendly cybersecurity assistant!\n" +
                $"Type a question (e.g. \"How are you?\"), or 'exit' to quit.\n"
            );

            // 4. Main Loop
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\nYou: ");
                Console.ResetColor();
                string input = Console.ReadLine()?.Trim() ?? "";

                if (string.IsNullOrEmpty(input))
                {
                    Warn("Please ask a question.");
                    continue;
                }

                conversationHistory.Add(input);

                // Exit
                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    Success("Goodbye! Stay cyber safe out there.");
                    break;
                }

                // List topics
                if (input.Equals("/list", StringComparison.OrdinalIgnoreCase))
                {
                    PrintDivider("🧠 I can help with these topics:");
                    foreach (var topic in KnowledgeBase.GetAllQuestions())  // Ensure this uses SELECT DISTINCT
                        Console.WriteLine($"• {topic}");
                    PrintDivider();
                    continue;
                }

                // Add new Q&A
                if (input.Equals("/add", StringComparison.OrdinalIgnoreCase))
                {
                    PrintDivider("📝 Add a new topic");
                    Console.Write("Question keyword: ");
                    string q = Console.ReadLine()?.Trim() ?? "";
                    Console.Write("Answer: ");
                    string a = Console.ReadLine()?.Trim() ?? "";

                    bool ok = KnowledgeBase.InsertEntry(q, a);
                    if (ok) Success("Added successfully!");
                    else    Warn("Failed to add entry.");
                    PrintDivider();
                    continue;
                }

                // Clear memory
                if (input.Equals("/reset", StringComparison.OrdinalIgnoreCase))
                {
                    lastTopic = string.Empty;
                    conversationHistory.Clear();
                    Info("Memory cleared.");
                    continue;
                }

                // Show history
                if (input.Equals("/history", StringComparison.OrdinalIgnoreCase))
                {
                    PrintDivider("🕘 Conversation History");
                    foreach (var msg in conversationHistory)
                        Console.WriteLine($"• {msg}");
                    PrintDivider();
                    continue;
                }

                // Context-aware follow-ups
                string combined = input;
                if (IsVague(input) && !string.IsNullOrWhiteSpace(lastTopic) && ShouldCombine(input, lastTopic))
                    combined = lastTopic + " " + input;

                // Get answer
                string answer = KnowledgeBase.GetAnswer(combined);
                PrintWithTypingEffect(answer);

                // Remember for next context
                if (!input.StartsWith("/") && input.Length > 5)
                    lastTopic = input;
            }
        }

        static void PlayGreetingWithText(string message, int delay)
        {
            var audioThread = new Thread(() =>
            {
                try
                {
                    if (OperatingSystem.IsWindows())
                    {
                        using var player = new SoundPlayer("greeting.wav");
                        player.PlaySync();
                    }
                }
                catch
                {
                    Warn("Could not play greeting audio.");
                }
            });
            audioThread.Start();
            PrintWithTypingEffect(message, delay);
            audioThread.Join();
        }

        static void DisplayWelcome()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            try
            {
                Console.WriteLine(File.ReadAllText("ascii_logo.txt"));
            }
            catch
            {
                Console.WriteLine("=== Welcome to CyberSecurityBot ===");
            }
            Console.ResetColor();
        }

        static void PrintWithTypingEffect(string msg, int delay = 25)
        {
            foreach (char c in msg)
            {
                Console.Write(c);
                Thread.Sleep(delay);
            }
            Console.WriteLine();
        }

        static void PrintDivider(string title = null)
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

        static bool IsVague(string input)
        {
            string[] vague = { "what", "how", "why", "can", "should", "is", "are", "do" };
            var lw = input.ToLower();
            foreach (var v in vague) if (lw.StartsWith(v)) return true;
            return input.Length < 8;
        }

        static bool ShouldCombine(string current, string previous)
        {
            var stop = new HashSet<string>
            {
                "how","do","is","my","what","should","can","i","you","the","a","an","it","to"
            };
            var cur = current.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var prev = previous.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var w in cur)
            {
                if (stop.Contains(w)) continue;
                foreach (var p in prev)
                    if (!stop.Contains(p) && w == p)
                        return true;
            }
            return false;
        }

        static void Warn(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("⚠️ " + msg);
            Console.ResetColor();
        }
        static void Success(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✅ " + msg);
            Console.ResetColor();
        }
        static void Info(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("🔄 " + msg);
            Console.ResetColor();
        }
    }
}
