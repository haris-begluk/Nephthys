using Bogus;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nephthys.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nephthys.Api.Helpers
{
    public static class DataGenerator
    {
        public static IEnumerable<Customer> Customers { get; set; }
        public static IEnumerable<Order> Orders { get; set; }
        public static void MigrateAndGenerateData(this IApplicationBuilder app)
        {
            //Randomizer.Seed = new Random(44141411);




            app.ExecuteMigrations<NephthysDbContext>();
            app.SeedData();

        }
        internal static void SeedData(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<NephthysDbContext>();
                if (!context.Customers.Any())
                {
                    var customerGenerator = new Faker<Customer>()
                        //.RuleFor(c => c.Id, Guid.NewGuid())
                        .RuleFor(c => c.Name, f => f.Company.CompanyName())
                        .RuleFor(c => c.Address, f => f.Address.FullAddress())
                        .RuleFor(c => c.City, f => f.Address.City())
                        .RuleFor(c => c.Country, f => f.Address.Country())
                        .RuleFor(c => c.ZipCode, f => f.Address.ZipCode())
                        .RuleFor(c => c.Phone, f => f.Phone.PhoneNumber())
                        .RuleFor(c => c.Email, f => f.Internet.Email())
                        .RuleFor(c => c.ContactName, (f, c) => f.Name.FullName());
                    Customers = customerGenerator.Generate(1000);
                    Console.WriteLine("Customers being populated");
                    //context.Customers.AddRange(Customers);
                    foreach (var client in Customers)
                    {
                        context.Customers.Add(client);
                    }
                    context.SaveChanges();
                }
                else
                {
                    Console.WriteLine("Customers already populated");
                }
                if (!context.Orders.Any())
                {
                    Console.WriteLine("Orders being populated");
                    var ordergenerator = new Faker<Order>()
                       //.RuleFor(o => o.Id, Guid.NewGuid())
                       .RuleFor(o => o.CustomerId, f => { return GetRandomCustomerId(context.Customers.ToList()); })
                       .RuleFor(o => o.Date, f => f.Date.Past(3))
                       .RuleFor(o => o.OrderValue, f => f.Finance.Amount(0, 10000))
                       .RuleFor(o => o.Shipped, f => f.Random.Bool(0.9f));
                    Orders = ordergenerator.Generate(1000);
                    context.Orders.AddRange(Orders);
                    context.SaveChanges();
                }
                else
                {
                    Console.WriteLine("Orders already populated");
                }

            }
        }
        internal static void ExecuteMigrations<T>(this IApplicationBuilder app) where T : DbContext
        {

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<T>();
                context.Database.Migrate();

            }
        }
        internal static Guid GetRandomCustomerId(IEnumerable<Customer> source)
        {
            var random = new Random();
            return source.ToList()[random.Next(0, source.Count())].Id;
        }
    }

}
