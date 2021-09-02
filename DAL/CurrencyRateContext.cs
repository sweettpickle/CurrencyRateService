using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CurrencyExchangeRateService.DAL
{
    public class CurrencyRateContext : DbContext
    {
        public DbSet<Valute> Valute { get; set; }
        public DbSet<CurrencyRate> CurrencyRate { get; set; }

        public CurrencyRateContext() 
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=CurrencyRate;Username=postgres;Password=root");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Valute>();
            modelBuilder.Entity<CurrencyRate>();
        }
    }
}
