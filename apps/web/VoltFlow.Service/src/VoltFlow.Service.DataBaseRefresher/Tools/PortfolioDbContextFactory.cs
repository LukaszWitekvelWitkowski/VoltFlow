using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using VoltFlow.Service.Infrastructure.Data;

namespace VoltFlow.Service.DataBaseRefresher.Tools
{
    public class PortfolioDbContextFactory : IDesignTimeDbContextFactory<PortfolioDbContext>
    {
        public PortfolioDbContext CreateDbContext(string[] args)
        {

            // 1. Calculate the path to the API project
            // Assume the structure: /SolutionFolder/YourProject and /SolutionFolder/VoltFlow.Service.API
            var currentDir = Directory.GetCurrentDirectory();

            // Look for the solution folder (e.g., start from bin/Debug/net8.0, 3-4 levels up)
            // It's safer to search for the folder by the API project name
            var apiProjectPath = Path.GetFullPath(Path.Combine(currentDir, "..", "..", "..", "..", "VoltFlow.Service.API"));

            // If the above doesn't work (depends on your structure), you can provide an absolute path for testing
            // or use a more sophisticated parent folder lookup.

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(apiProjectPath) // <--- Here we indicate the API project folder
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var builder = new DbContextOptionsBuilder<PortfolioDbContext>();
            var connectionString = configuration.GetConnectionString("AdminConnection");

            builder.UseNpgsql(connectionString);

            return new PortfolioDbContext(builder.Options);
        }

        //         We add a helper async method to your Refresh method
        public async Task<PortfolioDbContext> CreateDbContextAsync(string[] args)
        {
            return await Task.Run(() => CreateDbContext(args));
        }
    }
}