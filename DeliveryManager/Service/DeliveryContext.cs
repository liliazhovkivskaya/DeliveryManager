using DeliveryManager.Models.Entity;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DeliveryManager.Service
{
    public class DeliveryContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }  // Таблица заказов
        public DbSet<Log> Logs { get; set; }      // Таблица логов

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=deliverydb.db");
            }
        }

        public DeliveryContext(DbContextOptions<DeliveryContext> options)
            : base(options)
        {
        }
    }
}
