using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using eShop.Identity.API.Data;
using eShop.Identity.API.Models;
using eShop.Identity.API.Services;
using eShop.Identity.API.Configuration;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using System;

namespace Identity.API.Tests
{
    public class TestStartup : eShop.Identity.API.Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            // Add DbContext with in-memory database
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDb");
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }, contextLifetime: ServiceLifetime.Scoped, optionsLifetime: ServiceLifetime.Singleton);

            // Configure Identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Configure IdentityServer with minimal setup
            services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.IssuerUri = "http://localhost";
                options.Authentication.CookieLifetime = TimeSpan.FromHours(2);
            })
            .AddInMemoryClients(new[]
            {
                new Client
                {
                    ClientId = "basketswaggerui",
                    ClientName = "Basket Swagger UI",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedScopes = { "basket" }
                }
            })
            .AddInMemoryApiScopes(new[]
            {
                new ApiScope("basket", "Basket API")
            })
            .AddInMemoryApiResources(new[]
            {
                new ApiResource("basket", "Basket API")
                {
                    Scopes = { "basket" }
                }
            })
            .AddInMemoryIdentityResources(new[]
            {
                new IdentityResources.OpenId()
            })
            .AddAspNetIdentity<ApplicationUser>()
            .AddDeveloperSigningCredential();

            services.AddTransient<IProfileService, ProfileService>();
            services.AddTransient<ILoginService<ApplicationUser>, EFLoginService>();
            services.AddTransient<IRedirectService, RedirectService>();

            // Configure minimal test settings
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            });

            // Configure IdentityServer test settings
            services.PostConfigure<Duende.IdentityServer.Configuration.IdentityServerOptions>(options =>
            {
                options.IssuerUri = "http://localhost";
                options.Authentication.CookieLifetime = TimeSpan.FromHours(2);
            });
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
