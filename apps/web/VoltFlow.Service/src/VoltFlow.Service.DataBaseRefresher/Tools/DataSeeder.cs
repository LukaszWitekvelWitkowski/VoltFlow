using Microsoft.EntityFrameworkCore;

namespace VoltFlow.Service.DataBaseRefresher.Tools
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(DbContext context)
        {
            // Get the path to the directory where the executable (.exe) is located
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string seedFolderPath = Path.Combine(baseDir, "SeedScripts");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Searching for scripts in: {seedFolderPath}");
            Console.ResetColor();

            if (!Directory.Exists(seedFolderPath))
            {
                // Fallback logic: if not in bin, check 3 levels up (for VS Debug mode)
                string altPath = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "SeedScripts"));
                if (Directory.Exists(altPath))
                {
                    seedFolderPath = altPath;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[ERROR] SeedScripts folder does not exist in any expected location.");
                    Console.ResetColor();
                    return;
                }
            }

            var sqlFiles = Directory.GetFiles(seedFolderPath, "*.sql")
                                    .OrderBy(f => f)
                                    .ToList();

            if (!sqlFiles.Any())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No .sql files found in the SeedScripts folder.");
                Console.ResetColor();
                return;
            }

            foreach (var file in sqlFiles)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Executing: {Path.GetFileName(file)}");
                Console.ResetColor();

                string sql = await File.ReadAllTextAsync(file);

                using var transaction = await context.Database.BeginTransactionAsync();
                try
                {
                    await context.Database.ExecuteSqlRawAsync(sql);
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    // Implicit rollback occurs on Dispose, but explicit rollback is clearer
                    await transaction.RollbackAsync();

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[SQL ERROR] File {Path.GetFileName(file)}: {ex.Message}");
                    Console.ResetColor();

                    // Re-throw to prevent the Verifier from reporting false successes
                    throw;
                }
            }
        }
    }
}