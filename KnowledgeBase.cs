using System;
using System.Data.SQLite;

namespace CyberSecurityBot
{
    public static class KnowledgeBase
    {
        private static readonly string connectionString = "Data Source=knowledge.db;Version=3;";

        public static string GetAnswer(string input)
        {
            using var connection = new SQLiteConnection(connectionString);
            connection.Open();

            string keyword = input.ToLower();
            string query = "SELECT Answer FROM Knowledge WHERE LOWER(@keyword) LIKE '%' || LOWER(Question) || '%' LIMIT 1";
;

            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@keyword", keyword);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return reader.GetString(0);
            }

            return GetFallbackResponse();
        }

        private static string GetFallbackResponse()
        {
            var responses = new[]
            {
                "🤔 I’m not sure about that one. Try asking about 2FA, phishing, or VPNs.",
                "😅 Hmm... can you rephrase your question or try another topic?",
                "🧠 I'm still learning. Could you try asking about something like 'malware' or 'passwords'?"
            };

            var rand = new Random();
            return responses[rand.Next(responses.Length)];
        }
    }
}

