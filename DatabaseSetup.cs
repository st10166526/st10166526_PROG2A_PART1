using System;
using System.Data.SQLite;
using System.IO;

namespace CyberSecurityBot
{
    public static class DatabaseSetup
    {
    private const string DbFile = "knowledge.db";
    private const string SqlScript = "CreateDB.sql";

    public static void Initialize()
{
    var needsSeeding = !File.Exists(DbFile)
                   || !TableExists("Knowledge");

    if (!needsSeeding)
        return;

    var script = File.ReadAllText(SqlScript);
    using var conn = new SQLiteConnection($"Data Source={DbFile};");
    conn.Open();
    using var cmd = new SQLiteCommand(script, conn);
    cmd.ExecuteNonQuery();
}

private static bool TableExists(string tableName)
{
    using var conn = new SQLiteConnection($"Data Source={DbFile};");
    conn.Open();

    using var cmd = conn.CreateCommand();
    cmd.CommandText = 
      "SELECT count(*) FROM sqlite_master WHERE type='table' AND name=@t;";
    cmd.Parameters.AddWithValue("@t", tableName);

    return Convert.ToInt32(cmd.ExecuteScalar()!) > 0;
}
    }
}
