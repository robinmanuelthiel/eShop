using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using eShop.Identity.API.Data;
using eShop.Identity.API.Models;
using eShop.Identity.API.Services;
using eShop.Identity.API.Configuration;

namespace eShop.Identity.API;

public class Program
{
    public static void Main(string[] args)
    {
        var app = CreateWebApplication(args);
        app.Run();
    }

    // This method is used by both the runtime and test scenarios
    public static WebApplication CreateWebApplication(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder);

        var app = builder.Build();

        ConfigureMiddleware(app);

        return app;
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.AddServiceDefaults();

        builder.Services.AddControllersWithViews();

        builder.AddNpgsqlDbContext<ApplicationDbContext>("identitydb");

        // Apply database migration automatically. Note that this approach is not
        // recommended for production scenarios. Consider generating SQL scripts from
        // migrations instead.
        builder.Services.AddMigration<ApplicationDbContext, UsersSeed>();

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

        builder.Services.AddIdentityServer(options =>
        {
            //options.IssuerUri = "null";
            options.Authentication.CookieLifetime = TimeSpan.FromHours(2);

            options.Events.RaiseErrorEvents = true;
            options.Events.RaiseInformationEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseSuccessEvents = true;

            // TODO: Remove this line in production.
            options.KeyManagement.Enabled = false;
        })
        .AddInMemoryIdentityResources(Config.GetResources())
        .AddInMemoryApiScopes(Config.GetApiScopes())
        .AddInMemoryApiResources(Config.GetApis())
        .AddInMemoryClients(Config.GetClients(builder.Configuration))
        .AddAspNetIdentity<ApplicationUser>()
        // TODO: Not recommended for production - you need to store your key material somewhere secure
        .AddDeveloperSigningCredential();

        builder.Services.AddTransient<IProfileService, ProfileService>();
        builder.Services.AddTransient<ILoginService<ApplicationUser>, EFLoginService>();
        builder.Services.AddTransient<IRedirectService, RedirectService>();
    }

    private static void ConfigureMiddleware(WebApplication app)
    {
        app.MapDefaultEndpoints();

        app.UseStaticFiles();

        // This cookie policy fixes login issues with Chrome 80+ using HTTP
        app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });
        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();

        app.MapDefaultControllerRoute();
    }
}
