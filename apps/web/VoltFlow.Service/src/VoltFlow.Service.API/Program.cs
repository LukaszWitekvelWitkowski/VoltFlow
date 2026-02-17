using Serilog;
using Serilog.Events;
using VoltFlow.Service.API;

public class Program
{
    public static void Main(string[] args)
    {
        // 1. Konfiguracja loggera "na start"
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("logs/crm-log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            Log.Information("Uruchamianie aplikacji VoltFlow...");
            var host = CreateHostBuilder(args).Build();
            host.Run();
        }
        catch (Exception ex)
        {
            // U¿ywamy Log.Fatal zamiast Console.WriteLine
            Log.Fatal(ex, "Aplikacja nie mog³a wystartowaæ!");
        }
        finally
        {
            // Wa¿ne: czyœcimy zasoby i upewniamy siê, ¿e wszystkie logi zosta³y zapisane na dysk
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}