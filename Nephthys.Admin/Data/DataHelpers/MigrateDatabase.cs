using IdentityModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nephthys.Admin.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nephthys.Admin.Data.DataHelpers
{
    public static class MigrateDatabase
    {
        public static void ExecuteMigrations<T>(this IApplicationBuilder app) where T : DbContext
        {

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<T>();
                context.Database.Migrate();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var alice = userManager.FindByNameAsync("alice").Result;
                if (alice == null)
                {
                    alice = new ApplicationUser
                    {
                        UserName = "alice"
                    };
                    var result = userManager.CreateAsync(alice, "Pass123$").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = userManager.AddClaimsAsync(alice, new Claim[]{
                        new Claim(JwtClaimTypes.Name, "Alice Smith"),
                        new Claim(JwtClaimTypes.GivenName, "Alice"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                        new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
                    }).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                }
                else
                {
                }

                var bob = userManager.FindByNameAsync("bob").Result;
                if (bob == null)
                {
                    bob = new ApplicationUser
                    {
                        UserName = "bob"
                    };
                    var result = userManager.CreateAsync(bob, "Pass123$").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = userManager.AddClaimsAsync(bob, new Claim[]{
                        new Claim(JwtClaimTypes.Name, "Bob Smith"),
                        new Claim(JwtClaimTypes.GivenName, "Bob"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                        new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
                        new Claim("location", "somewhere")
                    }).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                }
                else
                {
                }
            }
        }
    }
}
