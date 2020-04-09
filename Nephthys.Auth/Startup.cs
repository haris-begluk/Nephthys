using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nephthys.Admin.Data.DataHelpers;
using Nephthys.Admin.Data.Entities;
using System.Reflection;

namespace Nephthys.Auth
{
    public class Startup
    {
        private readonly IConfiguration Configuration;

        public IWebHostEnvironment Environment { get; }
        private readonly string ConnString;
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
            ConnString = Configuration.GetConnectionString("AuthServer");
        }

        public void ConfigureServices(IServiceCollection services)
        {

            // uncomment, if you want to add an MVC-based UI
            services.AddControllersWithViews();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(ConnString));

            services.AddIdentity<ApplicationUser, ApplicationRole>()
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders();

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var builder = services.AddIdentityServer()
                .AddTestUsers(TestUsers.Users);

            builder.AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder =>
                    builder.UseSqlServer(ConnString,
                    options => options.MigrationsAssembly(migrationsAssembly));
            });

            builder.AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder =>
                    builder.UseSqlServer(ConnString,
                    options => options.MigrationsAssembly(migrationsAssembly));
            });
            //.AddInMemoryIdentityResources(Config.Ids)
            //.AddInMemoryApiResources(Config.Apis)
            //.AddInMemoryClients(Config.Clients)
            //.AddTestUsers(TestUsers.Users);

            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.MigrateWithData();
            // uncomment if you want to add MVC
            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();

            // uncomment, if you want to add MVC
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
