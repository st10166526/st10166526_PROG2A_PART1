using System;
using System.Data.SQLite;
using System.IO;

namespace CyberSecurityBot
{
    public static class DatabaseSetup
    {
        private static readonly string BasePath = AppContext.BaseDirectory;
        private static readonly string DbFile    = Path.Combine(BasePath, "knowledge.db");
        private static readonly string SqlScript = Path.Combine(BasePath, "CreateDB.sql");
        private static bool _initialized;

        public static void Initialize()
        {
            if (_initialized) return;

            // Delete old DB so we always start clean
            if (File.Exists(DbFile))
                File.Delete(DbFile);

            // Read your fixed SQL script in full
            var script = File.ReadAllText(SqlScript);

            using var conn = new SQLiteConnection($"Data Source={DbFile};");
            conn.Open();

            // Execute the entire script (CREATE + multi-row INSERT) at once
            using var cmd = new SQLiteCommand(script, conn);
            cmd.ExecuteNonQuery();

            _initialized = true;
        }
    }
}



