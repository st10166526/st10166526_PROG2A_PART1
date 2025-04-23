using System;
using System.Data.SQLite;
using System.IO;
using CyberSecurityBot;


namespace CyberSecurityBot
{
    public static class DatabaseSetup
    {
        private static readonly string dbPath = "knowledge.db";
        private static readonly string sqlPath = "CreateDB.sql";

        public static void Initialize()
        {
            if (!File.Exists(dbPath))
            {
                Console.WriteLine("⚠️ Database file not found. Creating a new one...");
                SQLiteConnection.CreateFile(dbPath);
            }

            using var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;");
            connection.Open();

            // Check if table exists
            var checkCommand = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name='Knowledge';", connection);
            var result = checkCommand.ExecuteScalar();

            if (result == null)
            {
                Console.WriteLine("🛠 Setting up knowledge base...");
                string sql = File.ReadAllText(sqlPath);

                using var command = new SQLiteCommand(sql, connection);
                command.ExecuteNonQuery();

                Console.WriteLine("✅ Knowledge base created and populated.");
            }
            else
            {
                Console.WriteLine("📚 Knowledge base loaded.");
            }
        }
    }
}
