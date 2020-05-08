using Microsoft.EntityFrameworkCore;
using Nephthys.Api.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nephthys.Api.Entities
{
    public class NephthysDbContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        public DbSet<Order> Orders { get; set; }

        public NephthysDbContext(DbContextOptions<NephthysDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Customer>().HasKey(c => c.Id);

            //var data = new DataGenerator();
            //modelBuilder.Entity<Customer>().HasData(data.Customers);
            ////modelBuilder.Entity<Order>().HasData(data.Orders);

        }


    }
}
