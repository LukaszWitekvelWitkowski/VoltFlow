using Microsoft.EntityFrameworkCore;
using VoltFlow.Service.DataBaseRefresher.Tools;

namespace VoltFlow.Service.DataBaseRefresher
{
    public static class DatabaseRefresher
    {
        public static async Task Main(string[] args)
        {
            try
            {
                await RefreshDatabaseAsync();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Database has been successfully refreshed.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An error occurred while refreshing the database: {ex.Message}");
                Console.ResetColor();
            }
        }

        private static async Task RefreshDatabaseAsync()
        {
            // Initialize the DbContext factory
            var factory = new PortfolioDbContextFactory();
            using var context = await factory.CreateDbContextAsync(Array.Empty<string>());

            // Initialize the database cleaner with the current context
            var cleaner = new DatabaseCleaner(context);

            // 1. Full database cleanup (dropping tables, constraints, etc.)
            cleaner.FullClean();

            // 2. Apply migrations to ensure the schema matches the current model
            await context.Database.MigrateAsync();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Generating schema script...");
            Console.ResetColor();

            var sqlScript = context.Database.GenerateCreateScript();
            await File.WriteAllTextAsync("last_schema.sql", sqlScript);

            try
            {
                Console.WriteLine("Applying structure to the database...");

                // Execute the generated script to verify its correctness
                await context.Database.ExecuteSqlRawAsync(sqlScript);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Success: Database structure created correctly.");
                Console.ResetColor();

                // 3. Seed data from SQL files
                await DataSeeder.SeedAsync(context);

                // 4. Run consistency and health checks
                var verifier = new DatabaseVerifier(context);
                bool isHealthy = await verifier.VerifyAllAsync();

                if (!isHealthy)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Database failed consistency tests!");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"CRITICAL ERROR during structure creation: {ex.Message}");
                Console.ResetColor();

                // Re-throw to stop the process if the structure fails
                throw;
            }
        }
    }
}