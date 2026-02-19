using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VoltFlow.Service.API.Validator;
using VoltFlow.Service.Application.Commands.Auth;
using VoltFlow.Service.Application.Queries.Category;
using VoltFlow.Service.Application.Services;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Abstractions.Services;
using VoltFlow.Service.Core.Abstractions.Tools;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Models.email;
using VoltFlow.Service.Infrastructure.Data;
using VoltFlow.Service.Infrastructure.Handlers.Category;
using VoltFlow.Service.Infrastructure.JWT;
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
            // 1. Core & Infrastructure
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // 2. Database (Tylko raz!)
            services.AddDbContext<VoltFlowDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            // 3. Identity
            services.AddIdentityCore<User>(options =>
            {
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole<int>>()
            .AddEntityFrameworkStores<VoltFlowDbContext>()
            .AddSignInManager<SignInManager<User>>()
            .AddDefaultTokenProviders();

            // 4. MediatR & Validation Pipeline
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
                typeof(GetCategoriesQuery).Assembly,
                typeof(GetCategoriesHandlers).Assembly
            ));

            services.AddValidatorsFromAssembly(typeof(RegisterUserCommand).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            // 5. Authentication & JWT
            var secretKey = Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing");
            services.Configure<JwtOptions>(Configuration.GetSection("Jwt"));

            services.Configure<SmtpSettings>(Configuration.GetSection("SmtpSettings"));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
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

            // 6. Dependency Injection - Repositories
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IElementRepository, ElementRepository>();
            services.AddScoped<IElementGroupRepository, ElementGroupRepository>();
            services.AddScoped<ITaskEntityRepository, TaskEntityRepository>();
            services.AddScoped<ICatalogRepository, CatalogRepository>();
            services.AddScoped<IRoleRepository, RoleRespository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // 7. Dependency Injection - Services
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IElementService, ElementService>();
            services.AddScoped<IElementGroupService, ElementGroupService>();
            services.AddScoped<ITaskEntityService, TaskEntityService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmailRepository, EmailRepository>();

            services.AddScoped<IJWTProvider, JwtProvider>();
            
            // 8. CORS
            services.AddCors(c =>
            {
                c.AddPolicy("AllowAngularApp", options =>
                    options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "VoltFlow API V1");
                });
            }

            // Global Error Handling Middleware (Powinieneś go tu mieć!)
            // app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowAngularApp");

            // Automatyczne przekierowanie na Swaggera
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}