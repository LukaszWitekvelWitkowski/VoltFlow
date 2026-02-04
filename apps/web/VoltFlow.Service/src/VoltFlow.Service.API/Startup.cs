using Microsoft.OpenApi;
using System.Data;
using Microsoft.Data.SqlClient;


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


            services.AddScoped<IDbConnection>(sp => new SqlConnection(Configuration.GetSection("ConnectionStrings:DataBase").Value!));




            services.AddCors(c =>
            {
                c.AddPolicy("AllowAngularApp", options =>
                                                         options.AllowAnyOrigin()
                                                                .AllowAnyHeader()
                                                                .AllowAnyMethod()
                 );
            });

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });
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
