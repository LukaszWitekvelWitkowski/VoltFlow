using Microsoft.EntityFrameworkCore;

namespace VoltFlow.Service.DataBaseRefresher.Tools
{
    public class DatabaseCleaner
    {
        private readonly DbContext _context;

        public DatabaseCleaner(DbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Main method to clean the database in the correct order.
        /// </summary>
        public void FullClean()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Starting deep cleanup of PostgreSQL database...");
            Console.ResetColor();

            DropForeignKeys();
            DropIndices();
            DropAllTables();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Database cleanup completed.");
            Console.ResetColor();
        }

        public void DropForeignKeys()
        {
            var sql = @"
            DO $$ DECLARE
                r RECORD;
            BEGIN
                FOR r IN (SELECT table_name, constraint_name
                          FROM information_schema.table_constraints
                          WHERE constraint_type = 'FOREIGN KEY' AND table_schema = 'public')
                LOOP
                    EXECUTE 'ALTER TABLE public.' || quote_ident(r.table_name) || ' DROP CONSTRAINT ' || quote_ident(r.constraint_name);
                END LOOP;
            END $$;";

            _context.Database.ExecuteSqlRaw(sql);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("- Foreign keys dropped.");
            Console.ResetColor();
        }

        public void DropIndices()
        {
            // Note: We skip primary key indexes (usually ending in _pkey)
            // and the __EFMigrationsHistory table indexes to avoid dependency errors.
            var sql = @"
            DO $$ DECLARE
                r RECORD;
            BEGIN
                FOR r IN (
                    SELECT indexname 
                    FROM pg_indexes 
                    WHERE schemaname = 'public' 
                    AND indexname NOT LIKE '%_pkey'
                    AND tablename <> '__EFMigrationsHistory'
                )
                LOOP
                    EXECUTE 'DROP INDEX IF EXISTS public.' || quote_ident(r.indexname);
                END LOOP;
            END $$;";

            _context.Database.ExecuteSqlRaw(sql);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("- Auxiliary indexes dropped.");
            Console.ResetColor();
        }

        public void DropAllTables()
        {
            // Dropping tables here will automatically remove associated _pkey indexes.
            var sql = @"
            DO $$ DECLARE
                r RECORD;
            BEGIN
                FOR r IN (
                    SELECT tablename 
                    FROM pg_tables 
                    WHERE schemaname = 'public' 
                    AND tablename <> '__EFMigrationsHistory'
                )
                LOOP
                    EXECUTE 'DROP TABLE IF EXISTS public.' || quote_ident(r.tablename) || ' CASCADE';
                END LOOP;
            END $$;";

            _context.Database.ExecuteSqlRaw(sql);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("- All user tables dropped.");
            Console.ResetColor();
        }
    }
}