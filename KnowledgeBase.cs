using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text.RegularExpressions;

namespace CyberSecurityBot
{
    public static class KnowledgeBase
    {
        private static readonly string connStr = "Data Source=knowledge.db;Version=3;";

        // 1) Quick category keyword lists
        static readonly Dictionary<string, string[]> CategoryKeywords = new()
        {
            ["Password"] = new[] { "password", "pass", "credential", "manager" },
            ["Phishing"] = new[] { "phish", "email", "link", "sender", "spoof" },
            ["Network"]  = new[] { "wifi", "wi-fi", "router", "network", "vpn" },
            ["Malware"]  = new[] { "malware", "virus", "spyware", "scan", "trojan" },
            ["Breach"]   = new[] { "hack", "breach", "compromise", "leak" }
        };

        // 2) Common aliases → DB keywords
        static readonly Dictionary<string, string> Synonyms = new()
        {
            ["remove malware"]           = "how do i remove malware",
            ["clear malware"]            = "how do i remove malware",
            ["i might be hacked"]        = "i think i got hacked",
            ["how do i not get hacked"]  = "what to do if hacked",
            ["can i get hacked"]         = "what to do if hacked",
            ["if i get hacked"]          = "what to do if hacked"
        };

        /// <summary>
        /// Returns (answer, category) using:
        ///   1) direct category-keyword lookup
        ///   2) General chit-chat
        ///   3) fuzzy match scoring
        /// </summary>
        public static (string Answer, string Category) LookupWithCategory(string rawInput)
        {
            var input = rawInput.ToLower();

            // apply synonyms
            foreach (var kv in Synonyms)
                if (input.Contains(kv.Key))
                    input = input.Replace(kv.Key, kv.Value);

            // 1) QUICK CATEGORY KEYWORD MATCH
            var tokens = Tokenize(input);
            foreach (var cat in CategoryKeywords)
            {
                if (tokens.Any(t => cat.Value.Any(k => t.Contains(k))))
                {
                    // grab first tip in that category (breach-step1, etc.)
                    var tips = GetRelatedTips(cat.Key);
                    if (tips.Count > 0)
                        return (tips[0], cat.Key);
                }
            }

            // 2) GENERAL "How are you?" style answers
            foreach (var e in GetByCategory("General"))
            {
                if (Regex.IsMatch(input, $@"\b{Regex.Escape(e.Keyword)}\b"))
                    return (e.Answer, e.Category);
            }

            // 3) FUZZY SCORING FOR EVERYTHING ELSE
            var all = GetAllEntries().Where(e => e.Category != "General").ToList();
            if (!all.Any())
                return (GetFallback(), null);

            double bestScore = 0;
            string bestAns   = GetFallback();
            string bestCat   = null;

            foreach (var e in all)
            {
                var qTokens = Tokenize(e.Keyword);
                double overlap = qTokens.Count > 0
                    ? qTokens.Count(w => tokens.Contains(w)) / (double)qTokens.Count
                    : 0;
                double sim = ComputeSimilarity(input, e.Keyword);
                double score = 0.5 * overlap + 0.5 * sim;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestAns   = e.Answer;
                    bestCat   = e.Category;
                }
            }

            if (bestScore < 0.3)
                return (GetFallback(), null);

            return (bestAns, bestCat);
        }

        /// <summary>Get all tips in a category, ordered by Id.</summary>
        public static List<string> GetRelatedTips(string category)
        {
            var list = new List<string>();
            using var conn = new SQLiteConnection(connStr);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "SELECT Answer FROM Knowledge WHERE Category=@cat ORDER BY Id", conn);
            cmd.Parameters.AddWithValue("@cat", category);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                list.Add(rd.GetString(0));
            return list;
        }

        /// <summary>List distinct keywords for /list.</summary>
        public static List<string> GetAllQuestions()
        {
            var list = new List<string>();
            using var conn = new SQLiteConnection(connStr);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "SELECT DISTINCT Keyword FROM Knowledge ORDER BY Keyword", conn);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                list.Add(rd.GetString(0));
            return list;
        }

        /// <summary>Insert a new General Q&A via /add.</summary>
        public static bool InsertEntry(string question, string answer)
        {
            try
            {
                using var conn = new SQLiteConnection(connStr);
                conn.Open();
                using var cmd = new SQLiteCommand(
                    "INSERT INTO Knowledge (Keyword,Answer,Category) VALUES (@q,@a,'General')",
                    conn);
                cmd.Parameters.AddWithValue("@q", question);
                cmd.Parameters.AddWithValue("@a", answer);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch { return false; }
        }

        //----------------------------------------------------------------------

        static string GetFallback()
        {
            var fb = new[]
            {
                "🤔 I’m not sure. Could you ask about passwords, phishing, or Wi-Fi?",
                "😅 Hmm… I’m still learning. Try another topic!",
                "🧠 I don’t have an answer for that yet."
            };
            return fb[new Random().Next(fb.Length)];
        }

        static HashSet<string> Tokenize(string text)
        {
            var stop = new HashSet<string>{ "the","a","an","and","or","of","to","in","on","for","is","are","do","how","what","my","your" };
            return text
              .ToLower()
              .Split(new[]{' ','.',',','?','!','-','\''}, StringSplitOptions.RemoveEmptyEntries)
              .Where(w => w.Length > 2 && !stop.Contains(w))
              .ToHashSet();
        }

        static double ComputeSimilarity(string s, string t)
        {
            int d   = Levenshtein(s, t);
            int max = Math.Max(s.Length, t.Length);
            return max == 0 ? 1.0 : 1.0 - (double)d / max;
        }

        static int Levenshtein(string s, string t)
        {
            int n = s.Length, m = t.Length;
            var dp = new int[n + 1, m + 1];
            for (int i = 0; i <= n; i++) dp[i, 0] = i;
            for (int j = 0; j <= m; j++) dp[0, j] = j;
            for (int i = 1; i <= n; i++)
            for (int j = 1; j <= m; j++)
            {
                int cost = s[i-1] == t[j-1] ? 0 : 1;
                dp[i,j] = Math.Min(
                  Math.Min(dp[i-1,j] + 1, dp[i,j-1] + 1),
                  dp[i-1,j-1] + cost);
            }
            return dp[n,m];
        }

        static List<(string Keyword,string Answer,string Category)> GetAllEntries()
        {
            var list = new List<(string,string,string)>();
            using var conn = new SQLiteConnection(connStr);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "SELECT Keyword,Answer,Category FROM Knowledge", conn);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                list.Add((rd.GetString(0), rd.GetString(1), rd.GetString(2)));
            return list;
        }

        static List<(string Keyword,string Answer,string Category)> GetByCategory(string cat)
            => GetAllEntries()
              .Where(e=>e.Category.Equals(cat, StringComparison.OrdinalIgnoreCase))
              .ToList();
    }
}





