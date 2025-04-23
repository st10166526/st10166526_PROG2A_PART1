using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace CyberSecurityBot
{
    public static class KnowledgeBase
    {
        private static readonly string connStr = "Data Source=knowledge.db;Version=3;";

        public static string GetAnswer(string input)
        {
            using var conn = new SQLiteConnection(connStr);
            conn.Open();

            using var cmd = new SQLiteCommand("SELECT Question, Answer FROM Knowledge", conn);
            using var rd = cmd.ExecuteReader();

            string best = GetFallbackResponse();
            double bestScore = 0.0;

            // Prepare input tokens
            var inputTokens = Tokenize(input);

            while (rd.Read())
            {
                string question = rd.GetString(0);
                string answer   = rd.GetString(1);

                // Keyword overlap score
                var qTokens = Tokenize(question);
                int overlap = 0;
                foreach (var tok in inputTokens)
                    if (qTokens.Contains(tok)) overlap++;

                double kwScore = (qTokens.Count>0) ? (double)overlap / qTokens.Count : 0.0;

                // Similarity score via Levenshtein
                double simScore = ComputeSimilarity(input.ToLower(), question.ToLower());

                // Weighted combined score
                double score = 0.6 * simScore + 0.4 * kwScore;

                if (score > bestScore)
                {
                    bestScore = score;
                    best = answer;
                }
            }

            return best;
        }

        public static List<string> GetAllQuestions()
        {
            var list = new List<string>();
            using var conn = new SQLiteConnection(connStr);
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT Question FROM Knowledge ORDER BY Question", conn);
            using var rd = cmd.ExecuteReader();
            while (rd.Read()) list.Add(rd.GetString(0));
            return list;
        }

        public static bool InsertEntry(string question, string answer)
        {
            try
            {
                using var conn = new SQLiteConnection(connStr);
                conn.Open();
                using var cmd = new SQLiteCommand(
                    "INSERT INTO Knowledge (Question, Answer) VALUES (@q,@a)", conn);
                cmd.Parameters.AddWithValue("@q", question);
                cmd.Parameters.AddWithValue("@a", answer);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch { return false; }
        }

        private static string GetFallbackResponse()
        {
            var fallback = new[]
            {
                "🤔 I’m not sure. Try asking about passwords, phishing, or Wi-Fi.",
                "😅 Could you rephrase that? I’m still learning.",
                "🧠 I don’t have an answer yet. Try another topic."
            };
            var rnd = new Random();
            return fallback[rnd.Next(fallback.Length)];
        }

        // Tokenize into lowercase alphanumeric words, filter stopwords
        private static HashSet<string> Tokenize(string text)
        {
            var stop = new HashSet<string>
            {
                "the","a","an","and","or","of","to","in","on","for","is","are","do","how","i","you","my","your"
            };
            var tokens = new HashSet<string>();
            foreach (var w in text.Split(new[]{' ','?','!','.',','}, StringSplitOptions.RemoveEmptyEntries))
            {
                var t = w.ToLower().Trim();
                if (t.Length<2 || stop.Contains(t)) continue;
                tokens.Add(t);
            }
            return tokens;
        }

        // Levenshtein distance ratio
        private static double ComputeSimilarity(string s, string t)
        {
            int d = LevenshteinDistance(s, t);
            int max = Math.Max(s.Length, t.Length);
            return max == 0 ? 1.0 : 1.0 - (double)d / max;
        }

        private static int LevenshteinDistance(string s, string t)
        {
            int n = s.Length, m = t.Length;
            var dp = new int[n + 1, m + 1];
            for (int i = 0; i <= n; i++) dp[i, 0] = i;
            for (int j = 0; j <= m; j++) dp[0, j] = j;
            for (int i = 1; i <= n; i++)
            for (int j = 1; j <= m; j++)
            {
                int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;
                dp[i, j] = Math.Min(
                    Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
                    dp[i - 1, j - 1] + cost);
            }
            return dp[n, m];
        }
    }
}


