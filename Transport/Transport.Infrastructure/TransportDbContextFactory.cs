using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using Transport.Infrastructure.Data; 

namespace Transport.Infrastructure.Data 
{
    public class TransportDbContextFactory : IDesignTimeDbContextFactory<TransportDbContext>
    {
        public TransportDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            }

            var builder = new DbContextOptionsBuilder<TransportDbContext>();
            builder.UseSqlServer(connectionString);

            return new TransportDbContext(builder.Options);
        }
    }
}