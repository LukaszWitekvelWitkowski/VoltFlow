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

        public void FullClean()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Starting deep cleanup of PostgreSQL database...");
            Console.ResetColor();

            // PostgreSQL sometimes requires resetting the connection pool after drastic schema changes
            var conn = _context.Database.GetDbConnection();
            if (conn.State == System.Data.ConnectionState.Open) conn.Close();

            DropForeignKeys();
            DropIndices();
            DropAllTables();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Database cleanup completed.");
            Console.ResetColor();
        }

        public void DropForeignKeys()
        {
            // We add quote_ident around the table name and schema
            var sql = @"
            DO $$ DECLARE
                r RECORD;
            BEGIN
                FOR r IN (SELECT table_name, constraint_name, table_schema
                          FROM information_schema.table_constraints
                          WHERE constraint_type = 'FOREIGN KEY' AND table_schema = 'public')
                LOOP
                    EXECUTE 'ALTER TABLE ' || quote_ident(r.table_schema) || '.' || quote_ident(r.table_name) || 
                            ' DROP CONSTRAINT ' || quote_ident(r.constraint_name);
                END LOOP;
            END $$;";

            _context.Database.ExecuteSqlRaw(sql);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("- Foreign keys dropped.");
            Console.ResetColor();
        }

        public void DropIndices()
        {
            var sql = @"
                DO $$ DECLARE
                    r RECORD;
                BEGIN
                    FOR r IN (
                        SELECT indexname, schemaname
                        FROM pg_indexes 
                        WHERE schemaname = 'public' 
                        AND indexname NOT ILIKE '%_pkey'
                        AND indexname NOT ILIKE 'pk_%'
                        AND tablename <> '__EFMigrationsHistory'
                    )
                    LOOP
                        EXECUTE 'DROP INDEX IF EXISTS ' || quote_ident(r.schemaname) || '.' || quote_ident(r.indexname);
                    END LOOP;
                END $$;";

            _context.Database.ExecuteSqlRaw(sql);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("- Auxiliary indexes dropped.");
            Console.ResetColor();
        }
        public void DropAllTables()
        {
            var sql = @"
            DO $$ DECLARE
                r RECORD;
            BEGIN
                FOR r IN (
                    SELECT tablename, schemaname
                    FROM pg_tables 
                    WHERE schemaname = 'public' 
                    AND tablename <> '__EFMigrationsHistory'
                )
                LOOP
                    EXECUTE 'DROP TABLE IF EXISTS ' || quote_ident(r.schemaname) || '.' || quote_ident(r.tablename) || ' CASCADE';
                END LOOP;
            END $$;";

            _context.Database.ExecuteSqlRaw(sql);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("- All user tables dropped.");
            Console.ResetColor();
        }
    }
}