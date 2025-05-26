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
        private static readonly Random rnd = new();

        // 1) Common aliases → DB keywords
        static readonly Dictionary<string, string> Synonyms = new()
        {
            ["remove malware"]          = "how do i remove malware",
            ["clear malware"]           = "how do i remove malware",
            ["i might be hacked"]       = "i think i got hacked",
            ["how do i not get hacked"] = "what to do if hacked",
            ["can i get hacked"]        = "what to do if hacked",
            ["if i get hacked"]         = "what to do if hacked"
        };

        // 2) Dynamically build a map from each Keyword to its Category
        static readonly Dictionary<string, string> KeywordCategoryMap;

        static KnowledgeBase()
        {
            KeywordCategoryMap = new Dictionary<string, string>();
            using var conn = new SQLiteConnection(connStr);
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT Keyword,Category FROM Knowledge", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var key = reader.GetString(0).ToLower();
                var cat = reader.GetString(1);
                if (!KeywordCategoryMap.ContainsKey(key))
                    KeywordCategoryMap.Add(key, cat);
            }
        }

        /// <summary>
        /// Returns (Answer, Category) by:
        ///  1) applying synonyms
        ///  2) exact keyword lookup
        ///  3) general chit-chat (Category = "General")
        ///  4) fuzzy match scoring on everything else
        /// </summary>
        public static (string Answer, string Category) LookupWithCategory(string rawInput)
        {
            var input = rawInput.ToLower().Trim();

            // 1) apply synonyms
            foreach (var kv in Synonyms)
                if (input.Contains(kv.Key))
                    input = input.Replace(kv.Key, kv.Value);

            // 2) exact keyword → category
            foreach (var kv in KeywordCategoryMap)
            {
                if (input.Contains(kv.Key))
                {
                    var tips = GetRelatedTips(kv.Value);
                    if (tips.Count > 0)
                        return (tips[rnd.Next(tips.Count)], kv.Value);
                }
            }

            // 3) general chit-chat (Category = "General")
            foreach (var e in GetByCategory("General"))
            {
                if (Regex.IsMatch(input, $@"\b{Regex.Escape(e.Keyword.ToLower())}\b"))
                    return (e.Answer, e.Category);
            }

            // 4) fuzzy scoring fallback
            var allTopics = GetAllEntries()
                .Where(e => !e.Category.Equals("General", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!allTopics.Any())
                return (GetFallback(), string.Empty);

            double bestScore = 0;
            (string Answer, string Category) bestMatch = (GetFallback(), string.Empty);
            var tokens = Tokenize(input);

            foreach (var e in allTopics)
            {
                var q = e.Keyword.ToLower();
                var qTokens = Tokenize(q);
                double overlap = qTokens.Count > 0
                    ? qTokens.Count(t => tokens.Contains(t)) / (double)qTokens.Count
                    : 0.0;
                double sim = ComputeSimilarity(input, q);
                double score = 0.5 * overlap + 0.5 * sim;
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMatch = (e.Answer, e.Category);
                }
            }

            if (bestScore < 0.3)
                return (GetFallback(), string.Empty);

            return bestMatch;
        }

        /// <summary>Fetch all tips for a given category.</summary>
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

        /// <summary>Get distinct entries by category (for “General” chit-chat).</summary>
        public static List<(string Keyword, string Answer, string Category)> GetByCategory(string category)
            => GetAllEntries()
                .Where(e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .ToList();

        /// <summary>Read every (Keyword,Answer,Category) tuple from the DB.</summary>
        public static List<(string Keyword, string Answer, string Category)> GetAllEntries()
        {
            var entries = new List<(string, string, string)>();
            using var conn = new SQLiteConnection(connStr);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "SELECT Keyword,Answer,Category FROM Knowledge", conn);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                entries.Add((rd.GetString(0), rd.GetString(1), rd.GetString(2)));
            return entries;
        }

        // Returns a random fallback when nothing else matches
        static string GetFallback()
        {
            var fb = new[]
            {
                "🤔 I’m not sure. Could you ask about passwords, phishing, or Wi-Fi?",
                "😅 Hmm… I’m still learning. Try another topic!",
                "🧠 I don’t have an answer for that yet."
            };
            return fb[rnd.Next(fb.Length)];
        }

        // Breaks text into meaningful tokens, removing stopwords
        static HashSet<string> Tokenize(string text)
        {
            var stop = new HashSet<string>
            { "the","a","an","and","or","of","to","in","on","for","is","are","do","how","what","my","your" };
            return text
                .Split(new[] { ' ', '.', ',', '?', '!', '-', '\'' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(w => w.ToLower())
                .Where(w => w.Length > 2 && !stop.Contains(w))
                .ToHashSet();
        }

        // A simple normalized Levenshtein‐based similarity
        static double ComputeSimilarity(string s, string t)
        {
            int d = Levenshtein(s, t);
            int max = Math.Max(s.Length, t.Length);
            return max == 0 ? 1.0 : 1.0 - (double)d / max;
        }

        // Standard DP for edit distance
        static int Levenshtein(string s, string t)
        {
            int n = s.Length, m = t.Length;
            var dp = new int[n + 1, m + 1];
            for (int i = 0; i <= n; i++) dp[i, 0] = i;
            for (int j = 0; j <= m; j++) dp[0, j] = j;
            for (int i = 1; i <= n; i++)
                for (int j = 1; j <= m; j++)
                {
                    int cost = s[i - 1] == t[j - 1] ? 0 : 1;
                    dp[i, j] = Math.Min(
                        Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
                        dp[i - 1, j - 1] + cost
                    );
                }
            return dp[n, m];
        }
    }
}



