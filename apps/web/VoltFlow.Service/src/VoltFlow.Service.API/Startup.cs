using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VoltFlow.Service.Application.Queries.Category;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Infrastructure.Data;
using VoltFlow.Service.Infrastructure.Handlers.Category;
using VoltFlow.Service.Infrastructure.Repositories;


namespace VoltFlow.Service.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
            typeof(GetCategoriesQuery).Assembly,   // Automatycznie rejestruje wszystko z warstwy Application
            typeof(GetCategoriesHandlers).Assembly // Automatycznie rejestruje wszystko z warstwy Infrastructure
        ));

            // Upewnij się, że nazwa w appsettings to "DefaultConnection" czy "DataBase"
            services.AddDbContext<VoltFlowDbContext>(options =>
                     options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddCors(c =>
            {
                c.AddPolicy("AllowAngularApp", options =>
                    options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // Konfiguracja JWT
            var secretKey = Configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("Jwt:Key is missing in appsettings.json");
            }

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"] ?? "your-app",
                    ValidAudience = Configuration["Jwt:Audience"] ?? "your-app",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });

            services.AddAuthorization();

            // Rejestracja repozytoriów
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IElementRepository, ElementRepository>();

        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowAngularApp");

            // developer convenience: redirect root URL to Swagger UI
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/")
                {
                    context.Response.Redirect("/swagger");
                    return;
                }

                await next();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }

    }
}