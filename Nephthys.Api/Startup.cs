using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using Nephthys.Api.Entities;
using Nephthys.Api.Helpers;
using Swashbuckle.AspNetCore.Filters;

namespace Nephthys.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<NephthysDbContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("NephthysDB")).EnableSensitiveDataLogging());
            services.AddControllers();
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "https://localhost:44389/";
                    options.ApiName = "nephthys-api";
                    options.ApiSecret = "A0906E90-ED48-47FD-023F-08D7D9719801";
                    options.RequireHttpsMetadata = false;

                });
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Nephthys Resource Api", Version = "v1" });

                string xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "User Authentication Token",
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows()
                    {
                        AuthorizationCode = new OpenApiOAuthFlow()
                        {
                            TokenUrl = new Uri("https://localhost:44389/connect/token"),
                            AuthorizationUrl = new Uri("https://localhost:44389/connect/authorize"),
                            Scopes = new Dictionary<string, string>
                        {
                            { "openid profile nephthys-api", "Scopes for the access token request" }
                        }
                        },
                    },
                    Description = "Authentication using OAuth2 \"bearer {token}\""

                });
                options.OperationFilter<SecurityRequirementsOperationFilter>();
                options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.MigrateAndGenerateData();
            app.UseHttpsRedirection();
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Nephthys Resource Api");
                options.OAuthClientId("swagger-api");
                options.OAuth2RedirectUrl("https://localhost:44356/swagger/oauth2-redirect.html");
            });
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
