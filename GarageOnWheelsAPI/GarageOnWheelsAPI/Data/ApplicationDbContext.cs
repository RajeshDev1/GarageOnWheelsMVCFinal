using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Proxies;
using GarageOnWheelsAPI.Models.DatabaseModels;

namespace GarageOnWheelsAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Garage> Garages { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<LoggerEntry> Loggers { get; set; }

        public virtual DbSet<OrderFiles> OrderFiles { get; set; }

        public virtual DbSet<Otp> Otps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Order entity
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");



            modelBuilder.Entity<User>()
                .HasOne(u => u.Country)
                .WithMany()
                .HasForeignKey(u => u.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.State)
                .WithMany()
                .HasForeignKey(u => u.StateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.City)
                .WithMany()
                .HasForeignKey(u => u.CityId)
                .OnDelete(DeleteBehavior.Restrict);

           /* modelBuilder.Entity<User>()
                .HasOne(u => u.Area)
                .WithMany()
                .HasForeignKey(u => u.AreaId)
                .OnDelete(DeleteBehavior.Restrict);*/

            // Configure Garage entity
            modelBuilder.Entity<Garage>()
                .HasOne(g => g.Country)
                .WithMany()
                .HasForeignKey(g => g.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Garage>()
                .HasOne(g => g.State)
                .WithMany()
                .HasForeignKey(g => g.StateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Garage>()
                .HasOne(g => g.City)
                .WithMany()
                .HasForeignKey(g => g.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Garage>()
                .HasOne(g => g.Area)
                .WithMany()
                .HasForeignKey(g => g.AreaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Garage>()
                .HasOne(g => g.User)
                .WithMany(u => u.Garages)
                .HasForeignKey(g => g.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            
        }

        // Enable lazy loading
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
