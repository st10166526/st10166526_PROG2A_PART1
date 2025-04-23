using System;
using System.Data.SQLite;
using System.IO;

namespace CyberSecurityBot
{
    public static class DatabaseSetup
    {
        private const string DbFile  = "knowledge.db";
        private const string SqlFile = "CreateDB.sql";

        public static void Initialize()
        {
            if (!File.Exists(DbFile))
            {
                // 1) Create empty DB
                SQLiteConnection.CreateFile(DbFile);

                // 2) Read & execute your schema + seed script
                var sql = File.ReadAllText(SqlFile);
                using var conn = new SQLiteConnection($"Data Source={DbFile};Version=3;");
                conn.Open();
                using var cmd = new SQLiteCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
