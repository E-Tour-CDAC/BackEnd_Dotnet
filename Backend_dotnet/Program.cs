using Backend_dotnet.Configuration;
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

namespace Backend_dotnet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ================= LOGGING =================
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .WriteTo.Console()
                .WriteTo.File("D://etour-.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog();

            // ================= DATABASE =================
            DotNetEnv.Env.Load();

            var dbUser = Environment.GetEnvironmentVariable("DB_USER");
            var dbPassword = Environment.GetEnvironmentVariable("DB_PASS");

            if (string.IsNullOrWhiteSpace(dbUser) || string.IsNullOrWhiteSpace(dbPassword))
            {
                throw new Exception("Database environment variables missing");
            }

            var connectionString =
                $"server=localhost;database=e_tour;user={dbUser};password={dbPassword}";

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(
                    connectionString,
                    ServerVersion.Parse("8.0.43-mysql")
                )
            );

            // ================= AUTOMAPPER =================
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

            // ================= REPOSITORIES =================
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

            // ================= SERVICES =================
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IPaymentGatewayService, RazorpayService>();

            // ================= RAZORPAY CONFIG =================
            builder.Services.Configure<RazorpayOptions>(
                builder.Configuration.GetSection("Razorpay")
            );

            // ================= HELPERS =================
            builder.Services.AddScoped<EmailHelper>();
            builder.Services.AddScoped<ImageHelper>();

            // ================= CORS =================
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // ================= MVC & SWAGGER =================
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "E-Tour API",
                    Version = "v1"
                });
            });

            var app = builder.Build();

            // ================= MIDDLEWARE =================
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseMiddleware<LoggingMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("AllowAll");

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
