using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using eShop.Identity.API.Data;
using eShop.Identity.API.Models;
using eShop.Identity.API.Services;
using eShop.Identity.API.Configuration;

namespace eShop.Identity.API;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddControllersWithViews();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("identitydb")));

        services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

        services.AddIdentityServer(options =>
        {
            options.Authentication.CookieLifetime = TimeSpan.FromHours(2);
            options.Events.RaiseErrorEvents = true;
            options.Events.RaiseInformationEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseSuccessEvents = true;
            options.KeyManagement.Enabled = false;
        })
        .AddInMemoryIdentityResources(Config.GetResources())
        .AddInMemoryApiScopes(Config.GetApiScopes())
        .AddInMemoryApiResources(Config.GetApis())
        .AddInMemoryClients(Config.GetClients(Configuration))
        .AddAspNetIdentity<ApplicationUser>()
        .AddDeveloperSigningCredential();


        services.AddTransient<IProfileService, ProfileService>();
        services.AddTransient<ILoginService<ApplicationUser>, EFLoginService>();
        services.AddTransient<IRedirectService, RedirectService>();
    }

    public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });
        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
        });
    }
}
