using Backend_dotnet.Data;
using Backend_dotnet.Middleware;
using Backend_dotnet.Repositories.Implementations;
using Backend_dotnet.Repositories.Interfaces;
using Backend_dotnet.Services;
using Backend_dotnet.Services.Implementations;
using Backend_dotnet.Services.Interfaces;
using Backend_dotnet.Utilities.Helpers;
using Backend_dotnet.Utilities.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Xml.Linq;

using Backend_dotnet.Services.Implementations;

namespace Backend_dotnet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ===== SERILOG LOGGING CONFIGURATION =====
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .WriteTo.Console()
                .WriteTo.File("D://etour-.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog();


            // ===== DATABASE CONFIGURATION =====
            DotNetEnv.Env.Load();
            var dbUser = Environment.GetEnvironmentVariable("DB_USER");
            var dbPassword = Environment.GetEnvironmentVariable("DB_PASS");

            if (string.IsNullOrWhiteSpace(dbUser))
            {
                throw new Exception("Database environment variables are missing");
            }

            var connectionString =
                $"server=localhost;database=e_tour;user={dbUser};password={dbPassword}";

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(
                    connectionString,
                    ServerVersion.Parse("8.0.43-mysql")
                )
            );

            // ===== AUTOMAPPER =====
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

            // ===== REPOSITORIES (GENERIC FIRST, THEN SPECIFIC) =====
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // 🔹 Register services
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<IBookingService, BookingService>();

            // Tour Module
            builder.Services.AddScoped<ITourRepository, TourRepository>();
            builder.Services.AddScoped<ITourService, TourService>();

            // ===== HELPERS =====
            builder.Services.AddScoped<EmailHelper>();
            builder.Services.AddScoped<ImageHelper>();

            // ===== CORS =====
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "E-Tour API",
                    Version = "v1",
                    Description = "E-Tour Booking System API",
                    Contact = new OpenApiContact
                    {
                        Name = "E-Tour Team",
                        Email = "etourvirtugo@gmail.com"
                    }
                });
            });



            // =========================
            // 🔹 CONTROLLERS + JSON (IMPORTANT)
            // =========================
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // 🔥 REQUIRED for Java DTO mapping
                    options.JsonSerializerOptions.PropertyNamingPolicy =
                        System.Text.Json.JsonNamingPolicy.CamelCase;
                });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();



            // =========================
            // 🔹 HTTP CLIENT (JAVA BACKEND)
            // =========================
            builder.Services.AddHttpClient("AuthService", client =>
            {
                client.BaseAddress = new Uri("http://localhost:8080/");
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            });

            // =========================
            // 🔹 SERVICES
            // =========================
            builder.Services.AddScoped<AuthService>(); // 🔥 CALLS JAVA AUTH

            var app = builder.Build();

                // 1. Exception Handling (FIRST!)
                app.UseMiddleware<ExceptionHandlingMiddleware>();

                // 2. Logging
                app.UseMiddleware<LoggingMiddleware>();

                // 3. Request/Response Logging (Development only)
                if (app.Environment.IsDevelopment())
                {
                    app.UseMiddleware<RequestResponseLoggingMiddleware>();
                }

                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();
                // 6. Static Files
                app.UseStaticFiles();

                // 7. CORS
                app.UseCors("AllowAll");
            //app.UseAuthentication();
            //app.UseAuthorization();
                app.MapControllers();
                app.Run();
            }
            }
    }

