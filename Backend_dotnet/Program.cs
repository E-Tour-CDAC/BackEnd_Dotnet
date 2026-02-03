using Backend_dotnet.Data;
using Backend_dotnet.Services;
using Backend_dotnet.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend_dotnet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 🔹 Read normal connection string
            var connectionString =
                builder.Configuration.GetConnectionString("DefaultConnection");

            

            Console.WriteLine("==== DB CONNECTION STRING CHECK ====");
            Console.WriteLine(connectionString);
            Console.WriteLine("===================================");

            // 🔹 Register DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(
                    connectionString,
                    ServerVersion.Parse("8.0.43-mysql")
                )
            );

            // 🔹 Register services
            builder.Services.AddScoped<ICategoryService, CategoryService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
