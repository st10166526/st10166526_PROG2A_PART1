using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using System.Threading;

namespace CyberSecurityBot
{
    class Program
    {
        private const int GREETING_DELAY = 63;
        private static readonly string BasePath = AppContext.BaseDirectory;

        static List<string>            history       = new();
        static string?                 lastCat       = null;
        static string?                 lastAnswer    = null;
        static Dictionary<string, string> memory     = new(StringComparer.OrdinalIgnoreCase);

        static void Main(string[] args)
        {
            DatabaseSetup.Initialize();

            PlayGreetingWithText(
              "Hello! Welcome to the Cybersecurity Awareness Bot. " +
              "I’m here to help you stay safe online."
            );

            DisplayWelcome();
            AskName();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\nYou: ");
                Console.ResetColor();

                var input = Console.ReadLine()?.Trim() ?? "";
                history.Add(input);

                if (string.IsNullOrWhiteSpace(input))
                {
                    Warn("Please ask something.");
                    continue;
                }

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    Success("Goodbye! Stay cyber safe.");
                    break;
                }

                if (HandleSlash(input))       continue;
                if (TryFollowUp(input))       continue;
                if (TryRemember(input))       continue;

                var (ans, cat) = KnowledgeBase.LookupWithCategory(input);
                lastAnswer = ans;
                lastCat    = cat;

                // personalize if they’ve told us their favorite topic
                if (memory.TryGetValue("favoriteTopic", out var fav)
                    && !string.IsNullOrEmpty(cat)
                    && cat.Equals(fav, StringComparison.OrdinalIgnoreCase))
                {
                    PrintWithTypingEffect(
                      $"As someone interested in {fav}, here’s another tip:");
                }

                PrintWithTypingEffect(ans);
            }
        }

        static void AskName()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("\nWhat is your name? ");
            Console.ResetColor();

            var n = Console.ReadLine()?.Trim() ?? "Guest";
            memory["UserName"] = n;

            PrintWithTypingEffect(
              $"Hello {n}, I'm your friendly cybersecurity assistant!\n" +
              "Type any question, or 'exit' to quit.");
        }

        static bool HandleSlash(string input)
        {
            var cmd = input.ToLowerInvariant();
            if (!cmd.StartsWith("/"))
                return false;

            switch (cmd)
            {
                case "/list":
                    PrintDivider("Topics I know:");
                    foreach (var question in KnowledgeBase.GetAllQuestions())
                        Console.WriteLine($"• {question}");
                    PrintDivider();
                    break;

                case "/add":
                    PrintDivider("📝 Add a new Q&A");
                    Console.Write("Keyword: ");
                    var q = Console.ReadLine()?.Trim() ?? "";
                    Console.Write("Answer : ");
                    var a = Console.ReadLine()?.Trim() ?? "";
                    if (KnowledgeBase.InsertEntry(q, a))
                        Success("Added!");
                    else
                        Warn("Add failed.");
                    PrintDivider();
                    break;

                case "/history":
                    PrintDivider("🕘 History");
                    foreach (var m in history)
                        Console.WriteLine($"• {m}");
                    PrintDivider();
                    break;

                case "/reset":
                    lastCat = lastAnswer = null;
                    memory.Clear();
                    Info("Memory cleared.");
                    break;

                default:
                    Warn("Unknown command. Try /list, /add, /history or /reset.");
                    break;
            }

            return true;
        }

        static bool TryFollowUp(string input)
        {
            var l = input.ToLowerInvariant();
            if (lastCat != null &&
               (l.Contains("more") || l.Contains("another") ||
                l.Contains("what else") || l.Contains("next")))
            {
                var tips = KnowledgeBase.GetRelatedTips(lastCat);
                if (tips.Count > 0)
                {
                    var idx = lastAnswer != null 
                            ? tips.IndexOf(lastAnswer) 
                            : -1;
                    var nxt = (idx >= 0 && idx + 1 < tips.Count)
                              ? tips[idx + 1]
                              : tips[0];
                    lastAnswer = nxt;
                    PrintWithTypingEffect(nxt);
                    return true;
                }
            }
            return false;
        }

       static bool TryRemember(string input)
{
    var lower = input.ToLowerInvariant();
    if (lower.Contains("interested in") || lower.Contains("favorite topic"))
    {
        // look through all category names for a match
        var match = KnowledgeBase
            .GetAllCategories()
            .FirstOrDefault(cat => 
                input.IndexOf(cat, StringComparison.OrdinalIgnoreCase) >= 0
            );

        if (match != null)
        {
            memory["favoriteTopic"] = match;
            PrintWithTypingEffect($"Got it—I’ll remember you like {match}.");
            return true;
        }
    }
    return false;
}

        [SupportedOSPlatform("windows")]
        static void PlayGreetingWithText(string msg)
        {
            if (OperatingSystem.IsWindows())
            {
                var thr = new Thread(() =>
                {
                    try
                    {
                        using var player = new SoundPlayer(
                            Path.Combine(BasePath, "greeting.wav"));
                        player.PlaySync();
                    }
                    catch
                    {
                        Warn("Could not play greeting audio.");
                    }
                });
                thr.Start();
                PrintWithTypingEffect(msg, GREETING_DELAY);
                thr.Join();
            }
            else
            {
                PrintWithTypingEffect(msg, GREETING_DELAY);
            }
        }

        static void DisplayWelcome()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            try
            {
                Console.WriteLine(
                  File.ReadAllText(Path.Combine(BasePath, "ascii_logo.txt"))
                );
            }
            catch
            {
                Console.WriteLine("=== Cybersecurity Awareness Bot ===");
            }
            Console.ResetColor();
        }

        static void PrintWithTypingEffect(string s, int d = 25)
        {
            foreach (var c in s)
            {
                Console.Write(c);
                Thread.Sleep(d);
            }
            Console.WriteLine();
        }

        static void PrintDivider(string? title = null)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(new string('═', 50));
            if (!string.IsNullOrEmpty(title))
            {
                Console.WriteLine($"  {title}");
                Console.WriteLine(new string('═', 50));
            }
            Console.ResetColor();
        }

        static void Warn(string m)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("⚠️ " + m);
            Console.ResetColor();
        }

        static void Success(string m)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✅ " + m);
            Console.ResetColor();
        }

        static void Info(string m)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("🔄 " + m);
            Console.ResetColor();
        }
    }
}


