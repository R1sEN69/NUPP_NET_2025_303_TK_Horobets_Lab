using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using Transport.Infrastructure.Models;

namespace Transport.Infrastructure
{
    public class TransportContext : DbContext
    {
        public DbSet<VehicleModel> Vehicles { get; set; }
        public DbSet<CarModel> Cars { get; set; }
        public DbSet<BusModel> Buses { get; set; }
        public DbSet<TrainModel> Trains { get; set; }
        public DbSet<EngineModel> Engines { get; set; }
        public DbSet<DriverModel> Drivers { get; set; }
        public DbSet<TripModel> Trips { get; set; }
        public DbSet<PassengerModel> Passengers { get; set; }

       
        public TransportContext(DbContextOptions<TransportContext> options) : base(options)
        {
        }


        public TransportContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            if (!optionsBuilder.IsConfigured)
            {

                optionsBuilder.UseSqlite("Data Source=transport.db");
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<VehicleModel>().ToTable("Vehicles");
            modelBuilder.Entity<CarModel>().ToTable("Cars");
            modelBuilder.Entity<BusModel>().ToTable("Buses");
            modelBuilder.Entity<TrainModel>().ToTable("Trains");

            modelBuilder.Entity<VehicleModel>()
                .HasOne(v => v.Engine)
                .WithOne(e => e.Vehicle)
                .HasForeignKey<VehicleModel>(v => v.EngineId)
                .IsRequired(false);

            
            modelBuilder.Entity<VehicleModel>()
                .HasMany(v => v.Drivers)
                .WithMany(d => d.Vehicles)
                .UsingEntity(j => j.ToTable("VehicleDrivers"));

            modelBuilder.Entity<TripModel>()
                .HasMany(t => t.Passengers)
                .WithOne(p => p.Trip)
                .HasForeignKey(p => p.TripId);

            modelBuilder.Entity<VehicleModel>()
                .Property(v => v.Brand)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<DriverModel>()
                .Property(d => d.LicenseNumber)
                .HasMaxLength(20)
                .IsRequired();

            base.OnModelCreating(modelBuilder);
        }
    }
}