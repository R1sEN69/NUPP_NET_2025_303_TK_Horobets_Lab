using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using Transport.Infrastructure; // Для TransportContext
using Transport.Infrastructure.Interfaces;
using Transport.Infrastructure.Repositories;
using Transport.Infrastructure.Services;
using Transport.Infrastructure.Models; // Для CarModel

namespace Transport.App // Назва простору імен вашого головного проекту
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    string? connectionString = context.Configuration.GetConnectionString("DefaultConnection");
                    if (string.IsNullOrEmpty(connectionString))
                    {
                        Console.WriteLine("Connection string 'DefaultConnection' не знайдена в appsettings.json. Використовую default: Data Source=transport.db");
                        connectionString = "Data Source=transport.db";
                    }

                    // Реєстрація DbContext
                    services.AddDbContext<TransportContext>(options =>
                        options.UseSqlite(connectionString));

                    // Реєстрація IRepository та Repository
                    services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

                    // Реєстрація ICrudServiceAsync та CrudServiceAsync
                    services.AddScoped(typeof(ICrudServiceAsync<>), typeof(CrudServiceAsync<>));

                    // Реєстрація класу Application
                    services.AddTransient<Application>(); // Application повинен бути Transient або Scoped
                })
                .Build();

            // Отримання сервісу Application з контейнера і його запуск
            var app = host.Services.GetRequiredService<Application>();
            await app.Run();
        }
    }
}