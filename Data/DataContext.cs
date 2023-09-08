using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SpaBookingApp.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<SpaProduct> SpaProducts { get; set; }
        public DbSet<Provision> Provisions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        public DbSet<ProvisionBooking> ProvisionBookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SpaProduct>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Provision>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SpaProduct>()
                .HasOne(p => p.Category)
                .WithMany(c => c.SpaProducts)
                .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<Order>()
                .Property(o => o.SubTotal)
                .HasColumnType("decimal(16, 2)"); // Chỉnh sửa kiểu cột tương ứng

            modelBuilder.Entity<Order>()
                .Property(o => o.ShippingFee)
                .HasColumnType("decimal(16, 2)"); // Chỉnh sửa kiểu cột tương ứng

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalPrice)
                .HasColumnType("decimal(16, 2)");

        }
    }
}
