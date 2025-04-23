using System;
using System.Collections.Generic;
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

            string query = "SELECT Answer FROM Knowledge WHERE LOWER(@keyword) LIKE '%' || LOWER(Question) || '%' LIMIT 1";

            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@keyword", input);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return reader.GetString(0);
            }

            return GetFallbackResponse();
        }

        public static List<string> GetAllQuestions()
        {
            var results = new List<string>();

            using var connection = new SQLiteConnection(connectionString);
            connection.Open();

            string query = "SELECT Question FROM Knowledge ORDER BY Question ASC";
            using var command = new SQLiteCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                results.Add(reader.GetString(0));
            }

            return results;
        }

        public static bool InsertEntry(string question, string answer)
        {
            try
            {
                using var connection = new SQLiteConnection(connectionString);
                connection.Open();

                string insert = "INSERT INTO Knowledge (Question, Answer) VALUES (@q, @a)";
                using var command = new SQLiteCommand(insert, connection);
                command.Parameters.AddWithValue("@q", question);
                command.Parameters.AddWithValue("@a", answer);
                command.ExecuteNonQuery();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string GetFallbackResponse()
        {
            var responses = new[]
            {
                "🤔 I'm not sure about that one. Try asking about 2FA, phishing, or VPNs.",
                "😅 Hmm... can you rephrase your question or try another topic?",
                "🧠 I'm still learning. Could you try asking about something like 'malware' or 'passwords'?"
            };

            var rand = new Random();
            return responses[rand.Next(responses.Length)];
        }
    }
}

