using Microsoft.EntityFrameworkCore;
using SensorizMonitoring.Data.Models;

namespace SensorizMonitoring.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Monitoring> Monitoring { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Branch> Branch { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Device> Device { get; set; }
        public DbSet<NotificationSettings> NotificationSettings { get; set; }
        public DbSet<NotificationOwner> NotificationOwner { get; set; }
        public DbSet<SensorType> SensorType { get; set; }
        public DbSet<NotificationLog> NotificationLog { get; set; }
        public DbSet<DeviceReference> DeviceReference { get; set; }
        public DbSet<NotificationControl> NotificationControl { get; set; }
        public DbSet<Role> Role { get; set; }

    }
}