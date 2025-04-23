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

        // A tiny set of synonyms / phrase-mappings to unify input
        static readonly Dictionary<string, string> _synonyms = new()
        {
            { "remove malware", "malware" },
            { "clear malware",  "malware" },
            { "public charging", "wifi"    },
            { "free wifi",       "wifi"    },
            { "password better", "password"}
        };

        public static string GetAnswer(string rawInput)
        {
            // 1) Normalize & apply synonyms
            var input = rawInput.ToLower();
            foreach (var (k,v) in _synonyms)
                if (input.Contains(k)) input = input.Replace(k, v);

            // 2) If it's a general chat question, answer directly from the DB
            foreach (var gen in GetByCategory("General"))
                if (Regex.IsMatch(input, $@"\b{Regex.Escape(gen.Keyword)}\b"))
                    return gen.Answer;

            // 3) Fetch all other topics
            var all = GetAllEntries().Where(e => e.Category != "General").ToList();
            if (all.Count == 0) return GetFallback();

            // 4) Score each entry by keyword overlap & Levenshtein similarity
            var tokens = Tokenize(input);
            double bestScore = 0;
            string bestAns = GetFallback();

            foreach (var e in all)
            {
                var qTokens = Tokenize(e.Keyword);
                // overlap ratio
                double overlap = qTokens.Count>0
                    ? qTokens.Count(w => tokens.Contains(w)) / (double)qTokens.Count
                    : 0;
                // string similarity
                double sim = ComputeSimilarity(input, e.Keyword);
                // weighted score
                double score = 0.5 * overlap + 0.5 * sim;
                if (score > bestScore)
                {
                    bestScore = score;
                    bestAns = e.Answer;
                }
            }

            // 5) enforce a minimum confidence
            return bestScore < 0.3 ? GetFallback() : bestAns;
        }

        // Load all entries from DB
        static List<(string Keyword,string Answer,string Category)> GetAllEntries()
        {
            var list = new List<(string,string,string)>();
            using var conn = new SQLiteConnection(connStr);
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT Keyword,Answer,Category FROM Knowledge", conn);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                list.Add((rd.GetString(0), rd.GetString(1), rd.GetString(2)));
            return list;
        }

        // Shortcut to fetch only one category
        static List<(string Keyword,string Answer,string Category)> GetByCategory(string cat)
            => GetAllEntries().Where(e => e.Category.Equals(cat, StringComparison.OrdinalIgnoreCase)).ToList();

        public static List<string> GetAllQuestions()
            => GetAllEntries()
               .Select(e => e.Keyword)
               .Distinct()
               .OrderBy(k => k)
               .ToList();

        public static bool InsertEntry(string q, string a)
        {
            try
            {
                using var conn = new SQLiteConnection(connStr);
                conn.Open();
                using var cmd = new SQLiteCommand(
                  "INSERT INTO Knowledge (Keyword,Answer,Category) VALUES (@q,@a,'General')", conn);
                cmd.Parameters.AddWithValue("@q", q);
                cmd.Parameters.AddWithValue("@a", a);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch { return false; }
        }

        static string GetFallback()
        {
            var fb = new[]
            {
                "🤔 I’m not sure. Could you ask about passwords, phishing or Wi-Fi?",
                "😅 Hmm… I’m still learning. Try another topic!",
                "🧠 I don’t have an answer for that yet."
            };
            return fb[new Random().Next(fb.Length)];
        }

        // Split into words, filter out short/common words
        static HashSet<string> Tokenize(string text)
        {
            var stop = new HashSet<string>{ "the","a","an","and","or","of","to","in","on","for","is","are","do","how","what","my","your" };
            return text
                .ToLower()
                .Split(new[]{' ','.',',','?','!','-'}, StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.Length>2 && !stop.Contains(w))
                .ToHashSet();
        }

        // Levenshtein distance → ratio
        static double ComputeSimilarity(string s, string t)
        {
            int d = Levenshtein(s,t); 
            int max = Math.Max(s.Length,t.Length);
            return max==0?1.0:1.0 - (double)d/max;
        }
        static int Levenshtein(string s, string t)
        {
            int n=s.Length, m=t.Length;
            var dp = new int[n+1,m+1];
            for(int i=0;i<=n;i++) dp[i,0]=i;
            for(int j=0;j<=m;j++) dp[0,j]=j;
            for(int i=1;i<=n;i++)
            for(int j=1;j<=m;j++)
            {
                int cost = s[i-1]==t[j-1]?0:1;
                dp[i,j]=Math.Min(Math.Min(dp[i-1,j]+1,dp[i,j-1]+1),dp[i-1,j-1]+cost);
            }
            return dp[n,m];
        }
    }
}



