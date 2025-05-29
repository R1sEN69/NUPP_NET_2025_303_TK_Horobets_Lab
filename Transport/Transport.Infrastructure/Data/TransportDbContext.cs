using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Data
{

    public class TransportDbContext : IdentityDbContext<ApplicationUser>
    {

        public TransportDbContext(DbContextOptions<TransportDbContext> options)
            : base(options)
        {
        }

        public DbSet<Vehicle> Vehicles { get; set; } = null!; 

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

        }
    }
}