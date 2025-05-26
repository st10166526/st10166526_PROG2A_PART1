using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CyberSecurityBot
{
    public static class KnowledgeBase
    {
        private static readonly string BasePath = AppContext.BaseDirectory;



        private static readonly string ConnStr =
    $"Data Source={Path.Combine(AppContext.BaseDirectory, "knowledge.db")};Version=3;";

        private static readonly Random Rng       = new();

        // these get populated only after the DB is seeded
        private static bool _loaded   = false;
        private static List<(string Q, string A, string C)>? _entries;
        private static Dictionary<string, string>? _map;

        // synonyms & sentiment maps (unchanged)
        static readonly Dictionary<string,string> Synonyms = new(StringComparer.OrdinalIgnoreCase)
        {
            ["remove malware"] = "how do i remove malware",
            // ...
        };
        static readonly Dictionary<string,string> Sentiment = new(StringComparer.OrdinalIgnoreCase)
        {
            ["worried"]    = "It’s understandable to feel worried—here’s a tip:",
            // ...
        };

        private static void LoadAll()
        {
            if (_loaded) return;
            DatabaseSetup.Initialize();

            _entries = new();
            _map     = new(StringComparer.OrdinalIgnoreCase);

            using var conn = new SQLiteConnection(ConnStr);
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT Keyword,Answer,Category FROM Knowledge", conn);
            using var rd  = cmd.ExecuteReader();
            while (rd.Read())
            {
                var q = rd.GetString(0);
                var a = rd.GetString(1);
                var c = rd.GetString(2);
                _entries.Add((q, a, c));
                if (!_map.ContainsKey(q))
                    _map[q] = c;
            }
            _loaded = true;
        }

        public static (string Answer, string Category) LookupWithCategory(string raw)
        {
            LoadAll();
            var input = (raw ?? "").Trim();

            // 1) apply synonyms
            foreach (var kv in Synonyms)
                if (input.Contains(kv.Key, StringComparison.OrdinalIgnoreCase))
                    input = Regex.Replace(input, Regex.Escape(kv.Key), kv.Value, RegexOptions.IgnoreCase);

            // 2) sentiment
            foreach (var kv in Sentiment)
            {
                if (input.Contains(kv.Key, StringComparison.OrdinalIgnoreCase))
                {
                    var resp = kv.Value;
                    var topic = _map != null
                        ? _map.Keys.FirstOrDefault(k => input.Contains(k, StringComparison.OrdinalIgnoreCase))
                        : null;
                    if (topic != null && _map != null)
                    {
                        var tip = GetRandomTip(topic);
                        return ($"{resp} {tip}", _map[topic]);
                    }
                    return (resp, "");
                }
            }

            // 3) exact keyword
            if (_map != null)
            {
                foreach (var kv in _map)
                {
                    if (input.Contains(kv.Key, StringComparison.OrdinalIgnoreCase))
                        return (GetRandomTip(kv.Value), kv.Value);
                }
            }

            // 4) general chit-chat
            if (_entries != null)
            {
                foreach (var e in _entries.Where(x => x.C.Equals("General", StringComparison.OrdinalIgnoreCase)))
                {
                    if (Regex.IsMatch(input, $@"\b{Regex.Escape(e.Q)}\b", RegexOptions.IgnoreCase))
                        return (e.A, e.C);
                }
            }

            // 5) fuzzy fallback
            var others = _entries.Where(x => !x.C.Equals("General", StringComparison.OrdinalIgnoreCase)).ToList();
            if (others.Count>0)
            {
                var tokens = Tokenize(input);
                double best = 0;
                (string A, string C) bestMatch = (GetFallback(), "");
                foreach (var e in others)
                {
                    var qt = Tokenize(e.Q);
                    var overlap = qt.Count(t => tokens.Contains(t)) / (double)qt.Count;
                    var sim = 1.0 - Levenshtein(input, e.Q) / (double)Math.Max(input.Length, e.Q.Length);
                    var score = 0.5 * overlap + 0.5 * sim;
                    if (score > best)
                    {
                        best = score;
                        bestMatch = (e.A, e.C);
                    }
                }
                if (best >= 0.3)
                    return bestMatch;
            }

            // 6) final fallback
            return (GetFallback(), "");
        }

        // helpers for retrieving tips, inserting, fallback, tokenizing, etc.
        public static List<string> GetAllQuestions()
            => _entries.Select(x => x.Q).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(q => q).ToList();

        public static List<string> GetRelatedTips(string cat)
            => _entries.Where(x => x.C.Equals(cat, StringComparison.OrdinalIgnoreCase)).Select(x => x.A).ToList();

        public static bool InsertEntry(string q, string a)
        {
            try
            {
                using var conn = new SQLiteConnection(ConnStr);
                conn.Open();
                using var cmd = new SQLiteCommand(
                    "INSERT INTO Knowledge (Keyword,Answer,Category) VALUES(@q,@a,'General')", conn);
                cmd.Parameters.AddWithValue("@q", q);
                cmd.Parameters.AddWithValue("@a", a);
                cmd.ExecuteNonQuery();
                // reflect in‐memory immediately
                _entries.Add((q, a, "General"));
                if (!_map.ContainsKey(q)) _map[q] = "General";
                return true;
            }
            catch { return false; }
        }

        private static string GetRandomTip(string cat)
        {
            var tips = _entries.Where(x => x.C == cat).Select(x => x.A).ToList();
            return tips.Count == 0 ? GetFallback() : tips[Rng.Next(tips.Count)];
        }

        private static string GetFallback()
        {
            var fb = new[]
            {
                "🤔 I’m not sure—try asking about passwords, phishing, or Wi-Fi.",
                "😅 I’m still learning. Ask another topic!",
                "🧠 Sorry, I don’t have that yet."
            };
            return fb[Rng.Next(fb.Length)];
        }

        private static HashSet<string> Tokenize(string s)
            => s.Split(new[]{' ','.',',','?','!','-','\''},StringSplitOptions.RemoveEmptyEntries)
                .Select(w => w.ToLowerInvariant())
                .Where(w => w.Length>2 && !"the,a,an,and,or,of,to,in,on,for,is,are,do,how,what,my,your".Split(',').Contains(w))
                .ToHashSet();

        private static int Levenshtein(string a, string b)
        {
            int n = a.Length, m = b.Length;
            var dp = new int[n+1,m+1];
            for(int i=0;i<=n;i++) dp[i,0] = i;
            for(int j=0;j<=m;j++) dp[0,j] = j;
            for(int i=1;i<=n;i++) for(int j=1;j<=m;j++)
            {
                int cost = a[i-1]==b[j-1]?0:1;
                dp[i,j] = Math.Min(Math.Min(dp[i-1,j]+1, dp[i,j-1]+1), dp[i-1,j-1]+cost);
            }
            return dp[n,m];
        }
    }
}





