using Microsoft.EntityFrameworkCore;
using System.Text;

namespace VoltFlow.Service.DataBaseRefresher.Tools
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(DbContext context)
        {

            // ENCODING REGISTRATION (Required for Encoding 1250)
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string seedFolderPath = Path.Combine(baseDir, "SeedScripts");

            if (!Directory.Exists(seedFolderPath))
            {
                string altPath = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "SeedScripts"));
                if (Directory.Exists(altPath)) seedFolderPath = altPath;
                else return;
            }

            // First we repair all files in the folder
            FixAllSqlFilesEncoding(seedFolderPath);

            var sqlFiles = Directory.GetFiles(seedFolderPath, "*.sql")
                                    .OrderBy(f => f)
                                    .ToList();

            foreach (var file in sqlFiles)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Executing: {Path.GetFileName(file)}");
                Console.ResetColor();

                string sql = await File.ReadAllTextAsync(file, Encoding.UTF8);

                using var transaction = await context.Database.BeginTransactionAsync();
                try
                {
                    await context.Database.ExecuteSqlRawAsync(sql);
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[SQL ERROR] File {Path.GetFileName(file)}: {ex.Message}");
                    Console.ResetColor();
                    throw;
                }
            }
        }

        public static void FixAllSqlFilesEncoding(string folderPath)
        {
     
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var win1250 = Encoding.GetEncoding(1250);

            var utf8WithoutBom = new UTF8Encoding(false);

            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine($"[ERROR] Folder {folderPath} does not exist.");
                return;
            }

            var files = Directory.GetFiles(folderPath, "*.sql");
            foreach (var file in files)
            {
                byte[] rawBytes = File.ReadAllBytes(file);

                // 1. We check if it is already valid UTF-8
                if (IsUtf8(rawBytes))
                {
                    // Optional: let's check if the file has any strings from previous incorrect conversions.
                    // If the file already contains strings like "ÄÂ", IsUtf8 will return true,
                    // because it's technically correct (though visually incorrect) UTF-8.
                    string currentContent = Encoding.UTF8.GetString(rawBytes);

                    if (currentContent.Contains("Ă"))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[WARNING] {Path.GetFileName(file)} seems corrupted (double encoded). Please restore from backup!");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"[SKIP] {Path.GetFileName(file)} is already correct UTF-8.");
                    }
                    continue;
                }

                // 2. If it is not UTF-8, we convert from 1250
                try
                {
                    string content = win1250.GetString(rawBytes);
                    File.WriteAllText(file, content, utf8WithoutBom);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[REPAIRED] {Path.GetFileName(file)} converted from Windows-1250 to UTF-8");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Could not repair {Path.GetFileName(file)}: {ex.Message}");
                }
            }
        }

        private static bool IsUtf8(byte[] bytes)
        {
            try
            {
                var utf8 = new UTF8Encoding(false, true);
                utf8.GetString(bytes);
                return true;
            }
            catch (ArgumentException)
            {

                return false;
            }
        }
    }
}