using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nephthys.Admin.Data.Entities;
using Nephthys.Auth;
using System;
using System.Linq;
using System.Security.Claims;

namespace Nephthys.Admin.Data.DataHelpers
{
    public static class MigrateDatabase
    {
        public static void MigrateWithData(this IApplicationBuilder app)
        {
            Console.WriteLine("Start migrations...");
            app.ExecuteMigrations<ConfigurationDbContext>();
            app.ExecuteMigrations<PersistedGrantDbContext>();
            app.ExecuteMigrations<ApplicationDbContext>();
            Console.WriteLine("Migrations finished...");
            Console.WriteLine("Seeding database...");
            app.SeedData();
            Console.WriteLine("Done seeding database.");
        }
        public static void ExecuteMigrations<T>(this IApplicationBuilder app) where T : DbContext
        {

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<T>();
                context.Database.Migrate();

            }
        }

        private static void SeedData(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConfigurationDbContext>();
                if (!context.Clients.Any())
                {
                    Console.WriteLine("Clients being populated");
                    foreach (var client in Config.GetClients().ToList())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }
                else
                {
                    Console.WriteLine("Clients already populated");
                }

                if (!context.IdentityResources.Any())
                {
                    Console.WriteLine("IdentityResources being populated");
                    foreach (var resource in Config.GetIdentityResources().ToList())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
                else
                {
                    Console.WriteLine("IdentityResources already populated");
                }

                if (!context.ApiResources.Any())
                {
                    Console.WriteLine("ApiResources being populated");
                    foreach (var resource in Config.GetApiResources().ToList())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
                else
                {
                    Console.WriteLine("ApiResources already populated");
                }
            }

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                if (!userManager.Users.Any())
                {
                    foreach (var user in Config.GetTestUsers().ToList())
                    {
                        var obj = userManager.FindByNameAsync(user.Username).Result;
                        if (obj == null)
                        {
                            obj = new ApplicationUser
                            {
                                UserName = user.Username,
                                Email = user.Claims.FirstOrDefault(c => c.Type.Equals("email")).Value,
                                EmailConfirmed = true
                            };
                            var result = userManager.CreateAsync(obj, user.Password).Result;
                            if (!result.Succeeded)
                            {
                                throw new Exception(result.Errors.First().Description);
                            }

                            result = userManager.AddClaimsAsync(obj, user.Claims).Result;
                            if (!result.Succeeded)
                            {
                                throw new Exception(result.Errors.First().Description);
                            }
                        }
                    }
                }
            }
        }
    }
}

