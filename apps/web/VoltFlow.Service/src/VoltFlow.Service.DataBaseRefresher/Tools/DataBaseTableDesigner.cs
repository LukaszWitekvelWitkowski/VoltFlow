namespace VoltFlow.Service.DataBaseRefresher.Tools
{
    public class DataBaseTableDesigner
    {

        public static string TargetCollation = "pl-PL-x-icu";

        public static string GenerateCollationUpdateScript(string tableName, params string[] columnNames)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[Config] Preparing collation change for table: {tableName}...");
            Console.ResetColor();

            // Przygotowanie listy kolumn do SQL
            string columnsFormatted = string.Join("', '", columnNames);

            return $@"
            DO $$ 
            DECLARE
                col_name TEXT;
                cols_to_change TEXT[] := ARRAY['{columnsFormatted}'];
            BEGIN
                FOREACH col_name IN ARRAY cols_to_change
                LOOP
                    -- Ważne: quote_ident chroni nazwy, TargetCollation idzie na koniec
                    EXECUTE 'ALTER TABLE ' || quote_ident('{tableName}') || 
                            ' ALTER COLUMN ' || quote_ident(col_name) || 
                            ' TYPE VARCHAR COLLATE ' || quote_ident('{TargetCollation}');
                END LOOP;
            END $$;";
        }
        public static string GenerateIndexesScript(string tableName, string[] columnNames)
        {
            // We combine the column names into a format understandable for a text array in Postgres
            string columnsFormatted = string.Join("', '", columnNames);

            return $@"
            DO $$ 
            DECLARE
                col_name TEXT;
                index_name TEXT;
                cols_to_index TEXT[] := ARRAY['{columnsFormatted}'];
            BEGIN
                FOREACH col_name IN ARRAY cols_to_index
                LOOP
                    -- Dynamiczne tworzenie nazwy indeksu, np. idx_uzytkownicy_nazwisko
                    index_name := 'idx_' || '{tableName}' || '_' || col_name;

                    -- Sprawdzenie czy indeks już istnieje, aby uniknąć błędów
                    IF NOT EXISTS (SELECT 1 FROM pg_class c JOIN pg_namespace n ON n.oid = c.relnamespace 
                                   WHERE c.relname = index_name AND n.nspname = 'public') THEN
                        
                        EXECUTE 'CREATE INDEX ' || quote_ident(index_name) || 
                                ' ON ' || quote_ident('{tableName}') || 
                                ' (' || quote_ident(col_name) || ')';
                                
                        RAISE NOTICE 'Utworzono indeks: %', index_name;
                    ELSE
                        RAISE NOTICE 'Indeks % juz istnieje, pomijam.', index_name;
                    END IF;
                END LOOP;
            END $$;";
        }
    }
}
