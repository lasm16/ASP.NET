using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;

namespace PromoCodeFactory.DataAccess.Persistense
{
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
    {
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Preference> Preferences => Set<Preference>();
        public DbSet<PromoCode> PromoCodes => Set<PromoCode>();
        public DbSet<CustomerPreference> CustomerPreferences => Set<CustomerPreference>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureRole(modelBuilder);
            ConfigureEmployee(modelBuilder);
            ConfigurePreference(modelBuilder);
            ConfigureCustomer(modelBuilder);
            ConfigureCustomerPreference(modelBuilder);
            ConfigurePromoCode(modelBuilder);
        }

        private static void ConfigurePromoCode(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PromoCode>(entity =>
            {
                entity.ToTable("PromoCodes");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
                entity.Property(x => x.ServiceInfo).HasMaxLength(200).IsRequired();
                entity.Property(x => x.PartnerName).HasMaxLength(100).IsRequired();

                entity.HasOne(x => x.Preference)
                      .WithMany()
                      .HasForeignKey(x => x.PreferenceId);

                entity.HasOne(x => x.PartnerManager)
                      .WithMany()
                      .HasForeignKey(x => x.PartnerManagerId)
                      .OnDelete(DeleteBehavior.Cascade);

                var data = FakeDataFactory.PromoCodes;
                entity.HasData(data);
            });
        }

        private static void ConfigureCustomerPreference(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerPreference>(entity =>
            {
                entity.ToTable("CustomerPreferences");

                entity.HasKey(x => new { x.CustomerId, x.PreferenceId });

                entity.HasOne(x => x.Customer)
                      .WithMany(c => c.CustomerPreferences)
                      .HasForeignKey(x => x.CustomerId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Preference)
                      .WithMany(p => p.CustomerPreferences)
                      .HasForeignKey(x => x.PreferenceId)
                      .OnDelete(DeleteBehavior.Cascade);

                var data = FakeDataFactory.CustomerPreferences;
                entity.HasData(data);
            });
        }

        private static void ConfigureCustomer(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customers");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(x => x.LastName).HasMaxLength(100).IsRequired();
                entity.Property(x => x.Email).HasMaxLength(200).IsRequired();

                var data = FakeDataFactory.Customers;
                entity.HasData(data);
            });
        }

        private static void ConfigurePreference(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Preference>(entity =>
            {
                entity.ToTable("Preferences");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name).HasMaxLength(100).IsRequired();

                var data = FakeDataFactory.Preferences;
                entity.HasData(data);
            });
        }

        private static void ConfigureEmployee(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employees");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(x => x.LastName).HasMaxLength(100).IsRequired();
                entity.Property(x => x.Email).HasMaxLength(150).IsRequired();

                entity.HasOne(x => x.Role)
                      .WithMany(r => r.Employees)
                      .HasForeignKey(x => x.RoleId);

                var data = FakeDataFactory.Employees;
                entity.HasData(data);
            });
        }

        private static void ConfigureRole(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");

                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name).HasMaxLength(50).IsRequired();
                entity.Property(x => x.Description).HasMaxLength(200);

                var data = FakeDataFactory.Roles;
                entity.HasData(data);
            });
        }
    }
}
