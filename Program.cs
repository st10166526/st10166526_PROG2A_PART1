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
        private const int GREETING_DELAY = 63;

        static List<string> conversationHistory = new();
        static string? lastCategory = null;
        static string? lastAnswer   = null;

        static void Main(string[] args)
        {
            DatabaseSetup.Initialize();
            Console.Title = "Cybersecurity Awareness Bot";

            // 1) Play & type the greeting
            string greetingText = 
                "Hello! Welcome to the Cybersecurity Awareness Bot. " +
                "I’m here to help you stay safe online.";
            PlayGreetingWithText(greetingText, GREETING_DELAY);

            // 2) ASCII banner
            DisplayWelcome();

            // 3) Ask for name
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("\nWhat is your name? ");
            Console.ResetColor();
            string name = Console.ReadLine()?.Trim() ?? "Guest";

            Console.WriteLine();
            PrintWithTypingEffect(
                $"Hello {name}, I'm your friendly cybersecurity assistant!\n" +
                "Type a question (e.g. \"How are you?\"), or 'exit' to quit.\n"
            );

            // 4) REPL loop
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\nYou: ");
                Console.ResetColor();
                string input = Console.ReadLine()?.Trim() ?? "";
                conversationHistory.Add(input);

                if (string.IsNullOrEmpty(input))
                {
                    Warn("Please ask a question.");
                    continue;
                }

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    Success("Goodbye! Stay cyber safe out there.");
                    break;
                }

                // Slash‐commands
                if (HandleSlashCommands(input)) continue;

                // 5) Follow‐up: only when user explicitly asks “what else…”
                if (IsFollowUp(input) && lastCategory != null)
                {
                    var tips = KnowledgeBase.GetRelatedTips(lastCategory);
                    if (tips.Count > 0)
                    {
                        int idx = lastAnswer != null ? tips.IndexOf(lastAnswer) : -1;
                        string next = (idx >= 0 && idx + 1 < tips.Count)
                                      ? tips[idx + 1]
                                      : tips[0];
                        lastAnswer = next;
                        PrintWithTypingEffect(next);
                        continue;
                    }
                }

                // 6) Normal lookup
                var (answer, category) = KnowledgeBase.LookupWithCategory(input);
                lastAnswer   = answer;
                lastCategory = category;
                PrintWithTypingEffect(answer);
            }
        }

        static bool HandleSlashCommands(string input)
        {
            switch (input.ToLower())
            {
                case "/list":
                    PrintDivider("🧠 Topics I can help with:");
                    foreach (var t in KnowledgeBase.GetAllQuestions())
                        Console.WriteLine($"• {t}");
                    PrintDivider();
                    return true;

                case "/add":
                    PrintDivider("📝 Add a new topic");
                    Console.Write("Question keyword: ");
                    string q = Console.ReadLine()?.Trim() ?? "";
                    Console.Write("Answer: ");
                    string a = Console.ReadLine()?.Trim() ?? "";
                    bool ok = KnowledgeBase.InsertEntry(q, a);
                    if (ok) Success("Added successfully!");
                    else    Warn("Failed to add.");
                    PrintDivider();
                    return true;

                case "/history":
                    PrintDivider("🕘 Conversation History");
                    foreach (var m in conversationHistory)
                        Console.WriteLine($"• {m}");
                    PrintDivider();
                    return true;

                case "/reset":
                    lastCategory = null;
                    lastAnswer   = null;
                    conversationHistory.Clear();
                    Info("Memory cleared.");
                    return true;

                default:
                    return false;
            }
        }

        static bool IsFollowUp(string input)
        {
            string lower = input.ToLower();
            return lower.Contains("what else")
                || lower.Contains("anything else")
                || lower.Contains("what should i do")
                || lower.Contains("next step")
                || lower.Equals("what else");
        }

        static void PlayGreetingWithText(string message, int delay)
        {
            Thread audio = new(() =>
            {
                try
                {
                    if (OperatingSystem.IsWindows())
                    {
                        using var player = new SoundPlayer("greeting.wav");
                        player.PlaySync();
                    }
                }
                catch { Warn("Could not play greeting audio."); }
            });
            audio.Start();
            PrintWithTypingEffect(message, delay);
            audio.Join();
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
                Console.WriteLine("=== Cybersecurity Awareness Bot ===");
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

        static void Warn(string msg)    { Console.ForegroundColor = ConsoleColor.Red;    Console.WriteLine("⚠️ " + msg); Console.ResetColor(); }
        static void Success(string msg) { Console.ForegroundColor = ConsoleColor.Green;  Console.WriteLine("✅ " + msg); Console.ResetColor(); }
        static void Info(string msg)    { Console.ForegroundColor = ConsoleColor.Cyan;   Console.WriteLine("🔄 " + msg); Console.ResetColor(); }
    }
}


