using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Threading;

namespace CyberSecurityBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Cybersecurity Awareness Bot";
            PlayGreeting();
            ShowAsciiArtFromFile("ascii_logo.txt");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("\nWhat is your name? ");
            Console.ResetColor();
            string name = Console.ReadLine() ?? "Guest";

            Console.WriteLine();
            PrintWithTypingEffect($"Hello, {name}! I'm here to help you stay safe online.\nType a cybersecurity question, or type 'exit' to leave.\n");

            var qna = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "password", "Use strong, unique passwords for each account. A password manager helps!" },
                { "phishing", "Never click suspicious links. Check email sender addresses and grammar." },
                { "wifi", "Avoid using public Wi-Fi for sensitive transactions without a VPN." },
                { "update", "Keep your software and systems updated to prevent vulnerabilities." },
                { "2fa", "Enable two-factor authentication wherever possible!" },
                { "hacked", "Immediately disconnect from the internet, run antivirus software, and change all passwords." },
                { "malware", "Avoid downloading from untrusted sites. Use updated antivirus tools regularly." },
                { "vpn", "Use a VPN to encrypt your data on public Wi-Fi networks." }
            };

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\nAsk me something: ");
                Console.ResetColor();
                string input = (Console.ReadLine() ?? string.Empty).Trim();

                if (string.IsNullOrEmpty(input))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("⚠️ Please enter a valid question.");
                    Console.ResetColor();
                    continue;
                }

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("👋 Stay safe, and goodbye!");
                    break;
                }

                bool found = false;
                foreach (var pair in qna)
                {
                    if (input.ToLower().Contains(pair.Key))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(pair.Value);
                        Console.ResetColor();
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("❓ I don't quite understand that. Try asking about passwords, phishing, or updates.");
                    Console.ResetColor();
                }
            }
        }

static void PlayGreeting()
{
    try
    {
        // What the WAV file says (subtitle)
        string subtitle = "Hello, and welcome to your Cybersecurity Awareness Bot! I am here to help you stay informed, and safe online!";
        
        // Start audio in parallel
        Task.Run(() =>
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
        });

        // Print subtitle as typing effect while audio plays
        PrintWithTypingEffect(subtitle, 15);
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("⚠️ Could not play greeting audio: " + ex.Message);
        Console.ResetColor();
    }
}


        static void ShowAsciiArtFromFile(string filePath)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                string asciiArt = File.ReadAllText(filePath);
                Console.WriteLine(asciiArt);
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("⚠️ Could not load ASCII art: " + ex.Message);
                Console.ResetColor();
            }
        }

        static void PrintWithTypingEffect(string text, int delay = 30)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(delay);
            }
            Console.WriteLine();
        }
    }
}
