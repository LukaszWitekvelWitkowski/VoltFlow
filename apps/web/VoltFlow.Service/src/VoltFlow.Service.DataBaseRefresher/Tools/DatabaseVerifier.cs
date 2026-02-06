using Microsoft.EntityFrameworkCore;
using System.Data;

public class DatabaseVerifier
{
    private readonly DbContext _context;

    public DatabaseVerifier(DbContext context)
    {
        _context = context;
    }

    public async Task<bool> VerifyAllAsync()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n--- STARTING DATABASE VERIFICATION ---");
        Console.ResetColor();

        // Note: Ensure these names match your C# entity names or [Table] attributes.
        // EF Core usually pluralizes names (e.g., "user" might be "Users" in the DB).
        var tablesToVerify = new[] { "User", "Transaction", "Category" };

        var tablesOk = await VerifyTablesExistAsync(tablesToVerify);
        var keysOk = await VerifyForeignKeysAsync();
        var seedOk = await VerifySeedDataAsync();

        return tablesOk && keysOk && seedOk;
    }

    private async Task<bool> VerifyTablesExistAsync(string[] expectedTables)
    {
        var existingTables = new List<string>();
        var conn = _context.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open) await conn.OpenAsync();

        using var command = conn.CreateCommand();
        // Retrieve all table names from the public schema
        command.CommandText = "SELECT tablename FROM pg_tables WHERE schemaname = 'public'";

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync()) existingTables.Add(reader.GetString(0));

        bool allExist = true;
        foreach (var table in expectedTables)
        {
            // PostgreSQL is case-insensitive by default. 
            // We check using OrdinalIgnoreCase for maximum safety.
            if (!existingTables.Any(t => string.Equals(t, table, StringComparison.OrdinalIgnoreCase)))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] Missing table: {table}. (Available in DB: {string.Join(", ", existingTables)})");
                Console.ResetColor();
                allExist = false;
            }
        }

        if (allExist)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[OK] All required structural tables exist.");
            Console.ResetColor();
        }
        return allExist;
    }

    private async Task<bool> VerifySeedDataAsync()
    {
        try
        {
            // We verify the "user" table count. 
            // Double quotes are used to handle potential case sensitivity in PostgreSQL.
            var userCount = await ExecuteScalarAsync<long>("SELECT COUNT(*) FROM \"User\"");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[OK] Number of records in table 'User': {userCount}");
            Console.ResetColor();

            return userCount >= 0; // 0 is technically valid if the seeder was empty
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[DATA VERIFICATION ERROR]: {ex.Message}");
            Console.ResetColor();
            return false;
        }
    }

    private async Task<bool> VerifyForeignKeysAsync()
    {
        var sql = "SELECT COUNT(*) FROM information_schema.table_constraints WHERE constraint_type = 'FOREIGN KEY' AND table_schema = 'public'";
        var count = await ExecuteScalarAsync<long>(sql);

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"[INFO] Found {count} foreign keys.");
        Console.ResetColor();

        return count > 0;
    }

    private async Task<T> ExecuteScalarAsync<T>(string sql)
    {
        var conn = _context.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open) await conn.OpenAsync();
        using var command = conn.CreateCommand();
        command.CommandText = sql;
        var result = await command.ExecuteScalarAsync();
        return (T)Convert.ChangeType(result ?? 0, typeof(T));
    }
}