using System;
using System.Collections.Generic;

namespace CyberSecurityBot
{
    public static class KnowledgeBase
    {
        private static readonly Dictionary<string, string> QnA = new(StringComparer.OrdinalIgnoreCase)
        {
            { "password", "🔒 Use strong, unique passwords and enable a password manager to help you remember them." },
            { "phishing", "🎣 Be cautious of emails asking for credentials. Always verify links before clicking." },
            { "wifi", "📶 Public Wi-Fi is risky. Use a VPN whenever you’re on an open network." },
            { "update", "⬆️ Keeping software updated patches security flaws and helps keep you safe." },
            { "2fa", "🔐 Two-Factor Authentication adds a second layer of protection — always enable it!" },
            { "malware", "🛡️ Use antivirus software, and avoid downloading unknown files or clicking pop-ups." },
            { "vpn", "🌐 A VPN encrypts your internet traffic, which protects your data from snooping." },
            { "hacked", "🚨 Disconnect from the internet, run antivirus, and change your passwords immediately." }
        };

        private static readonly string[] FallbackResponses = new[]
        {
            "🤔 I’m not sure about that. Try asking something related to passwords, phishing, or Wi-Fi.",
            "🔍 That's a tricky one! Could you rephrase it?",
            "🤖 I don’t know that yet, but I’m learning! Try a different question."
        };

        public static string GetAnswer(string input)
        {
            foreach (var pair in QnA)
            {
                if (input.Contains(pair.Key, StringComparison.OrdinalIgnoreCase))
                {
                    return pair.Value;
                }
            }

            var rand = new Random();
            return FallbackResponses[rand.Next(FallbackResponses.Length)];
        }
    }
}
